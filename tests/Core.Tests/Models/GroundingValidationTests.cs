using System.Text.Json;
using DocumentationGeneratorAI.Core.Models;
using FluentAssertions;
using Xunit;

namespace DocumentationGeneratorAI.Core.Tests.Models;

public class GroundingValidationTests
{
    [Fact]
    public void ModelContext_SerializationRoundtrip_ShouldPreserveAllData()
    {
        var original = CreateFullContext();

        var json = JsonSerializer.Serialize(original, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        var deserialized = JsonSerializer.Deserialize<ModelContext>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        deserialized.Should().NotBeNull();
        deserialized!.ProjectInfo.Name.Should().Be(original.ProjectInfo.Name);
        deserialized.ProjectInfo.Number.Should().Be(original.ProjectInfo.Number);
        deserialized.ProjectInfo.ClientName.Should().Be(original.ProjectInfo.ClientName);
        deserialized.ProjectInfo.Address.Should().Be(original.ProjectInfo.Address);
        deserialized.Levels.Should().HaveCount(original.Levels.Count);
        deserialized.CategoryCounts.Should().HaveCount(original.CategoryCounts.Count);
        deserialized.Materials.Should().HaveCount(original.Materials.Count);
        deserialized.MepSystems.Should().HaveCount(original.MepSystems.Count);
        deserialized.Rooms.Should().HaveCount(original.Rooms.Count);
        deserialized.Warnings.Should().HaveCount(original.Warnings.Count);
    }

    [Fact]
    public void ModelContext_SerializationRoundtrip_ShouldPreserveRoomDetails()
    {
        var original = CreateFullContext();

        var json = JsonSerializer.Serialize(original, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var deserialized = JsonSerializer.Deserialize<ModelContext>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        for (int i = 0; i < original.Rooms.Count; i++)
        {
            deserialized!.Rooms[i].Name.Should().Be(original.Rooms[i].Name);
            deserialized.Rooms[i].Number.Should().Be(original.Rooms[i].Number);
            deserialized.Rooms[i].Area.Should().Be(original.Rooms[i].Area);
            deserialized.Rooms[i].Volume.Should().Be(original.Rooms[i].Volume);
            deserialized.Rooms[i].Level.Should().Be(original.Rooms[i].Level);
        }
    }

    [Fact]
    public void ModelContext_SerializationRoundtrip_ShouldPreserveQuantities()
    {
        var original = CreateFullContext();

        var json = JsonSerializer.Serialize(original, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var deserialized = JsonSerializer.Deserialize<ModelContext>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        deserialized!.QuantitySummary.TotalFloorArea.Should().Be(original.QuantitySummary.TotalFloorArea);
        deserialized.QuantitySummary.TotalWallArea.Should().Be(original.QuantitySummary.TotalWallArea);
        deserialized.QuantitySummary.TotalRoomArea.Should().Be(original.QuantitySummary.TotalRoomArea);
        deserialized.QuantitySummary.TotalRoomVolume.Should().Be(original.QuantitySummary.TotalRoomVolume);
        deserialized.QuantitySummary.LevelCount.Should().Be(original.QuantitySummary.LevelCount);
        deserialized.QuantitySummary.RoomCount.Should().Be(original.QuantitySummary.RoomCount);
    }

    [Fact]
    public void ModelContext_SpecialCharacters_ShouldSerializeCorrectly()
    {
        var context = new ModelContext
        {
            ProjectInfo = new ProjectInfoDto
            {
                Name = "Test \"Project\" <#1> & Co.",
                Address = "Calle 5 #38-25, Cali"
            },
            Rooms = new List<RoomDto>
            {
                new() { Name = "Baño Principal — Piso 1", Number = "101", Level = "Level 1", Area = 12.5, Volume = 37.5 }
            }
        };

        var json = JsonSerializer.Serialize(context, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var deserialized = JsonSerializer.Deserialize<ModelContext>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        deserialized!.ProjectInfo.Name.Should().Be("Test \"Project\" <#1> & Co.");
        deserialized.ProjectInfo.Address.Should().Be("Calle 5 #38-25, Cali");
        deserialized.Rooms[0].Name.Should().Be("Baño Principal — Piso 1");
    }

    [Fact]
    public void ModelContext_EmptyCollections_ShouldSerializeCorrectly()
    {
        var context = new ModelContext();

        var json = JsonSerializer.Serialize(context, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        json.Should().Contain("\"levels\":[]");
        json.Should().Contain("\"rooms\":[]");
        json.Should().Contain("\"materials\":[]");

        var deserialized = JsonSerializer.Deserialize<ModelContext>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        deserialized!.Levels.Should().BeEmpty();
        deserialized.Rooms.Should().BeEmpty();
        deserialized.Materials.Should().BeEmpty();
    }

    [Fact]
    public void ModelContext_JsonSize_ShouldBeUnderLimit()
    {
        var context = CreateFullContext();
        var json = JsonSerializer.Serialize(context, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // 30KB limit from PluginConstants.MaxJsonSizeBytes
        System.Text.Encoding.UTF8.GetByteCount(json).Should().BeLessThan(30 * 1024);
    }

    private static ModelContext CreateFullContext()
    {
        return new ModelContext
        {
            ProjectInfo = new ProjectInfoDto
            {
                Name = "Test Project Alpha",
                Number = "TP-001",
                ClientName = "Acme Corp",
                BuildingName = "Main Building",
                Author = "Test Author",
                IssueDate = "2025-01-15",
                Status = "Detailed Design",
                Address = "123 Test Street"
            },
            Levels = new List<LevelDto>
            {
                new() { Name = "Ground", Elevation = 0.0 },
                new() { Name = "Level 1", Elevation = 3.5 },
                new() { Name = "Roof", Elevation = 7.0 }
            },
            CategoryCounts = new List<CategoryCountDto>
            {
                new() { CategoryName = "Walls", Count = 42 },
                new() { CategoryName = "Doors", Count = 12 },
                new() { CategoryName = "Windows", Count = 8 }
            },
            Materials = new List<MaterialDto>
            {
                new() { Name = "Concrete", MaterialClass = "Concrete" },
                new() { Name = "Steel", MaterialClass = "Metal" }
            },
            MepSystems = new List<MepSystemDto>
            {
                new() { Name = "Supply Air", SystemType = "Mechanical", ElementCount = 15 }
            },
            Rooms = new List<RoomDto>
            {
                new() { Name = "Lobby", Number = "001", Level = "Ground", Area = 50.0, Volume = 175.0 },
                new() { Name = "Office", Number = "101", Level = "Level 1", Area = 30.0, Volume = 105.0 }
            },
            QuantitySummary = new QuantitySummaryDto
            {
                TotalFloorArea = 250.0,
                TotalWallArea = 600.0,
                TotalRoomArea = 80.0,
                TotalRoomVolume = 280.0,
                LevelCount = 3,
                RoomCount = 2
            },
            Warnings = new List<ExtractionWarning>
            {
                new("TEST_WARNING", "This is a test warning.")
            }
        };
    }
}
