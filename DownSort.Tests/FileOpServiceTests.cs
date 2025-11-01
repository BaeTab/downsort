using System;
using System.IO;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Infrastructure.FileSystem;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DownSort.Tests
{
    public class FileOpServiceTests : IDisposable
    {
        private readonly FileOpService _service;
        private readonly string _testDir;

        public FileOpServiceTests()
        {
            _service = new FileOpService(NullLogger<FileOpService>.Instance);
            _testDir = Path.Combine(Path.GetTempPath(), "DownSortTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }

        [Fact]
        public async Task MoveAsync_ValidFile_ShouldSucceed()
        {
            // Arrange
            var sourcePath = Path.Combine(_testDir, "source.txt");
            var targetPath = Path.Combine(_testDir, "target", "source.txt");
            
            File.WriteAllText(sourcePath, "test content");

            // Act
            var result = await _service.MoveAsync(sourcePath, targetPath, NameConflictStrategy.AutoRename);

            // Assert
            result.Result.Should().Be("Success");
            File.Exists(targetPath).Should().BeTrue();
            File.Exists(sourcePath).Should().BeFalse();
        }

        [Fact]
        public async Task MoveAsync_WithConflict_AutoRename_ShouldCreateNewFile()
        {
            // Arrange
            var sourcePath = Path.Combine(_testDir, "source.txt");
            var targetPath = Path.Combine(_testDir, "target.txt");
            
            File.WriteAllText(sourcePath, "source content");
            File.WriteAllText(targetPath, "existing content");

            // Act
            var result = await _service.MoveAsync(sourcePath, targetPath, NameConflictStrategy.AutoRename);

            // Assert
            result.Result.Should().Be("Success");
            result.TargetPath.Should().NotBe(targetPath);
            result.TargetPath.Should().Contain("target_");
            File.Exists(result.TargetPath).Should().BeTrue();
        }

        [Fact]
        public async Task MoveAsync_WithConflict_Skip_ShouldSkip()
        {
            // Arrange
            var sourcePath = Path.Combine(_testDir, "source.txt");
            var targetPath = Path.Combine(_testDir, "target.txt");
            
            File.WriteAllText(sourcePath, "source content");
            File.WriteAllText(targetPath, "existing content");

            // Act
            var result = await _service.MoveAsync(sourcePath, targetPath, NameConflictStrategy.Skip);

            // Assert
            result.Result.Should().Be("Skipped");
            File.Exists(sourcePath).Should().BeTrue();
            File.ReadAllText(targetPath).Should().Be("existing content");
        }

        [Fact]
        public async Task CopyAsync_ValidFile_ShouldSucceed()
        {
            // Arrange
            var sourcePath = Path.Combine(_testDir, "source.txt");
            var targetPath = Path.Combine(_testDir, "copy.txt");
            
            File.WriteAllText(sourcePath, "test content");

            // Act
            var result = await _service.CopyAsync(sourcePath, targetPath, NameConflictStrategy.AutoRename);

            // Assert
            result.Result.Should().Be("Success");
            File.Exists(sourcePath).Should().BeTrue();
            File.Exists(targetPath).Should().BeTrue();
            result.CanUndo.Should().BeFalse(); // Copy operations are not undoable
        }

        [Fact]
        public async Task DeleteAsync_ValidFile_ShouldSucceed()
        {
            // Arrange
            var filePath = Path.Combine(_testDir, "todelete.txt");
            File.WriteAllText(filePath, "delete me");

            // Act
            var result = await _service.DeleteAsync(filePath);

            // Assert
            result.Result.Should().Be("Success");
            File.Exists(filePath).Should().BeFalse();
            result.CanUndo.Should().BeFalse(); // Delete operations are not undoable
        }

        [Fact]
        public void ResolveConflictPath_ShouldGenerateUniqueName()
        {
            // Arrange
            var basePath = Path.Combine(_testDir, "file.txt");
            File.WriteAllText(basePath, "content");

            // Act
            var newPath = _service.ResolveConflictPath(basePath);

            // Assert
            newPath.Should().NotBe(basePath);
            newPath.Should().Contain("file_1.txt");
        }

        [Fact]
        public async Task IsFileLockedAsync_LockedFile_ShouldReturnTrue()
        {
            // Arrange
            var filePath = Path.Combine(_testDir, "locked.txt");
            using var stream = File.Create(filePath);
            stream.Write(new byte[] { 1, 2, 3 });

            // Act
            var isLocked = await _service.IsFileLockedAsync(filePath);

            // Assert (file is locked because stream is still open)
            isLocked.Should().BeTrue();
        }

        [Fact]
        public async Task IsFileLockedAsync_UnlockedFile_ShouldReturnFalse()
        {
            // Arrange
            var filePath = Path.Combine(_testDir, "unlocked.txt");
            File.WriteAllText(filePath, "content");

            // Act
            var isLocked = await _service.IsFileLockedAsync(filePath);

            // Assert
            isLocked.Should().BeFalse();
        }
    }
}
