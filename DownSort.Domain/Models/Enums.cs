using System;

namespace DownSort.Domain.Models
{
    /// <summary>
    /// Defines the type of action to perform on matched files
    /// </summary>
    public enum RuleAction
    {
        Move,
        Copy,
        Rename,
        Delete
    }

    /// <summary>
    /// Defines how to handle file name conflicts
    /// </summary>
    public enum NameConflictStrategy
    {
        AutoRename,
        Skip,
        Overwrite
    }

    /// <summary>
    /// Status of a planned action
    /// </summary>
    public enum PlannedActionStatus
    {
        Pending,
        Success,
        Skipped,
        Failed
    }
}
