using System;
using System.IO;

namespace DownSort.Domain.Models
{
    /// <summary>
    /// Lightweight file information snapshot for processing
    /// </summary>
    public class FileInfoLite
    {
        public string FullPath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset ModifiedTime { get; set; }
        public string Directory { get; set; } = string.Empty;

        public static FileInfoLite FromFileInfo(FileInfo fileInfo)
        {
            return new FileInfoLite
            {
                FullPath = fileInfo.FullName,
                Name = fileInfo.Name,
                Extension = fileInfo.Extension,
                SizeBytes = fileInfo.Length,
                CreatedTime = fileInfo.CreationTimeUtc,
                ModifiedTime = fileInfo.LastWriteTimeUtc,
                Directory = fileInfo.DirectoryName ?? string.Empty
            };
        }
    }
}
