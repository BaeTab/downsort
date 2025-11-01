using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using DownSort.Domain.Models;
using DownSort.Domain.Services;
using Downsort.Views;

namespace Downsort.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IFileWatcherService _watcherService;
        private readonly IRuleEngine _ruleEngine;
        private readonly IFileOpService _fileOpService;
        private readonly ISettingsService _settingsService;
        private readonly IRulesStore _rulesStore;
        private readonly ILogStore _logStore;
        private readonly IUndoService _undoService;
        private readonly IUpdateService _updateService;

        private bool _isWatcherRunning;
        private string _statusMessage = "준비";
        private int _processedCount;
        private int _queuedCount;
        private bool _isProcessing;

        public ObservableCollection<MovePlan> PreviewItems { get; }
        public ObservableCollection<RuleModel> Rules { get; }
        public ObservableCollection<MoveLogEntry> RecentLogs { get; }

        public bool IsWatcherRunning
        {
            get => _isWatcherRunning;
            set => SetProperty(ref _isWatcherRunning, value, nameof(IsWatcherRunning));
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value, nameof(StatusMessage));
        }

        public int ProcessedCount
        {
            get => _processedCount;
            set => SetProperty(ref _processedCount, value, nameof(ProcessedCount));
        }

        public int QueuedCount
        {
            get => _queuedCount;
            set => SetProperty(ref _queuedCount, value, nameof(QueuedCount));
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value, nameof(IsProcessing));
        }

        public ICommand ToggleWatcherCommand { get; }
        public ICommand ScanCommand { get; }
        public ICommand ExecuteSelectedCommand { get; }
        public ICommand UndoLastCommand { get; }
        public ICommand ClearPreviewCommand { get; }
        public ICommand OpenFolderCommand { get; }
        public ICommand CheckUpdateCommand { get; }

        public MainViewModel(
            IFileWatcherService watcherService,
            IRuleEngine ruleEngine,
            IFileOpService fileOpService,
            ISettingsService settingsService,
            IRulesStore rulesStore,
            ILogStore logStore,
            IUndoService undoService,
            IUpdateService updateService)
        {
            _watcherService = watcherService ?? throw new ArgumentNullException(nameof(watcherService));
            _ruleEngine = ruleEngine ?? throw new ArgumentNullException(nameof(ruleEngine));
            _fileOpService = fileOpService ?? throw new ArgumentNullException(nameof(fileOpService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _rulesStore = rulesStore ?? throw new ArgumentNullException(nameof(rulesStore));
            _logStore = logStore ?? throw new ArgumentNullException(nameof(logStore));
            _undoService = undoService ?? throw new ArgumentNullException(nameof(undoService));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));

            PreviewItems = new ObservableCollection<MovePlan>();
            Rules = new ObservableCollection<RuleModel>();
            RecentLogs = new ObservableCollection<MoveLogEntry>();

            ToggleWatcherCommand = new DelegateCommand(async () => await ToggleWatcherAsync(), () => !IsProcessing);
            ScanCommand = new DelegateCommand(async () => await ScanAsync(), () => !IsProcessing);
            ExecuteSelectedCommand = new DelegateCommand(async () => await ExecuteSelectedAsync(), 
                () => PreviewItems.Any(p => p.IsSelected) && !IsProcessing);
            UndoLastCommand = new DelegateCommand(async () => await UndoLastAsync(), () => _undoService.CanUndo);
            ClearPreviewCommand = new DelegateCommand(() => PreviewItems.Clear());
            OpenFolderCommand = new DelegateCommand<MoveLogEntry>(OpenFolder);
            CheckUpdateCommand = new DelegateCommand(async () => await CheckForUpdatesAsync());

            _watcherService.FileDetected += OnFileDetected;
            _watcherService.ErrorOccurred += OnWatcherError;

            // Load initial data
            Task.Run(async () => await InitializeAsync());
        }

        private void OpenFolder(MoveLogEntry? logEntry)
        {
            if (logEntry == null)
                return;

            try
            {
                var filePath = logEntry.TargetPath;

                if (File.Exists(filePath))
                {
                    // Open folder and select the file
                    Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                    StatusMessage = $"Opened: {Path.GetFileName(filePath)}";
                }
                else
                {
                    // If file doesn't exist, just open the folder
                    var directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        Process.Start("explorer.exe", directory);
                        StatusMessage = $"Opened folder: {directory}";
                    }
                    else
                    {
                        StatusMessage = "File or folder not found";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to open folder: {ex.Message}";
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                await _settingsService.LoadAsync();
                var rules = await _rulesStore.LoadRulesAsync();
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var rule in rules)
                    {
                        Rules.Add(rule);
                    }
                });

                var recentLogs = await _logStore.GetRecentLogsAsync(50);
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var log in recentLogs)
                    {
                        RecentLogs.Add(log);
                    }
                });

                StatusMessage = "준비 완료";

                if (_settingsService.Current.AutoStartWatcher)
                {
                    await ToggleWatcherAsync();
                }

                // 시작 시 업데이트 확인 (백그라운드)
                _ = CheckForUpdatesOnStartupAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"초기화 오류: {ex.Message}";
            }
        }

        /// <summary>
        /// 시작 시 자동 업데이트 확인 (백그라운드)
        /// </summary>
        private async Task CheckForUpdatesOnStartupAsync()
        {
            try
            {
                var settings = _settingsService.Current;
                
                // 설정에서 비활성화했거나 최근 확인했으면 스킵
                if (!settings.CheckForUpdatesOnStartup)
                    return;
                
                if ((DateTime.Now - settings.LastUpdateCheck).TotalHours < 24)
                    return;
                
                // GitHub에서 최신 릴리스 확인
                var releaseInfo = await _updateService.CheckForUpdatesAsync();
                
                if (releaseInfo == null)
                    return;
                
                // 마지막 확인 시간 업데이트
                settings.LastUpdateCheck = DateTime.Now;
                await _settingsService.SaveAsync(settings);
                
                // 새 버전이 있으면 다이얼로그 표시
                if (releaseInfo.IsNewerThan(_updateService.GetCurrentVersion()))
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var dialog = new UpdateDialog(releaseInfo, _updateService);
                        if (Application.Current.MainWindow != null)
                        {
                            dialog.Owner = Application.Current.MainWindow;
                        }
                        dialog.ShowDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                // 업데이트 확인 실패는 조용히 무시
                Debug.WriteLine($"업데이트 확인 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 수동 업데이트 확인
        /// </summary>
        private async Task CheckForUpdatesAsync()
        {
            try
            {
                StatusMessage = "업데이트를 확인하는 중...";
                
                var releaseInfo = await _updateService.CheckForUpdatesAsync();
                
                if (releaseInfo == null)
                {
                    MessageBox.Show(
                        "업데이트 정보를 가져올 수 없습니다.\n\n" +
                        "인터넷 연결을 확인하거나 나중에 다시 시도하세요.",
                        "업데이트 확인",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    StatusMessage = "준비";
                    return;
                }
                
                if (releaseInfo.IsNewerThan(_updateService.GetCurrentVersion()))
                {
                    var dialog = new UpdateDialog(releaseInfo, _updateService);
                    if (Application.Current.MainWindow != null)
                    {
                        dialog.Owner = Application.Current.MainWindow;
                    }
                    dialog.ShowDialog();
                    StatusMessage = "준비";
                }
                else
                {
                    MessageBox.Show(
                        $"현재 최신 버전을 사용 중입니다.\n\n" +
                        $"현재 버전: v{_updateService.GetCurrentVersion()}",
                        "업데이트 확인",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    StatusMessage = "준비";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"업데이트 확인 중 오류가 발생했습니다:\n\n{ex.Message}",
                    "오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StatusMessage = $"오류: {ex.Message}";
            }
        }

        private async Task ToggleWatcherAsync()
        {
            try
            {
                if (IsWatcherRunning)
                {
                    await _watcherService.StopAsync();
                    IsWatcherRunning = false;
                    StatusMessage = "감시 중지됨";
                }
                else
                {
                    await _watcherService.StartAsync();
                    IsWatcherRunning = true;
                    StatusMessage = "파일 감시 중...";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"오류: {ex.Message}";
            }
        }

        private async Task ScanAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "스캔 중...";

                // UI 스레드에서 Clear 호출
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PreviewItems.Clear();
                });

                var settings = _settingsService.Current;
                var folders = settings.WatchedFolders;

                if (folders.Length == 0)
                {
                    StatusMessage = "감시 폴더가 설정되지 않았습니다.";
                    return;
                }

                int totalCount = 0;

                foreach (var folder in folders)
                {
                    var files = await _watcherService.ScanFolderAsync(folder);
                    var plans = await _ruleEngine.EvaluateAsync(files, Rules, settings.ConflictStrategy);

                    // UI 스레드에서 추가
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        foreach (var plan in plans)
                        {
                            PreviewItems.Add(plan);
                            totalCount++;
                        }
                    });
                }

                QueuedCount = totalCount;
                StatusMessage = $"스캔 완료: {totalCount}개 파일 발견";
            }
            catch (Exception ex)
            {
                StatusMessage = $"스캔 오류: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task ExecuteSelectedAsync()
        {
            try
            {
                IsProcessing = true;
                var selected = PreviewItems.Where(p => p.IsSelected).ToList();
                var successCount = 0;
                var failedCount = 0;

                StatusMessage = $"처리 중: {selected.Count}개 파일...";

                foreach (var plan in selected)
                {
                    MoveLogEntry logEntry;

                    switch (plan.ActionType)
                    {
                        case RuleAction.Move:
                            logEntry = await _fileOpService.MoveAsync(
                                plan.Source.FullPath,
                                plan.TargetPath,
                                plan.ConflictPolicy);
                            break;

                        case RuleAction.Copy:
                            logEntry = await _fileOpService.CopyAsync(
                                plan.Source.FullPath,
                                plan.TargetPath,
                                plan.ConflictPolicy);
                            break;

                        case RuleAction.Delete:
                            logEntry = await _fileOpService.DeleteAsync(plan.Source.FullPath);
                            break;

                        default:
                            continue;
                    }

                    logEntry.RuleName = plan.MatchedRule?.Name ?? "Unknown";
                    logEntry.RuleId = plan.MatchedRule?.Id ?? Guid.Empty;

                    await _logStore.AddLogAsync(logEntry);
                    
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        RecentLogs.Insert(0, logEntry);
                        if (RecentLogs.Count > 100)
                            RecentLogs.RemoveAt(RecentLogs.Count - 1);
                    });

                    if (logEntry.Result == "Success")
                    {
                        successCount++;
                        if (logEntry.CanUndo)
                            _undoService.Push(logEntry);
                    }
                    else
                    {
                        failedCount++;
                    }

                    plan.Status = logEntry.Result == "Success" 
                        ? PlannedActionStatus.Success 
                        : PlannedActionStatus.Failed;
                    plan.ErrorMessage = logEntry.ErrorMessage;
                }

                ProcessedCount += successCount;
                StatusMessage = $"완료: 성공 {successCount}, 실패 {failedCount}";

                // Remove successful items
                var toRemove = PreviewItems.Where(p => p.Status == PlannedActionStatus.Success).ToList();
                foreach (var item in toRemove)
                {
                    PreviewItems.Remove(item);
                }

                QueuedCount = PreviewItems.Count;
            }
            catch (Exception ex)
            {
                StatusMessage = $"실행 오류: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task UndoLastAsync()
        {
            try
            {
                var success = await _undoService.UndoLastAsync();
                StatusMessage = success ? "실행 취소 완료" : "실행 취소 실패";
            }
            catch (Exception ex)
            {
                StatusMessage = $"실행 취소 오류: {ex.Message}";
            }
        }

        private async void OnFileDetected(object? sender, FileInfoLite file)
        {
            try
            {
                var plans = await _ruleEngine.EvaluateAsync(
                    new[] { file },
                    Rules,
                    _settingsService.Current.ConflictStrategy);

                var plan = plans.FirstOrDefault();
                if (plan != null)
                {
                    // Auto-execute
                    MoveLogEntry logEntry;

                    switch (plan.ActionType)
                    {
                        case RuleAction.Move:
                            logEntry = await _fileOpService.MoveAsync(
                                plan.Source.FullPath,
                                plan.TargetPath,
                                plan.ConflictPolicy);
                            break;

                        case RuleAction.Copy:
                            logEntry = await _fileOpService.CopyAsync(
                                plan.Source.FullPath,
                                plan.TargetPath,
                                plan.ConflictPolicy);
                            break;

                        default:
                            return;
                    }

                    logEntry.RuleName = plan.MatchedRule?.Name ?? "Unknown";
                    logEntry.RuleId = plan.MatchedRule?.Id ?? Guid.Empty;

                    await _logStore.AddLogAsync(logEntry);
                    
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        RecentLogs.Insert(0, logEntry);
                        if (RecentLogs.Count > 100)
                            RecentLogs.RemoveAt(RecentLogs.Count - 1);

                        if (logEntry.Result == "Success")
                        {
                            ProcessedCount++;
                            if (logEntry.CanUndo)
                                _undoService.Push(logEntry);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    StatusMessage = $"자동 처리 오류: {ex.Message}";
                });
            }
        }

        private void OnWatcherError(object? sender, Exception ex)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                StatusMessage = $"감시 오류: {ex.Message}";
            });
        }
    }
}
