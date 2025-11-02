using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Domain.Services;

namespace DownSort.Infrastructure.Services
{
    /// <summary>
    /// GitHub Releases를 통한 자동 업데이트 서비스
    /// </summary>
    public class UpdateService : IUpdateService
    {
        private const string GitHubApiUrl = "https://api.github.com/repos/BaeTab/downsort/releases/latest";
        private const string UserAgent = "DownSort-UpdateChecker";
        
        private readonly HttpClient _httpClient;
        private readonly string _tempDownloadPath;
        
        public UpdateService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            
            _tempDownloadPath = Path.Combine(Path.GetTempPath(), "DownSort_Updates");
            Directory.CreateDirectory(_tempDownloadPath);
        }
        
        /// <summary>
        /// 최신 릴리스 정보 확인
        /// </summary>
        public async Task<ReleaseInfo?> CheckForUpdatesAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GitHubApiUrl);
                var jsonDoc = JsonDocument.Parse(response);
                var root = jsonDoc.RootElement;
                
                var releaseInfo = new ReleaseInfo
                {
                    TagName = root.GetProperty("tag_name").GetString() ?? string.Empty,
                    Version = root.GetProperty("tag_name").GetString()?.TrimStart('v') ?? string.Empty,
                    Name = root.GetProperty("name").GetString() ?? string.Empty,
                    Body = root.GetProperty("body").GetString() ?? string.Empty,
                    PublishedAt = root.GetProperty("published_at").GetDateTime(),
                    HtmlUrl = root.GetProperty("html_url").GetString() ?? string.Empty,
                    IsPrerelease = root.GetProperty("prerelease").GetBoolean()
                };
                
                // Assets 파싱
                if (root.TryGetProperty("assets", out var assetsElement))
                {
                    var assets = assetsElement.EnumerateArray()
                        .Select(asset => new ReleaseAsset
                        {
                            Name = asset.GetProperty("name").GetString() ?? string.Empty,
                            BrowserDownloadUrl = asset.GetProperty("browser_download_url").GetString() ?? string.Empty,
                            Size = asset.GetProperty("size").GetInt64(),
                            ContentType = asset.GetProperty("content_type").GetString() ?? string.Empty
                        })
                        .ToArray();
                    
                    releaseInfo.Assets = assets;
                }
                
                return releaseInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"업데이트 확인 실패: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 업데이트 다운로드
        /// </summary>
        public async Task<string?> DownloadUpdateAsync(ReleaseAsset asset, IProgress<int>? progress = null)
        {
            try
            {
                var fileName = asset.Name;
                var filePath = Path.Combine(_tempDownloadPath, fileName);
                
                // 이미 다운로드된 파일이 있으면 삭제
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                using var response = await _httpClient.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var canReportProgress = totalBytes != -1 && progress != null;
                
                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
                
                var buffer = new byte[8192];
                long totalBytesRead = 0;
                int bytesRead;
                
                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                    
                    if (canReportProgress)
                    {
                        var progressPercentage = (int)((totalBytesRead * 100) / totalBytes);
                        progress!.Report(progressPercentage);
                    }
                }
                
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"업데이트 다운로드 실패: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 업데이트 설치 (애플리케이션 종료 후 설치 프로그램 실행)
        /// </summary>
        public void InstallUpdate(string installerPath)
        {
            if (!File.Exists(installerPath))
            {
                throw new FileNotFoundException("설치 파일을 찾을 수 없습니다.", installerPath);
            }
            
            try
            {
                // 설치 프로그램 실행 (자동 업데이트 모드)
                var startInfo = new ProcessStartInfo
                {
                    FileName = installerPath,
                    Arguments = "/SILENT /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS",
                    UseShellExecute = true,
                    Verb = "runas" // 관리자 권한 요청
                };
                
                Process.Start(startInfo);
                
                // 현재 애플리케이션 종료
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"업데이트 설치 실패: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 현재 버전 가져오기 - AssemblyInformationalVersion 사용
        /// </summary>
        public string GetCurrentVersion()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                
                // AssemblyInformationalVersion 속성 먼저 확인 (Directory.Build.props의 InformationalVersion)
                var infoVersionAttr = assembly?.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                    .FirstOrDefault() as AssemblyInformationalVersionAttribute;
                
                if (infoVersionAttr != null && !string.IsNullOrEmpty(infoVersionAttr.InformationalVersion))
                {
                    // "1.0.3+metadata" 형식에서 "1.0.3"만 추출
                    var version = infoVersionAttr.InformationalVersion.Split('+')[0];
                    Debug.WriteLine($"현재 버전: {version} (InformationalVersion)");
                    return version;
                }
                
                // Assembly Version으로 폴백
                var assemblyVersion = assembly?.GetName().Version;
                if (assemblyVersion != null)
                {
                    var version = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
                    Debug.WriteLine($"현재 버전: {version} (AssemblyVersion)");
                    return version;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"버전 확인 실패: {ex.Message}");
            }
            
            return "1.0.0";
        }
        
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
