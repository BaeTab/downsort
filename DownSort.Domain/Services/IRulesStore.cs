using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DownSort.Domain.Models;

namespace DownSort.Domain.Services
{
    /// <summary>
    /// Rules persistence service interface
    /// </summary>
    public interface IRulesStore
    {
        /// <summary>
        /// Load all rules
        /// </summary>
        Task<IEnumerable<RuleModel>> LoadRulesAsync();

        /// <summary>
        /// Save all rules
        /// </summary>
        Task SaveRulesAsync(IEnumerable<RuleModel> rules);

        /// <summary>
        /// Add a new rule
        /// </summary>
        Task AddRuleAsync(RuleModel rule);

        /// <summary>
        /// Update an existing rule
        /// </summary>
        Task UpdateRuleAsync(RuleModel rule);

        /// <summary>
        /// Delete a rule
        /// </summary>
        Task DeleteRuleAsync(Guid ruleId);

        /// <summary>
        /// Export rules to file
        /// </summary>
        Task ExportRulesAsync(string filePath, IEnumerable<RuleModel> rules);

        /// <summary>
        /// Import rules from file
        /// </summary>
        Task<IEnumerable<RuleModel>> ImportRulesAsync(string filePath);
    }
}
