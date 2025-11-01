using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DownSort.Domain.Models;

namespace DownSort.Domain.Services
{
    /// <summary>
    /// Rule evaluation engine interface
    /// </summary>
    public interface IRuleEngine
    {
        /// <summary>
        /// Evaluate files against rules and create move plans
        /// </summary>
        Task<IEnumerable<MovePlan>> EvaluateAsync(
            IEnumerable<FileInfoLite> files,
            IEnumerable<RuleModel> rules,
            NameConflictStrategy conflictStrategy,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Test if a file matches a specific rule
        /// </summary>
        bool MatchesRule(FileInfoLite file, RuleModel rule);

        /// <summary>
        /// Render target path template with macros
        /// </summary>
        string RenderTargetPath(string template, FileInfoLite file, RuleModel rule, string sourceFolder);
    }
}
