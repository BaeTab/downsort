using System;

namespace DownSort.Domain.Models
{
    /// <summary>
    /// Statistics aggregation model
    /// </summary>
    public class FileStatistics
    {
        public string Extension { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public long TotalBytes { get; set; }
        public DateTimeOffset PeriodStart { get; set; }
        public DateTimeOffset PeriodEnd { get; set; }
    }

    /// <summary>
    /// Processing statistics summary
    /// </summary>
    public class ProcessingStats
    {
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public int SkippedCount { get; set; }
        public DateTimeOffset LastProcessedTime { get; set; }
    }
}
