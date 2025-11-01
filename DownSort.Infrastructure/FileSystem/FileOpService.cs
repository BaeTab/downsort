using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DownSort.Infrastructure.FileSystem
{
    public class FileOpService : IFileOpService
    {
        private readonly ILogger<FileOpService> _logger;

        public FileOpService(ILogger<FileOpService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MoveLogEntry> MoveAsync(
            string sourcePath,
            string targetPath,
            NameConflictStrategy conflictStrategy,
            CancellationToken cancellationToken = default)
        {
            var entry = new MoveLogEntry
            {
                SourcePath = sourcePath,
                TargetPath = targetPath,
                ActionType = RuleAction.Move,
                Timestamp = DateTimeOffset.Now
            };

            try
            {
                if (!File.Exists(sourcePath))
                {
                    entry.Result = "Failed";
                    entry.ErrorMessage = "Source file not found";
                    entry.CanUndo = false;
                    return entry;
                }

                // Handle conflict
                var finalTargetPath = targetPath;
                if (File.Exists(targetPath))
                {
                    switch (conflictStrategy)
                    {
                        case NameConflictStrategy.Skip:
                            entry.Result = "Skipped";
                            entry.ErrorMessage = "File already exists";
                            entry.CanUndo = false;
                            return entry;

                        case NameConflictStrategy.AutoRename:
                            finalTargetPath = ResolveConflictPath(targetPath);
                            break;

                        case NameConflictStrategy.Overwrite:
                            File.Delete(targetPath);
                            break;
                    }
                }

                // Ensure target directory exists
                var targetDir = Path.GetDirectoryName(finalTargetPath);
                if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Move file
                await Task.Run(() => File.Move(sourcePath, finalTargetPath), cancellationToken).ConfigureAwait(false);

                entry.TargetPath = finalTargetPath;
                entry.Result = "Success";
                _logger.LogInformation("Moved file: {Source} -> {Target}", sourcePath, finalTargetPath);
            }
            catch (Exception ex)
            {
                entry.Result = "Failed";
                entry.ErrorMessage = ex.Message;
                entry.CanUndo = false;
                _logger.LogError(ex, "Failed to move file: {Source}", sourcePath);
            }

            return entry;
        }

        public async Task<MoveLogEntry> CopyAsync(
            string sourcePath,
            string targetPath,
            NameConflictStrategy conflictStrategy,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var entry = new MoveLogEntry
            {
                SourcePath = sourcePath,
                TargetPath = targetPath,
                ActionType = RuleAction.Copy,
                Timestamp = DateTimeOffset.Now
            };

            try
            {
                if (!File.Exists(sourcePath))
                {
                    entry.Result = "Failed";
                    entry.ErrorMessage = "Source file not found";
                    entry.CanUndo = false;
                    return entry;
                }

                var finalTargetPath = targetPath;
                if (File.Exists(targetPath))
                {
                    switch (conflictStrategy)
                    {
                        case NameConflictStrategy.Skip:
                            entry.Result = "Skipped";
                            entry.ErrorMessage = "File already exists";
                            entry.CanUndo = false;
                            return entry;

                        case NameConflictStrategy.AutoRename:
                            finalTargetPath = ResolveConflictPath(targetPath);
                            break;

                        case NameConflictStrategy.Overwrite:
                            File.Delete(targetPath);
                            break;
                    }
                }

                var targetDir = Path.GetDirectoryName(finalTargetPath);
                if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                await Task.Run(() => File.Copy(sourcePath, finalTargetPath, false), cancellationToken).ConfigureAwait(false);

                entry.TargetPath = finalTargetPath;
                entry.Result = "Success";
                entry.CanUndo = false; // Copy operations are not undoable
                _logger.LogInformation("Copied file: {Source} -> {Target}", sourcePath, finalTargetPath);
            }
            catch (Exception ex)
            {
                entry.Result = "Failed";
                entry.ErrorMessage = ex.Message;
                entry.CanUndo = false;
                _logger.LogError(ex, "Failed to copy file: {Source}", sourcePath);
            }

            return entry;
        }

        public async Task<MoveLogEntry> RenameAsync(
            string sourcePath,
            string newName,
            NameConflictStrategy conflictStrategy,
            CancellationToken cancellationToken = default)
        {
            var directory = Path.GetDirectoryName(sourcePath) ?? string.Empty;
            var targetPath = Path.Combine(directory, newName);
            return await MoveAsync(sourcePath, targetPath, conflictStrategy, cancellationToken).ConfigureAwait(false);
        }

        public async Task<MoveLogEntry> DeleteAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            var entry = new MoveLogEntry
            {
                SourcePath = filePath,
                TargetPath = string.Empty,
                ActionType = RuleAction.Delete,
                Timestamp = DateTimeOffset.Now,
                CanUndo = false
            };

            try
            {
                if (!File.Exists(filePath))
                {
                    entry.Result = "Failed";
                    entry.ErrorMessage = "File not found";
                    return entry;
                }

                await Task.Run(() => File.Delete(filePath), cancellationToken).ConfigureAwait(false);

                entry.Result = "Success";
                _logger.LogInformation("Deleted file: {Path}", filePath);
            }
            catch (Exception ex)
            {
                entry.Result = "Failed";
                entry.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Failed to delete file: {Path}", filePath);
            }

            return entry;
        }

        public async Task<bool> IsFileLockedAsync(string filePath, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                    return false;

                try
                {
                    using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    return false;
                }
                catch (IOException)
                {
                    return true;
                }
                catch
                {
                    return false;
                }
            }, cancellationToken).ConfigureAwait(false);
        }

        public string ResolveConflictPath(string targetPath)
        {
            if (!File.Exists(targetPath))
                return targetPath;

            var directory = Path.GetDirectoryName(targetPath) ?? string.Empty;
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(targetPath);
            var extension = Path.GetExtension(targetPath);

            var counter = 1;
            string newPath;
            do
            {
                var newFileName = $"{fileNameWithoutExt}_{counter}{extension}";
                newPath = Path.Combine(directory, newFileName);
                counter++;
            }
            while (File.Exists(newPath) && counter < 1000);

            return newPath;
        }
    }
}
