using System;
using System.IO;
using System.Diagnostics;
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
                    var directory = Path.GetDirectoryName(filePath);
                    if (Directory.Exists(directory))
                    {
                        Process.Start("explorer.exe", directory);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to open folder: {ex.Message}", 
                    "Error", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
