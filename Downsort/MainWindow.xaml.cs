using System;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Downsort.ViewModels;
using DownSort.Domain.Models;

namespace Downsort
{
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            
            // Window state changed event for maximize/restore button
            StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            // Update maximize/restore button icon
            if (WindowState == WindowState.Maximized)
            {
                MaximizeRestoreIcon.Data = (Geometry)FindResource("RestoreIcon");
                MaximizeRestoreButton.ToolTip = "Restore";
            }
            else
            {
                MaximizeRestoreIcon.Data = (Geometry)FindResource("MaximizeIcon");
                MaximizeRestoreButton.ToolTip = "Maximize";
            }
        }

        // Title Bar Event Handlers
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Double click to maximize/restore
                MaximizeRestoreButton_Click(sender, e);
            }
            else
            {
                // Single click to drag window
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    try
                    {
                        DragMove();
                    }
                    catch (InvalidOperationException)
                    {
                        // Can only call DragMove when primary mouse button is down
                        // Ignore this exception
                    }
                }
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnRecentLogDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            if (e.HitInfo.RowHandle >= 0)
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
