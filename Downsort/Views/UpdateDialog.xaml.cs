using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DownSort.Domain.Models;
using DownSort.Domain.Services;

namespace Downsort.Views
{
    /// <summary>
    /// 업데이트 알림 다이얼로그
    /// </summary>
    public partial class UpdateDialog : Window
    {
        private readonly ReleaseInfo _releaseInfo;
        private readonly IUpdateService _updateService;
        private bool _isDownloading;
        
        public UpdateDialog(ReleaseInfo releaseInfo, IUpdateService updateService)
        {
            InitializeComponent();
            
            _releaseInfo = releaseInfo;
            _updateService = updateService;
            
            LoadReleaseInfo();
        }
        
        private void LoadReleaseInfo()
        {
            // 버전 정보 표시
            TxtCurrentVersion.Text = _updateService.GetCurrentVersion();
            TxtNewVersion.Text = _releaseInfo.Version;
            TxtReleaseName.Text = _releaseInfo.Name;
            TxtReleaseDate.Text = _releaseInfo.PublishedAt.ToString("yyyy-MM-dd");
            
            // 변경사항 표시
            TxtReleaseNotes.Text = _releaseInfo.Body;
        }
        
        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_isDownloading) return;
            
            try
            {
                _isDownloading = true;
                BtnUpdate.IsEnabled = false;
                BtnLater.IsEnabled = false;
                
                // 설치 프로그램 찾기
                var installer = Array.Find(_releaseInfo.Assets, a => a.IsInstaller);
                if (installer == null)
                {
                    MessageBox.Show(
                        "설치 파일을 찾을 수 없습니다.",
                        "업데이트 오류",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                
                // 다운로드 진행
                TxtStatus.Text = "업데이트를 다운로드 중입니다...";
                ProgressBar.Visibility = Visibility.Visible;
                
                var progress = new Progress<int>(percent =>
                {
                    ProgressBar.Value = percent;
                    TxtProgress.Text = $"{percent}%";
                });
                
                var installerPath = await _updateService.DownloadUpdateAsync(installer, progress);
                
                if (installerPath == null)
                {
                    MessageBox.Show(
                        "다운로드에 실패했습니다.",
                        "업데이트 오류",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                
                // 설치 확인
                var result = MessageBox.Show(
                    "업데이트 다운로드가 완료되었습니다.\n\n" +
                    "지금 설치하시겠습니까? (애플리케이션이 종료됩니다)",
                    "업데이트 설치",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    _updateService.InstallUpdate(installerPath);
                }
                else
                {
                    MessageBox.Show(
                        $"업데이트가 다운로드되었습니다.\n\n" +
                        $"나중에 다음 파일을 실행하여 설치하세요:\n{installerPath}",
                        "업데이트 다운로드 완료",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    
                    DialogResult = false;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"업데이트 중 오류가 발생했습니다:\n{ex.Message}",
                    "업데이트 오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                _isDownloading = false;
                BtnUpdate.IsEnabled = true;
                BtnLater.IsEnabled = true;
            }
        }
        
        private void BtnLater_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        
        private void BtnViewOnGitHub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _releaseInfo.HtmlUrl,
                    UseShellExecute = true
                });
            }
            catch
            {
                // 무시
            }
        }
    }
}
