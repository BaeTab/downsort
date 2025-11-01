using System;

namespace DownSort.Domain.Models
{
    /// <summary>
    /// Application settings model
    /// </summary>
    public class SettingsModel
    {
        public string[] WatchedFolders { get; set; } = Array.Empty<string>();
        public string Language { get; set; } = "ko-KR";
        public string Theme { get; set; } = "Light";
        public int MaxParallelOperations { get; set; } = 4;
        public int UndoCapacity { get; set; } = 100;
        public int LogRetentionDays { get; set; } = 90;
        public bool StartInTray { get; set; } = false;
        public bool StartWithWindows { get; set; } = false;
        public NameConflictStrategy ConflictStrategy { get; set; } = NameConflictStrategy.AutoRename;
        public bool ConfirmDelete { get; set; } = true;
        public int FileLockPollingMaxSeconds { get; set; } = 30;
        public long LargeFileSizeThresholdBytes { get; set; } = 2L * 1024 * 1024 * 1024; // 2GB
        public bool ShowNotifications { get; set; } = true;
        public int NotificationFrequencySeconds { get; set; } = 5;
        public bool AutoStartWatcher { get; set; } = false;
    }
}
