using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DownSort.Domain.Models;

namespace DownSort.Domain.Services
{
    /// <summary>
    /// Settings persistence service interface
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Load settings from storage
        /// </summary>
        Task<SettingsModel> LoadAsync();

        /// <summary>
        /// Save settings to storage
        /// </summary>
        Task SaveAsync(SettingsModel settings);

        /// <summary>
        /// Get current settings
        /// </summary>
        SettingsModel Current { get; }

        /// <summary>
        /// Settings changed event
        /// </summary>
        event EventHandler<SettingsModel>? SettingsChanged;

        /// <summary>
        /// Reset to default settings
        /// </summary>
        Task ResetToDefaultAsync();
    }
}
