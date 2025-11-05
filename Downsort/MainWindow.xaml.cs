using System;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Downsort.ViewModels;
using DownSort.Domain.Models;
using Hardcodet.Wpf.TaskbarNotification;

namespace Downsort
{
    public partial class MainWindow : ThemedWindow
    {
        private readonly TaskbarIcon? _notifyIcon;
        private bool _isClosing = false;
        
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            
            // Window state changed event for minimize-to-tray
            StateChanged += MainWindow_StateChanged;
            
            // Initialize System Tray Icon
            _notifyIcon = new TaskbarIcon
            {
                ToolTipText = "DownSort - Smart File Organizer"
            };
            
            // Create icon from window icon or use default
            try
            {
                if (Icon != null)
                {
                    _notifyIcon.Icon = ConvertBitmapSourceToIcon(Icon);
                }
            }
            catch
            {
                // ignore
            }
            
            // Tray icon click - restore window
            _notifyIcon.TrayLeftMouseUp += (s, e) => RestoreWindow();
            
            // Tray icon context menu
            var contextMenu = new System.Windows.Controls.ContextMenu();
            
            var showMenuItem = new System.Windows.Controls.MenuItem { Header = "열기 (Show)" };
            showMenuItem.Click += (s, e) => RestoreWindow();
            contextMenu.Items.Add(showMenuItem);
            
            contextMenu.Items.Add(new System.Windows.Controls.Separator());
            
            var exitMenuItem = new System.Windows.Controls.MenuItem { Header = "종료 (Exit)" };
            exitMenuItem.Click += (s, e) => 
            {
                _isClosing = true;
                Close();
            };
            contextMenu.Items.Add(exitMenuItem);
            
            _notifyIcon.ContextMenu = contextMenu;
        }
        
        private System.Drawing.Icon? ConvertBitmapSourceToIcon(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                using var stream = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                
                using var bitmap = new System.Drawing.Bitmap(stream);
                return System.Drawing.Icon.FromHandle(bitmap.GetHicon());
            }
            return null;
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            // Minimize to tray
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                _notifyIcon?.ShowBalloonTip("DownSort", "트레이로 최소화되었습니다", BalloonIcon.Info);
            }
        }
        
        private void RestoreWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_isClosing)
            {
                // Minimize to tray instead of closing
                e.Cancel = true;
                WindowState = WindowState.Minimized;
            }
            else
            {
                // Actually closing - dispose tray icon
                _notifyIcon?.Dispose();
            }
            
            base.OnClosing(e);
        }

        private void OnRecentLogDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            if (e.HitInfo.RowHandle >=0)
            {
                var view = sender as TableView;
                var log = view?.Grid.GetRow(e.HitInfo.RowHandle) as MoveLogEntry;
                
                if (log != null)
                {
                    OpenFolderAndSelectFile(log.TargetPath);
                }
            }
        }

        private void OpenFolderAndSelectFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // Open folder and select the file
                    Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                }
                else
                {
                    // If file doesn't exist, just open the folder
                    var directory = System.IO.Path.GetDirectoryName(filePath);
                    if (Directory.Exists(directory))
                    {
                        Process.Start("explorer.exe", directory);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to open folder: {ex.Message}", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }
    }
}
