using System;
using System.Threading;
using System.Threading.Tasks;
using DownSort.Domain.Models;

namespace DownSort.Domain.Services
{
    /// <summary>
    /// File operation service interface
    /// </summary>
    public interface IFileOpService
    {
        /// <summary>
        /// Move a file to target location
        /// </summary>
        Task<MoveLogEntry> MoveAsync(
            string sourcePath,
            string targetPath,
            NameConflictStrategy conflictStrategy,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Copy a file to target location
        /// </summary>
        Task<MoveLogEntry> CopyAsync(
            string sourcePath,
            string targetPath,
            NameConflictStrategy conflictStrategy,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rename a file
        /// </summary>
        Task<MoveLogEntry> RenameAsync(
            string sourcePath,
            string newName,
            NameConflictStrategy conflictStrategy,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a file
        /// </summary>
        Task<MoveLogEntry> DeleteAsync(
            string filePath,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if file is locked
        /// </summary>
        Task<bool> IsFileLockedAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolve conflict by auto-renaming
        /// </summary>
        string ResolveConflictPath(string targetPath);
    }
}
