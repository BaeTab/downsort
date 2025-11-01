using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DownSort.Domain.Models;
using DownSort.Infrastructure.Rules;
using FluentAssertions;
using Xunit;

namespace DownSort.Tests
{
    public class RuleEngineTests
    {
        private readonly RuleEngine _ruleEngine;

        public RuleEngineTests()
        {
            _ruleEngine = new RuleEngine();
        }

        [Fact]
        public void MatchesRule_WithExtension_ShouldMatch()
        {
            // Arrange
            var file = new FileInfoLite
            {
                Name = "document.pdf",
                Extension = ".pdf",
                FullPath = "C:\\test\\document.pdf"
            };

            var rule = new RuleModel
            {
                Extensions = new[] { "pdf", "doc" }
            };

            // Act
            var matches = _ruleEngine.MatchesRule(file, rule);

            // Assert
            matches.Should().BeTrue();
        }

        [Fact]
        public void MatchesRule_WithWrongExtension_ShouldNotMatch()
        {
            // Arrange
            var file = new FileInfoLite
            {
                Name = "image.jpg",
                Extension = ".jpg",
                FullPath = "C:\\test\\image.jpg"
            };

            var rule = new RuleModel
            {
                Extensions = new[] { "pdf", "doc" }
            };

            // Act
            var matches = _ruleEngine.MatchesRule(file, rule);

            // Assert
            matches.Should().BeFalse();
        }

        [Fact]
        public void MatchesRule_WithSizeConstraints_ShouldMatch()
        {
            // Arrange
            var file = new FileInfoLite
            {
                Name = "file.txt",
                Extension = ".txt",
                SizeBytes = 5000,
                FullPath = "C:\\test\\file.txt"
            };

            var rule = new RuleModel
            {
                MinSizeBytes = 1000,
                MaxSizeBytes = 10000
            };

            // Act
            var matches = _ruleEngine.MatchesRule(file, rule);

            // Assert
            matches.Should().BeTrue();
        }

        [Fact]
        public void MatchesRule_WithIncludeKeyword_ShouldMatch()
        {
            // Arrange
            var file = new FileInfoLite
            {
                Name = "invoice_2024.pdf",
                Extension = ".pdf",
                FullPath = "C:\\test\\invoice_2024.pdf"
            };

            var rule = new RuleModel
            {
                IncludeKeywords = new[] { "invoice", "receipt" }
            };

            // Act
            var matches = _ruleEngine.MatchesRule(file, rule);

            // Assert
            matches.Should().BeTrue();
        }

        [Fact]
        public void MatchesRule_WithExcludeKeyword_ShouldNotMatch()
        {
            // Arrange
            var file = new FileInfoLite
            {
                Name = "temp_file.txt",
                Extension = ".txt",
                FullPath = "C:\\test\\temp_file.txt"
            };

            var rule = new RuleModel
            {
                ExcludeKeywords = new[] { "temp", "tmp" }
            };

            // Act
            var matches = _ruleEngine.MatchesRule(file, rule);

            // Assert
            matches.Should().BeFalse();
        }

        [Fact]
        public void RenderTargetPath_WithMacros_ShouldReplace()
        {
            // Arrange
            var file = new FileInfoLite
            {
                Name = "document.pdf",
                Extension = ".pdf",
                FullPath = "C:\\Downloads\\document.pdf",
                Directory = "C:\\Downloads"
            };

            var rule = new RuleModel
            {
                Category = "Documents"
            };

            var template = "C:\\Sorted\\{Category}\\{YYYY}\\{MM}\\";

            // Act
            var result = _ruleEngine.RenderTargetPath(template, file, rule, "C:\\Downloads");

            // Assert
            result.Should().Contain("Documents");
            result.Should().Contain(DateTime.Now.Year.ToString());
            result.Should().EndWith("document.pdf");
        }

        [Fact]
        public async Task EvaluateAsync_WithMultipleFiles_ShouldCreatePlans()
        {
            // Arrange
            var files = new[]
            {
                new FileInfoLite { Name = "doc1.pdf", Extension = ".pdf", FullPath = "C:\\test\\doc1.pdf", Directory = "C:\\test" },
                new FileInfoLite { Name = "image1.jpg", Extension = ".jpg", FullPath = "C:\\test\\image1.jpg", Directory = "C:\\test" },
                new FileInfoLite { Name = "doc2.pdf", Extension = ".pdf", FullPath = "C:\\test\\doc2.pdf", Directory = "C:\\test" }
            };

            var rules = new[]
            {
                new RuleModel
                {
                    Enabled = true,
                    Priority = 1,
                    Extensions = new[] { "pdf" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = "C:\\Docs\\",
                    Category = "Documents"
                },
                new RuleModel
                {
                    Enabled = true,
                    Priority = 2,
                    Extensions = new[] { "jpg" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = "C:\\Images\\",
                    Category = "Images"
                }
            };

            // Act
            var plans = await _ruleEngine.EvaluateAsync(files, rules, NameConflictStrategy.AutoRename);

            // Assert
            plans.Should().HaveCount(3);
            plans.Count(p => p.MatchedRule?.Category == "Documents").Should().Be(2);
            plans.Count(p => p.MatchedRule?.Category == "Images").Should().Be(1);
        }

        [Fact]
        public async Task EvaluateAsync_WithDisabledRule_ShouldSkip()
        {
            // Arrange
            var files = new[]
            {
                new FileInfoLite { Name = "doc1.pdf", Extension = ".pdf", FullPath = "C:\\test\\doc1.pdf", Directory = "C:\\test" }
            };

            var rules = new[]
            {
                new RuleModel
                {
                    Enabled = false,
                    Extensions = new[] { "pdf" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = "C:\\Docs\\"
                }
            };

            // Act
            var plans = await _ruleEngine.EvaluateAsync(files, rules, NameConflictStrategy.AutoRename);

            // Assert
            plans.Should().BeEmpty();
        }

        [Fact]
        public async Task EvaluateAsync_WithStopProcessing_ShouldStopAfterFirstMatch()
        {
            // Arrange
            var files = new[]
            {
                new FileInfoLite { Name = "doc1.pdf", Extension = ".pdf", FullPath = "C:\\test\\doc1.pdf", Directory = "C:\\test" }
            };

            var rules = new[]
            {
                new RuleModel
                {
                    Enabled = true,
                    Priority = 1,
                    Extensions = new[] { "pdf" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = "C:\\Docs\\",
                    StopProcessingFurtherRules = true,
                    Name = "FirstRule"
                },
                new RuleModel
                {
                    Enabled = true,
                    Priority = 2,
                    Extensions = new[] { "pdf" },
                    ActionType = RuleAction.Move,
                    TargetTemplate = "C:\\Other\\",
                    StopProcessingFurtherRules = false,
                    Name = "SecondRule"
                }
            };

            // Act
            var plans = await _ruleEngine.EvaluateAsync(files, rules, NameConflictStrategy.AutoRename);

            // Assert
            plans.Should().HaveCount(1);
            plans.First().MatchedRule?.Name.Should().Be("FirstRule");
        }
    }
}
