using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DownSort.Infrastructure.Services
{
    public class UndoService : IUndoService
    {
        private readonly Stack<MoveLogEntry> _undoStack;
        private readonly ISettingsService _settingsService;
        private readonly ILogger<UndoService> _logger;
        private readonly object _lock = new();

        public bool CanUndo => _undoStack.Count > 0;

        public UndoService(ISettingsService settingsService, ILogger<UndoService> logger)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _undoStack = new Stack<MoveLogEntry>();
        }

        public void Push(MoveLogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (!entry.CanUndo)
                return;

            lock (_lock)
            {
                _undoStack.Push(entry);

                // Enforce capacity limit
                var capacity = _settingsService.Current.UndoCapacity;
                while (_undoStack.Count > capacity)
                {
                    var items = _undoStack.ToList();
                    items.Reverse();
                    _undoStack.Clear();
                    foreach (var item in items.Take(capacity))
                    {
                        _undoStack.Push(item);
                    }
                }
            }
        }

        public async Task<bool> UndoLastAsync()
        {
            MoveLogEntry? entry;

            lock (_lock)
            {
                if (_undoStack.Count == 0)
                    return false;

                entry = _undoStack.Pop();
            }

            try
            {
                // Only Move operations are undoable
                if (entry.ActionType != RuleAction.Move)
                    return false;

                // Check if target file still exists
                if (!File.Exists(entry.TargetPath))
                {
                    _logger.LogWarning("Cannot undo: Target file no longer exists: {Target}", entry.TargetPath);
                    return false;
                }

                // Check if source location is available
                if (File.Exists(entry.SourcePath))
                {
                    // Source path is occupied, need to resolve conflict
                    var directory = Path.GetDirectoryName(entry.SourcePath) ?? string.Empty;
                    var fileName = Path.GetFileNameWithoutExtension(entry.SourcePath);
                    var extension = Path.GetExtension(entry.SourcePath);
                    
                    var counter = 1;
                    string newSourcePath;
                    do
                    {
                        newSourcePath = Path.Combine(directory, $"{fileName}_restored_{counter}{extension}");
                        counter++;
                    } while (File.Exists(newSourcePath) && counter < 1000);

                    entry.SourcePath = newSourcePath;
                }

                // Move file back
                await Task.Run(() => File.Move(entry.TargetPath, entry.SourcePath)).ConfigureAwait(false);

                _logger.LogInformation("Undo successful: {Target} -> {Source}", entry.TargetPath, entry.SourcePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to undo operation for: {Target}", entry.TargetPath);
                
                // Put entry back on stack
                lock (_lock)
                {
                    _undoStack.Push(entry);
                }
                
                return false;
            }
        }

        public IReadOnlyList<MoveLogEntry> GetUndoHistory()
        {
            lock (_lock)
            {
                return _undoStack.ToList();
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _undoStack.Clear();
            }
        }
    }
}
