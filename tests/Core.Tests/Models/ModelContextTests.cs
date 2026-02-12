using DocumentationGeneratorAI.Core.Models;
using FluentAssertions;
using Xunit;

namespace DocumentationGeneratorAI.Core.Tests.Models;

public class ModelContextTests
{
    [Fact]
    public void NewModelContext_ShouldHaveEmptyCollections()
    {
        var context = new ModelContext();

        context.ProjectInfo.Should().NotBeNull();
        context.Levels.Should().NotBeNull().And.BeEmpty();
        context.CategoryCounts.Should().NotBeNull().And.BeEmpty();
        context.Materials.Should().NotBeNull().And.BeEmpty();
        context.MepSystems.Should().NotBeNull().And.BeEmpty();
        context.Rooms.Should().NotBeNull().And.BeEmpty();
        context.Warnings.Should().NotBeNull().And.BeEmpty();
        context.QuantitySummary.Should().NotBeNull();
    }

    [Fact]
    public void NewModelContext_ShouldHaveTimestamp()
    {
        var before = DateTime.UtcNow;
        var context = new ModelContext();
        var after = DateTime.UtcNow;

        context.ExtractionTimestamp.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void GeneratedDocument_ToMarkdown_ShouldContainTitle()
    {
        var doc = new GeneratedDocument
        {
            Title = "Test Document",
            DocumentType = "Descriptive Report",
            GeneratedDate = "2025-01-01",
            ProjectName = "Test Project",
            Phase = "Detailed Design",
            Audience = "Client",
            DetailLevel = "Standard",
            Sections = new List<DocumentSection>
            {
                new() { Heading = "Overview", Content = "This is an overview.", Order = 1 },
                new() { Heading = "Details", Content = "These are details.", Order = 2 }
            },
            Warnings = new List<string> { "Missing data" }
        };

        var markdown = doc.ToMarkdown();

        markdown.Should().Contain("# Test Document");
        markdown.Should().Contain("**Project:** Test Project");
        markdown.Should().Contain("## Overview");
        markdown.Should().Contain("## Details");
        markdown.Should().Contain("This is an overview.");
        markdown.Should().Contain("Missing data");
    }

    [Fact]
    public void GeneratedDocument_ToMarkdown_SectionsOrderedByOrder()
    {
        var doc = new GeneratedDocument
        {
            Title = "Test",
            Sections = new List<DocumentSection>
            {
                new() { Heading = "Second", Content = "B", Order = 2 },
                new() { Heading = "First", Content = "A", Order = 1 },
                new() { Heading = "Third", Content = "C", Order = 3 }
            }
        };

        var markdown = doc.ToMarkdown();
        var firstIdx = markdown.IndexOf("## First");
        var secondIdx = markdown.IndexOf("## Second");
        var thirdIdx = markdown.IndexOf("## Third");

        firstIdx.Should().BeLessThan(secondIdx);
        secondIdx.Should().BeLessThan(thirdIdx);
    }

    [Fact]
    public void GeneratedDocument_ToMarkdown_NoWarnings_ShouldNotContainWarningsSection()
    {
        var doc = new GeneratedDocument
        {
            Title = "Test",
            Warnings = new List<string>()
        };

        var markdown = doc.ToMarkdown();
        markdown.Should().NotContain("> **Warnings:**");
    }

    [Fact]
    public void DocumentRequest_ShouldHaveDefaults()
    {
        var request = new DocumentRequest
        {
            DocumentType = Core.Enums.DocumentType.DescriptiveReport,
            Phase = Core.Enums.ProjectPhase.DetailedDesign,
            Audience = Core.Enums.AudienceType.Client,
            DetailLevel = Core.Enums.DetailLevel.Standard
        };

        request.IncludeQuantitiesSummary.Should().BeTrue();
        request.UseCompanyTemplate.Should().BeFalse();
    }

    [Fact]
    public void ExtractionWarning_Constructor_ShouldSetProperties()
    {
        var warning = new ExtractionWarning("CODE_1", "Test message");

        warning.Code.Should().Be("CODE_1");
        warning.Message.Should().Be("Test message");
    }
}
