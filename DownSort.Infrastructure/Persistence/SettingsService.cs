using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DownSort.Infrastructure.Persistence
{
    public class SettingsService : ISettingsService
    {
        private readonly ILogger<SettingsService> _logger;
        private readonly string _settingsPath;
        private SettingsModel _current;

        public event EventHandler<SettingsModel>? SettingsChanged;

        public SettingsModel Current => _current;

        public SettingsService(ILogger<SettingsService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DownSort");
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            _settingsPath = Path.Combine(appDataPath, "settings.json");
            _current = new SettingsModel();
        }

        public async Task<SettingsModel> LoadAsync()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = await File.ReadAllTextAsync(_settingsPath).ConfigureAwait(false);
                    var settings = JsonSerializer.Deserialize<SettingsModel>(json);
                    
                    if (settings != null)
                    {
                        _current = settings;
                        _logger.LogInformation("Settings loaded successfully");
                        return _current;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load settings, using defaults");
            }

            // Return default settings
            _current = GetDefaultSettings();
            await SaveAsync(_current).ConfigureAwait(false);
            return _current;
        }

        public async Task SaveAsync(SettingsModel settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            try
            {
                // Create backup
                if (File.Exists(_settingsPath))
                {
                    var backupPath = _settingsPath + ".bak";
                    File.Copy(_settingsPath, backupPath, true);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(settings, options);
                await File.WriteAllTextAsync(_settingsPath, json).ConfigureAwait(false);

                _current = settings;
                SettingsChanged?.Invoke(this, settings);

                _logger.LogInformation("Settings saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save settings");
                throw;
            }
        }

        public async Task ResetToDefaultAsync()
        {
            _current = GetDefaultSettings();
            await SaveAsync(_current).ConfigureAwait(false);
            _logger.LogInformation("Settings reset to defaults");
        }

        private SettingsModel GetDefaultSettings()
        {
            var downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");

            return new SettingsModel
            {
                WatchedFolders = new[] { downloadsPath },
                Language = "ko-KR",
                Theme = "Light",
                MaxParallelOperations = 4,
                UndoCapacity = 100,
                LogRetentionDays = 90,
                StartInTray = false,
                StartWithWindows = false,
                ConflictStrategy = NameConflictStrategy.AutoRename,
                ConfirmDelete = true,
                FileLockPollingMaxSeconds = 30,
                LargeFileSizeThresholdBytes = 2L * 1024 * 1024 * 1024,
                ShowNotifications = true,
                NotificationFrequencySeconds = 5,
                AutoStartWatcher = false
            };
        }
    }
}
