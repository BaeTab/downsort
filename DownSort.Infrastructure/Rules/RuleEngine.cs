using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Domain.Services;

namespace DownSort.Infrastructure.Rules
{
    public class RuleEngine : IRuleEngine
    {
        public async Task<IEnumerable<MovePlan>> EvaluateAsync(
            IEnumerable<FileInfoLite> files,
            IEnumerable<RuleModel> rules,
            NameConflictStrategy conflictStrategy,
            CancellationToken cancellationToken = default)
        {
            var orderedRules = rules.Where(r => r.Enabled).OrderBy(r => r.Priority).ToList();
            var plans = new List<MovePlan>();

            await Task.Run(() =>
            {
                foreach (var file in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    RuleModel? matchedRule = null;
                    foreach (var rule in orderedRules)
                    {
                        if (MatchesRule(file, rule))
                        {
                            matchedRule = rule;
                            if (rule.StopProcessingFurtherRules)
                                break;
                        }
                    }

                    if (matchedRule != null)
                    {
                        var targetPath = RenderTargetPath(
                            matchedRule.TargetTemplate,
                            file,
                            matchedRule,
                            file.Directory);

                        var plan = new MovePlan
                        {
                            Source = file,
                            TargetPath = targetPath,
                            MatchedRule = matchedRule,
                            ActionType = matchedRule.ActionType,
                            ConflictPolicy = conflictStrategy,
                            HasConflict = File.Exists(targetPath)
                        };

                        plans.Add(plan);
                    }
                }
            }, cancellationToken).ConfigureAwait(false);

            return plans;
        }

        public bool MatchesRule(FileInfoLite file, RuleModel rule)
        {
            // Extension check
            if (rule.Extensions.Length > 0)
            {
                var ext = file.Extension.TrimStart('.').ToLowerInvariant();
                if (!rule.Extensions.Any(e => e.TrimStart('.').Equals(ext, StringComparison.OrdinalIgnoreCase)))
                    return false;
            }

            // Size check
            if (rule.MinSizeBytes.HasValue && file.SizeBytes < rule.MinSizeBytes.Value)
                return false;

            if (rule.MaxSizeBytes.HasValue && file.SizeBytes > rule.MaxSizeBytes.Value)
                return false;

            // Date check
            if (rule.MinCreated.HasValue && file.CreatedTime < rule.MinCreated.Value)
                return false;

            if (rule.MaxCreated.HasValue && file.CreatedTime > rule.MaxCreated.Value)
                return false;

            // Include keywords
            if (rule.IncludeKeywords.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Name).ToLowerInvariant();
                if (!rule.IncludeKeywords.Any(k => fileName.Contains(k.ToLowerInvariant())))
                    return false;
            }

            // Exclude keywords
            if (rule.ExcludeKeywords.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Name).ToLowerInvariant();
                if (rule.ExcludeKeywords.Any(k => fileName.Contains(k.ToLowerInvariant())))
                    return false;
            }

            // Regex pattern
            if (!string.IsNullOrWhiteSpace(rule.RegexPattern))
            {
                try
                {
                    if (!Regex.IsMatch(file.Name, rule.RegexPattern, RegexOptions.IgnoreCase))
                        return false;
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }

            return true;
        }

        public string RenderTargetPath(string template, FileInfoLite file, RuleModel rule, string sourceFolder)
        {
            var now = DateTimeOffset.Now;
            var rendered = template;

            // Replace macros
            rendered = rendered.Replace("{Category}", rule.Category);
            rendered = rendered.Replace("{Ext}", file.Extension.TrimStart('.'));
            rendered = rendered.Replace("{YYYY}", now.Year.ToString("D4"));
            rendered = rendered.Replace("{MM}", now.Month.ToString("D2"));
            rendered = rendered.Replace("{DD}", now.Day.ToString("D2"));
            rendered = rendered.Replace("{Today}", now.ToString("yyyy-MM-dd"));
            rendered = rendered.Replace("{SourceFolderName}", Path.GetFileName(sourceFolder));
            rendered = rendered.Replace("{FileName}", Path.GetFileNameWithoutExtension(file.Name));

            // Resolve Downloads folder
            rendered = rendered.Replace("{Downloads}",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\");

            // Combine with filename
            var targetDir = rendered;
            var targetFile = Path.Combine(targetDir, file.Name);

            return targetFile;
        }
    }
}
