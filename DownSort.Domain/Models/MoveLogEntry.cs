using System;

namespace DownSort.Domain.Models
{
    /// <summary>
    /// Log entry for executed file operations
    /// </summary>
    public class MoveLogEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
        public string SourcePath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public Guid RuleId { get; set; }
        public RuleAction ActionType { get; set; }
        public string Result { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public bool CanUndo { get; set; } = true;
    }
}
