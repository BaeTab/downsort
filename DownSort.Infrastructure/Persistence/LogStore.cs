using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DownSort.Infrastructure.Persistence
{
    public class LogStore : ILogStore
    {
        private readonly ILogger<LogStore> _logger;
        private readonly string _logsPath;
        private readonly List<MoveLogEntry> _cachedLogs;
        private readonly object _lock = new();

        public LogStore(ILogger<LogStore> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DownSort");
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            _logsPath = Path.Combine(appDataPath, "logs.json");
            _cachedLogs = new List<MoveLogEntry>();

            // Load logs on initialization
            Task.Run(async () =>
            {
                try
                {
                    if (File.Exists(_logsPath))
                    {
                        var json = await File.ReadAllTextAsync(_logsPath).ConfigureAwait(false);
                        var logs = JsonSerializer.Deserialize<List<MoveLogEntry>>(json);
                        if (logs != null)
                        {
                            lock (_lock)
                            {
                                _cachedLogs.AddRange(logs);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load logs");
                }
            });
        }

        public async Task AddLogAsync(MoveLogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            lock (_lock)
            {
                _cachedLogs.Add(entry);
            }

            await SaveLogsAsync().ConfigureAwait(false);
        }

        public Task<IEnumerable<MoveLogEntry>> GetLogsAsync(DateTimeOffset from, DateTimeOffset to)
        {
            lock (_lock)
            {
                var filtered = _cachedLogs
                    .Where(l => l.Timestamp >= from && l.Timestamp <= to)
                    .OrderByDescending(l => l.Timestamp)
                    .ToList();

                return Task.FromResult<IEnumerable<MoveLogEntry>>(filtered);
            }
        }

        public Task<IEnumerable<MoveLogEntry>> GetRecentLogsAsync(int count)
        {
            lock (_lock)
            {
                var recent = _cachedLogs
                    .OrderByDescending(l => l.Timestamp)
                    .Take(count)
                    .ToList();

                return Task.FromResult<IEnumerable<MoveLogEntry>>(recent);
            }
        }

        public async Task PurgeOldLogsAsync(int retentionDays)
        {
            var cutoffDate = DateTimeOffset.Now.AddDays(-retentionDays);
            
            lock (_lock)
            {
                var countBefore = _cachedLogs.Count;
                _cachedLogs.RemoveAll(l => l.Timestamp < cutoffDate);
                var removed = countBefore - _cachedLogs.Count;

                if (removed > 0)
                {
                    _logger.LogInformation("Purged {Count} old log entries", removed);
                }
            }

            await SaveLogsAsync().ConfigureAwait(false);
        }

        public async Task ExportToCsvAsync(string filePath, DateTimeOffset from, DateTimeOffset to)
        {
            var logs = await GetLogsAsync(from, to).ConfigureAwait(false);

            var csv = new StringBuilder();
            csv.AppendLine("Timestamp,Source,Target,Action,Rule,Result,Error");

            foreach (var log in logs)
            {
                csv.AppendLine($"\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{EscapeCsv(log.SourcePath)}\",\"{EscapeCsv(log.TargetPath)}\",\"{log.ActionType}\",\"{EscapeCsv(log.RuleName)}\",\"{log.Result}\",\"{EscapeCsv(log.ErrorMessage ?? string.Empty)}\"");
            }

            await File.WriteAllTextAsync(filePath, csv.ToString()).ConfigureAwait(false);
            _logger.LogInformation("Exported {Count} logs to CSV: {Path}", logs.Count(), filePath);
        }

        public Task<IEnumerable<FileStatistics>> GetStatisticsAsync(DateTimeOffset from, DateTimeOffset to)
        {
            lock (_lock)
            {
                var filtered = _cachedLogs
                    .Where(l => l.Timestamp >= from && l.Timestamp <= to && l.Result == "Success")
                    .ToList();

                // Group by extension
                var extStats = filtered
                    .GroupBy(l => Path.GetExtension(l.SourcePath).TrimStart('.').ToLowerInvariant())
                    .Select(g => new FileStatistics
                    {
                        Extension = g.Key,
                        Count = g.Count(),
                        PeriodStart = from,
                        PeriodEnd = to
                    })
                    .OrderByDescending(s => s.Count)
                    .ToList();

                return Task.FromResult<IEnumerable<FileStatistics>>(extStats);
            }
        }

        private async Task SaveLogsAsync()
        {
            try
            {
                List<MoveLogEntry> logsToSave;
                lock (_lock)
                {
                    logsToSave = new List<MoveLogEntry>(_cachedLogs);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(logsToSave, options);
                await File.WriteAllTextAsync(_logsPath, json).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save logs");
            }
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("\"", "\"\"");
        }
    }
}
