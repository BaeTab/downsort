using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DownSort.Domain.Models;

namespace DownSort.Domain.Services
{
    /// <summary>
    /// File system watcher host interface
    /// </summary>
    public interface IFileWatcherService
    {
        /// <summary>
        /// Start watching folders
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop watching folders
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Check if watcher is running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// File detected event
        /// </summary>
        event EventHandler<FileInfoLite>? FileDetected;

        /// <summary>
        /// Error occurred event
        /// </summary>
        event EventHandler<Exception>? ErrorOccurred;

        /// <summary>
        /// Scan folder manually
        /// </summary>
        Task<IEnumerable<FileInfoLite>> ScanFolderAsync(
            string folderPath,
            bool recursive = false,
            CancellationToken cancellationToken = default);
    }
}
