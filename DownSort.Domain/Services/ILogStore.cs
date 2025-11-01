using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DownSort.Domain.Models;

namespace DownSort.Domain.Services
{
    /// <summary>
    /// Log persistence service interface
    /// </summary>
    public interface ILogStore
    {
        /// <summary>
        /// Add a log entry
        /// </summary>
        Task AddLogAsync(MoveLogEntry entry);

        /// <summary>
        /// Get logs within date range
        /// </summary>
        Task<IEnumerable<MoveLogEntry>> GetLogsAsync(DateTimeOffset from, DateTimeOffset to);

        /// <summary>
        /// Get recent logs
        /// </summary>
        Task<IEnumerable<MoveLogEntry>> GetRecentLogsAsync(int count);

        /// <summary>
        /// Clear old logs beyond retention period
        /// </summary>
        Task PurgeOldLogsAsync(int retentionDays);

        /// <summary>
        /// Export logs to CSV
        /// </summary>
        Task ExportToCsvAsync(string filePath, DateTimeOffset from, DateTimeOffset to);

        /// <summary>
        /// Get statistics for period
        /// </summary>
        Task<IEnumerable<FileStatistics>> GetStatisticsAsync(DateTimeOffset from, DateTimeOffset to);
    }
}
