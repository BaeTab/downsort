using System;

namespace DownSort.Domain.Models
{
    /// <summary>
    /// GitHub 릴리스 정보
    /// </summary>
    public class ReleaseInfo
    {
        public string Version { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public string HtmlUrl { get; set; } = string.Empty;
        public bool IsPrerelease { get; set; }
        public ReleaseAsset[] Assets { get; set; } = Array.Empty<ReleaseAsset>();
        
        /// <summary>
        /// 현재 버전보다 새로운지 확인
        /// </summary>
        public bool IsNewerThan(string currentVersion)
        {
            try
            {
                var current = ParseVersion(currentVersion);
                var release = ParseVersion(this.Version);
                
                if (current == null || release == null)
                    return false;
                
                return release > current;
            }
            catch
            {
                return false;
            }
        }
        
        private static System.Version? ParseVersion(string version)
        {
            // v1.0.1 형식을 1.0.1로 변환
            var cleaned = version.TrimStart('v');
            return System.Version.TryParse(cleaned, out var result) ? result : null;
        }
    }
    
    /// <summary>
    /// 릴리스 자산 (다운로드 파일)
    /// </summary>
    public class ReleaseAsset
    {
        public string Name { get; set; } = string.Empty;
        public string BrowserDownloadUrl { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        
        /// <summary>
        /// 설치 프로그램 파일인지 확인
        /// </summary>
        public bool IsInstaller => Name.EndsWith("-Setup.exe", StringComparison.OrdinalIgnoreCase) ||
                                    Name.EndsWith(".msi", StringComparison.OrdinalIgnoreCase);
        
        /// <summary>
        /// ZIP 파일인지 확인
        /// </summary>
        public bool IsZip => Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);
    }
}
