using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DownSort.Infrastructure.FileSystem
{
    public class FileWatcherService : IFileWatcherService, IDisposable
    {
        private readonly ISettingsService _settingsService;
        private readonly IFileOpService _fileOpService;
        private readonly ILogger<FileWatcherService> _logger;
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers = new();
        private readonly ConcurrentDictionary<string, DateTime> _pendingFiles = new();
        private Timer? _processingTimer;
        private bool _isRunning;
        private bool _disposed;

        public event EventHandler<FileInfoLite>? FileDetected;
        public event EventHandler<Exception>? ErrorOccurred;

        public bool IsRunning => _isRunning;

        public FileWatcherService(
            ISettingsService settingsService,
            IFileOpService fileOpService,
            ILogger<FileWatcherService> logger)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _fileOpService = fileOpService ?? throw new ArgumentNullException(nameof(fileOpService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_isRunning)
                return Task.CompletedTask;

            var settings = _settingsService.Current;
            var folders = settings.WatchedFolders;

            if (folders.Length == 0)
            {
                // Default to Downloads folder
                folders = new[] { Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads") };
            }

            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    _logger.LogWarning("Watched folder does not exist: {Folder}", folder);
                    continue;
                }

                try
                {
                    var watcher = new FileSystemWatcher(folder)
                    {
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                        EnableRaisingEvents = true,
                        IncludeSubdirectories = false
                    };

                    watcher.Created += OnFileCreated;
                    watcher.Changed += OnFileChanged;
                    watcher.Renamed += OnFileRenamed;
                    watcher.Error += OnError;

                    _watchers[folder] = watcher;
                    _logger.LogInformation("Started watching folder: {Folder}", folder);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to start watching folder: {Folder}", folder);
                    ErrorOccurred?.Invoke(this, ex);
                }
            }

            // Start timer to process pending files
            _processingTimer = new Timer(ProcessPendingFiles, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            _isRunning = true;

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            if (!_isRunning)
                return Task.CompletedTask;

            _processingTimer?.Dispose();
            _processingTimer = null;

            foreach (var watcher in _watchers.Values)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }

            _watchers.Clear();
            _pendingFiles.Clear();
            _isRunning = false;

            _logger.LogInformation("Stopped file watcher service");
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<FileInfoLite>> ScanFolderAsync(
            string folderPath,
            bool recursive = false,
            CancellationToken cancellationToken = default)
        {
            var files = new List<FileInfoLite>();

            if (!Directory.Exists(folderPath))
                return files;

            await Task.Run(() =>
            {
                try
                {
                    var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var fileInfos = new DirectoryInfo(folderPath).GetFiles("*", searchOption);

                    foreach (var fileInfo in fileInfos)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        // Skip system and hidden files
                        if ((fileInfo.Attributes & FileAttributes.Hidden) != 0 ||
                            (fileInfo.Attributes & FileAttributes.System) != 0)
                            continue;

                        files.Add(FileInfoLite.FromFileInfo(fileInfo));
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.LogWarning(ex, "Access denied scanning folder: {Folder}", folderPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error scanning folder: {Folder}", folderPath);
                    ErrorOccurred?.Invoke(this, ex);
                }
            }, cancellationToken).ConfigureAwait(false);

            return files;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            AddPendingFile(e.FullPath);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            AddPendingFile(e.FullPath);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            AddPendingFile(e.FullPath);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            var ex = e.GetException();
            _logger.LogError(ex, "File watcher error");
            ErrorOccurred?.Invoke(this, ex);
        }

        private void AddPendingFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            _pendingFiles[filePath] = DateTime.Now;
        }

        private async void ProcessPendingFiles(object? state)
        {
            var settings = _settingsService.Current;
            var maxWaitSeconds = settings.FileLockPollingMaxSeconds;
            var now = DateTime.Now;
            var filesToProcess = new List<string>();

            foreach (var kvp in _pendingFiles.ToList())
            {
                var filePath = kvp.Key;
                var addedTime = kvp.Value;

                // Check if file is still locked
                var isLocked = await _fileOpService.IsFileLockedAsync(filePath).ConfigureAwait(false);

                if (!isLocked)
                {
                    filesToProcess.Add(filePath);
                    _pendingFiles.TryRemove(filePath, out _);
                }
                else if ((now - addedTime).TotalSeconds > maxWaitSeconds)
                {
                    // Timeout - remove from pending
                    _pendingFiles.TryRemove(filePath, out _);
                    _logger.LogWarning("File lock timeout, skipping: {File}", filePath);
                }
            }

            // Notify detected files
            foreach (var filePath in filesToProcess)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        var fileInfo = new FileInfo(filePath);
                        var fileLite = FileInfoLite.FromFileInfo(fileInfo);
                        FileDetected?.Invoke(this, fileLite);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file: {File}", filePath);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            StopAsync().GetAwaiter().GetResult();
            _disposed = true;
        }
    }
}
