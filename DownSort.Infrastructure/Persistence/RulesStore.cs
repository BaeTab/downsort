using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DownSort.Infrastructure.Persistence
{
    public class RulesStore : IRulesStore
    {
        private readonly ILogger<RulesStore> _logger;
        private readonly string _rulesPath;
        private List<RuleModel> _cachedRules;

        public RulesStore(ILogger<RulesStore> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DownSort");
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            _rulesPath = Path.Combine(appDataPath, "rules.json");
            _cachedRules = new List<RuleModel>();
        }

        public async Task<IEnumerable<RuleModel>> LoadRulesAsync()
        {
            try
            {
                if (File.Exists(_rulesPath))
                {
                    var json = await File.ReadAllTextAsync(_rulesPath).ConfigureAwait(false);
                    var rules = JsonSerializer.Deserialize<List<RuleModel>>(json);
                    
                    if (rules != null && rules.Count > 0)
                    {
                        _cachedRules = rules;
                        _logger.LogInformation("Loaded {Count} rules", rules.Count);
                        return _cachedRules;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load rules, using defaults");
            }

            // Create default rules
            _cachedRules = GetDefaultRules();
            await SaveRulesAsync(_cachedRules).ConfigureAwait(false);
            return _cachedRules;
        }

        public async Task SaveRulesAsync(IEnumerable<RuleModel> rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            try
            {
                _cachedRules = rules.ToList();

                // Create backup
                if (File.Exists(_rulesPath))
                {
                    var backupPath = _rulesPath + ".bak";
                    File.Copy(_rulesPath, backupPath, true);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(_cachedRules, options);
                await File.WriteAllTextAsync(_rulesPath, json).ConfigureAwait(false);

                _logger.LogInformation("Saved {Count} rules", _cachedRules.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save rules");
                throw;
            }
        }

        public async Task AddRuleAsync(RuleModel rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            _cachedRules.Add(rule);
            await SaveRulesAsync(_cachedRules).ConfigureAwait(false);
        }

        public async Task UpdateRuleAsync(RuleModel rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            var index = _cachedRules.FindIndex(r => r.Id == rule.Id);
            if (index >= 0)
            {
                _cachedRules[index] = rule;
                await SaveRulesAsync(_cachedRules).ConfigureAwait(false);
            }
        }

        public async Task DeleteRuleAsync(Guid ruleId)
        {
            _cachedRules.RemoveAll(r => r.Id == ruleId);
            await SaveRulesAsync(_cachedRules).ConfigureAwait(false);
        }

        public async Task ExportRulesAsync(string filePath, IEnumerable<RuleModel> rules)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(rules, options);
            await File.WriteAllTextAsync(filePath, json).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RuleModel>> ImportRulesAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
            var rules = JsonSerializer.Deserialize<List<RuleModel>>(json);
            return rules ?? new List<RuleModel>();
        }

        private List<RuleModel> GetDefaultRules()
        {
            var downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");

            return new List<RuleModel>
            {
                new RuleModel
                {
                    Id = Guid.NewGuid(),
                    Enabled = true,
                    Priority = 1,
                    Name = "Documents",
                    Category = "Documents",
                    Extensions = new[] { "pdf", "doc", "docx", "xls", "xlsx", "ppt", "pptx", "txt", "rtf", "odt" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = downloadsPath + "\\{Category}\\",
                    StopProcessingFurtherRules = true
                },
                new RuleModel
                {
                    Id = Guid.NewGuid(),
                    Enabled = true,
                    Priority = 2,
                    Name = "Images",
                    Category = "Images",
                    Extensions = new[] { "jpg", "jpeg", "png", "gif", "bmp", "svg", "webp", "ico" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = downloadsPath + "\\{Category}\\",
                    StopProcessingFurtherRules = true
                },
                new RuleModel
                {
                    Id = Guid.NewGuid(),
                    Enabled = true,
                    Priority = 3,
                    Name = "Archives",
                    Category = "Archives",
                    Extensions = new[] { "zip", "rar", "7z", "tar", "gz", "bz2" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = downloadsPath + "\\{Category}\\",
                    StopProcessingFurtherRules = true
                },
                new RuleModel
                {
                    Id = Guid.NewGuid(),
                    Enabled = true,
                    Priority = 4,
                    Name = "Installers",
                    Category = "Installers",
                    Extensions = new[] { "exe", "msi", "dmg", "pkg" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = downloadsPath + "\\{Category}\\",
                    StopProcessingFurtherRules = true
                },
                new RuleModel
                {
                    Id = Guid.NewGuid(),
                    Enabled = true,
                    Priority = 5,
                    Name = "Videos",
                    Category = "Videos",
                    Extensions = new[] { "mp4", "avi", "mkv", "mov", "wmv", "flv", "webm" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = downloadsPath + "\\{Category}\\",
                    StopProcessingFurtherRules = true
                },
                new RuleModel
                {
                    Id = Guid.NewGuid(),
                    Enabled = true,
                    Priority = 6,
                    Name = "Audio",
                    Category = "Audio",
                    Extensions = new[] { "mp3", "wav", "flac", "aac", "ogg", "wma", "m4a" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = downloadsPath + "\\{Category}\\",
                    StopProcessingFurtherRules = true
                }
            };
        }
    }
}
