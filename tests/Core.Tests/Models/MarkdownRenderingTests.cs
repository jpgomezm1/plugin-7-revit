using DocumentationGeneratorAI.Core.Models;
using FluentAssertions;
using Xunit;

namespace DocumentationGeneratorAI.Core.Tests.Models;

public class MarkdownRenderingTests
{
    [Fact]
    public void ToMarkdown_EmptySections_ShouldRenderHeaderOnly()
    {
        var doc = new GeneratedDocument
        {
            Title = "Empty Document",
            DocumentType = "Test",
            ProjectName = "Test Project",
            Sections = new List<DocumentSection>()
        };

        var md = doc.ToMarkdown();

        md.Should().Contain("# Empty Document");
        md.Should().Contain("**Project:** Test Project");
        md.Should().NotContain("##");
    }

    [Fact]
    public void ToMarkdown_SpecialCharactersInTitle_ShouldRenderCorrectly()
    {
        var doc = new GeneratedDocument
        {
            Title = "Project \"Alpha\" — Phase 1 & Testing <v2>",
            DocumentType = "Report",
            Sections = new List<DocumentSection>
            {
                new() { Heading = "Content", Content = "Test", Order = 1 }
            }
        };

        var md = doc.ToMarkdown();
        md.Should().Contain("# Project \"Alpha\" — Phase 1 & Testing <v2>");
    }

    [Fact]
    public void ToMarkdown_MarkdownInContent_ShouldPreserve()
    {
        var doc = new GeneratedDocument
        {
            Title = "Test",
            Sections = new List<DocumentSection>
            {
                new()
                {
                    Heading = "Data",
                    Content = "| Column A | Column B |\n|---|---|\n| Value 1 | Value 2 |",
                    Order = 1
                }
            }
        };

        var md = doc.ToMarkdown();
        md.Should().Contain("| Column A | Column B |");
        md.Should().Contain("| Value 1 | Value 2 |");
    }

    [Fact]
    public void ToMarkdown_MultipleWarnings_ShouldRenderAll()
    {
        var doc = new GeneratedDocument
        {
            Title = "Test",
            Warnings = new List<string>
            {
                "Warning 1: Missing rooms",
                "Warning 2: No MEP systems",
                "Warning 3: No floor area"
            },
            Sections = new List<DocumentSection>()
        };

        var md = doc.ToMarkdown();
        md.Should().Contain("> **Warnings:**");
        md.Should().Contain("> - Warning 1: Missing rooms");
        md.Should().Contain("> - Warning 2: No MEP systems");
        md.Should().Contain("> - Warning 3: No floor area");
    }

    [Fact]
    public void ToMarkdown_AllMetadataFields_ShouldRender()
    {
        var doc = new GeneratedDocument
        {
            Title = "Full Test",
            DocumentType = "Technical Specification",
            GeneratedDate = "2025-06-15",
            ProjectName = "Big Project",
            Phase = "Construction",
            Audience = "Client",
            DetailLevel = "Extended",
            Sections = new List<DocumentSection>()
        };

        var md = doc.ToMarkdown();
        md.Should().Contain("**Document Type:** Technical Specification");
        md.Should().Contain("**Project:** Big Project");
        md.Should().Contain("**Phase:** Construction");
        md.Should().Contain("**Audience:** Client");
        md.Should().Contain("**Detail Level:** Extended");
        md.Should().Contain("**Generated:** 2025-06-15");
    }

    [Fact]
    public void ToMarkdown_EmptyStringFields_ShouldNotCrash()
    {
        var doc = new GeneratedDocument
        {
            Title = "",
            DocumentType = "",
            ProjectName = "",
            Phase = "",
            Audience = "",
            DetailLevel = "",
            GeneratedDate = "",
            Sections = new List<DocumentSection>(),
            Warnings = new List<string>()
        };

        var act = () => doc.ToMarkdown();
        act.Should().NotThrow();

        var md = doc.ToMarkdown();
        md.Should().Contain("# ");
    }

    [Fact]
    public void ToMarkdown_LargeDocument_ShouldRenderAllSections()
    {
        var sections = Enumerable.Range(1, 20).Select(i => new DocumentSection
        {
            Heading = $"Section {i}",
            Content = $"Content for section {i}. This section covers important details.",
            Order = i
        }).ToList();

        var doc = new GeneratedDocument
        {
            Title = "Large Document",
            Sections = sections
        };

        var md = doc.ToMarkdown();
        for (int i = 1; i <= 20; i++)
        {
            md.Should().Contain($"## Section {i}");
            md.Should().Contain($"Content for section {i}");
        }
    }

    [Fact]
    public void ToMarkdown_DuplicateSectionOrders_ShouldStillRender()
    {
        var doc = new GeneratedDocument
        {
            Title = "Test",
            Sections = new List<DocumentSection>
            {
                new() { Heading = "A", Content = "Content A", Order = 1 },
                new() { Heading = "B", Content = "Content B", Order = 1 },
                new() { Heading = "C", Content = "Content C", Order = 2 }
            }
        };

        var act = () => doc.ToMarkdown();
        act.Should().NotThrow();

        var md = doc.ToMarkdown();
        md.Should().Contain("## A");
        md.Should().Contain("## B");
        md.Should().Contain("## C");
    }
}
