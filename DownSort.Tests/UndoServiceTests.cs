using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Infrastructure.Services;
using DownSort.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DownSort.Tests
{
    public class UndoServiceTests
    {
        private readonly UndoService _undoService;
        private readonly SettingsService _settingsService;

        public UndoServiceTests()
        {
            _settingsService = new SettingsService(NullLogger<SettingsService>.Instance);
            _undoService = new UndoService(_settingsService, NullLogger<UndoService>.Instance);
        }

        [Fact]
        public void Push_ValidEntry_ShouldAddToStack()
        {
            // Arrange
            var entry = new MoveLogEntry
            {
                SourcePath = "C:\\source\\file.txt",
                TargetPath = "C:\\target\\file.txt",
                ActionType = RuleAction.Move,
                Result = "Success",
                CanUndo = true
            };

            // Act
            _undoService.Push(entry);

            // Assert
            _undoService.CanUndo.Should().BeTrue();
            _undoService.GetUndoHistory().Should().HaveCount(1);
        }

        [Fact]
        public void Push_NonUndoableEntry_ShouldNotAdd()
        {
            // Arrange
            var entry = new MoveLogEntry
            {
                SourcePath = "C:\\source\\file.txt",
                TargetPath = "C:\\target\\file.txt",
                ActionType = RuleAction.Delete,
                Result = "Success",
                CanUndo = false
            };

            // Act
            _undoService.Push(entry);

            // Assert
            _undoService.CanUndo.Should().BeFalse();
            _undoService.GetUndoHistory().Should().BeEmpty();
        }

        [Fact]
        public void Push_ExceedsCapacity_ShouldRemoveOldest()
        {
            // Arrange
            var capacity = _settingsService.Current.UndoCapacity;
            
            // Act - Push more than capacity
            for (int i = 0; i < capacity + 10; i++)
            {
                var entry = new MoveLogEntry
                {
                    SourcePath = $"C:\\source\\file{i}.txt",
                    TargetPath = $"C:\\target\\file{i}.txt",
                    ActionType = RuleAction.Move,
                    Result = "Success",
                    CanUndo = true
                };
                _undoService.Push(entry);
            }

            // Assert
            _undoService.GetUndoHistory().Should().HaveCountLessOrEqualTo(capacity);
        }

        [Fact]
        public void Clear_ShouldRemoveAllEntries()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                var entry = new MoveLogEntry
                {
                    SourcePath = $"C:\\source\\file{i}.txt",
                    TargetPath = $"C:\\target\\file{i}.txt",
                    ActionType = RuleAction.Move,
                    Result = "Success",
                    CanUndo = true
                };
                _undoService.Push(entry);
            }

            // Act
            _undoService.Clear();

            // Assert
            _undoService.CanUndo.Should().BeFalse();
            _undoService.GetUndoHistory().Should().BeEmpty();
        }

        [Fact]
        public void GetUndoHistory_ShouldReturnOrderedList()
        {
            // Arrange
            var entries = new[]
            {
                new MoveLogEntry { SourcePath = "file1.txt", TargetPath = "target1.txt", ActionType = RuleAction.Move, CanUndo = true },
                new MoveLogEntry { SourcePath = "file2.txt", TargetPath = "target2.txt", ActionType = RuleAction.Move, CanUndo = true },
                new MoveLogEntry { SourcePath = "file3.txt", TargetPath = "target3.txt", ActionType = RuleAction.Move, CanUndo = true }
            };

            foreach (var entry in entries)
            {
                _undoService.Push(entry);
            }

            // Act
            var history = _undoService.GetUndoHistory();

            // Assert
            history.Should().HaveCount(3);
            history.First().SourcePath.Should().Be("file3.txt"); // Most recent first
        }
    }
}
