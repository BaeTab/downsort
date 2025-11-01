using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DownSort.Domain.Models;

namespace DownSort.Domain.Services
{
    /// <summary>
    /// Undo service interface
    /// </summary>
    public interface IUndoService
    {
        /// <summary>
        /// Push a log entry to undo stack
        /// </summary>
        void Push(MoveLogEntry entry);

        /// <summary>
        /// Undo the last operation
        /// </summary>
        Task<bool> UndoLastAsync();

        /// <summary>
        /// Get undo history
        /// </summary>
        IReadOnlyList<MoveLogEntry> GetUndoHistory();

        /// <summary>
        /// Clear undo history
        /// </summary>
        void Clear();

        /// <summary>
        /// Check if undo is available
        /// </summary>
        bool CanUndo { get; }
    }
}
