using System;

namespace DownSort.Domain.Models
{
    /// <summary>
    /// Represents a planned file operation
    /// </summary>
    public class MovePlan
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public FileInfoLite Source { get; set; } = new();
        public string TargetPath { get; set; } = string.Empty;
        public RuleModel? MatchedRule { get; set; }
        public RuleAction ActionType { get; set; }
        public NameConflictStrategy ConflictPolicy { get; set; }
        public PlannedActionStatus Status { get; set; } = PlannedActionStatus.Pending;
        public string? ErrorMessage { get; set; }
        public bool IsSelected { get; set; } = true;
        public bool HasConflict { get; set; }
    }
}
