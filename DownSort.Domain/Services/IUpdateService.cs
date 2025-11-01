using System;
using System.Threading.Tasks;
using DownSort.Domain.Models;

namespace DownSort.Domain.Services
{
    /// <summary>
    /// 업데이트 확인 및 다운로드 서비스 인터페이스
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// 최신 릴리스 정보 확인
        /// </summary>
        Task<ReleaseInfo?> CheckForUpdatesAsync();
        
        /// <summary>
        /// 업데이트 다운로드 (백그라운드)
        /// </summary>
        /// <param name="asset">다운로드할 자산</param>
        /// <param name="progress">진행률 콜백 (0-100)</param>
        Task<string?> DownloadUpdateAsync(ReleaseAsset asset, IProgress<int>? progress = null);
        
        /// <summary>
        /// 업데이트 설치 (애플리케이션 종료 후 설치 프로그램 실행)
        /// </summary>
        void InstallUpdate(string installerPath);
        
        /// <summary>
        /// 현재 버전 가져오기
        /// </summary>
        string GetCurrentVersion();
    }
}
