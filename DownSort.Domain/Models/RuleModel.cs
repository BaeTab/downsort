using System;

namespace DownSort.Domain.Models
{
    /// <summary>
    /// Represents a sorting rule with conditions and actions
    /// </summary>
    public class RuleModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool Enabled { get; set; } = true;
        public int Priority { get; set; }
        public string Name { get; set; } = string.Empty;
        public string[] Extensions { get; set; } = Array.Empty<string>();
        public string[] IncludeKeywords { get; set; } = Array.Empty<string>();
        public string[] ExcludeKeywords { get; set; } = Array.Empty<string>();
        public string? RegexPattern { get; set; }
        public long? MinSizeBytes { get; set; }
        public long? MaxSizeBytes { get; set; }
        public DateTimeOffset? MinCreated { get; set; }
        public DateTimeOffset? MaxCreated { get; set; }
        public RuleAction ActionType { get; set; } = RuleAction.Move;
        public string TargetTemplate { get; set; } = string.Empty;
        public bool StopProcessingFurtherRules { get; set; } = true;
        public string Category { get; set; } = "General";
    }
}
