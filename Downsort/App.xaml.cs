using System;
using System.Windows;
using DevExpress.Xpf.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DownSort.Domain.Services;
using DownSort.Infrastructure.FileSystem;
using DownSort.Infrastructure.Persistence;
using DownSort.Infrastructure.Rules;
using DownSort.Infrastructure.Services;
using Downsort.ViewModels;

namespace Downsort
{
    public partial class App : Application
    {
        private IHost? _host;

        static App()
        {
            CompatibilitySettings.UseLightweightThemes = true;
            ApplicationThemeHelper.Preload(PreloadCategories.Core);
            ApplicationThemeHelper.ApplicationThemeName = Theme.Win11LightName;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Domain Services
                    services.AddSingleton<IRuleEngine, RuleEngine>();
                    services.AddSingleton<IFileOpService, FileOpService>();
                    services.AddSingleton<IFileWatcherService, FileWatcherService>();
                    services.AddSingleton<IUndoService, UndoService>();
                    
                    // Persistence Services
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IRulesStore, RulesStore>();
                    services.AddSingleton<ILogStore, LogStore>();
                    
                    // ViewModels
                    services.AddTransient<MainViewModel>();
                    
                    // Main Window
                    services.AddSingleton<MainWindow>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .Build();

            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }

            base.OnExit(e);
        }
    }
}
