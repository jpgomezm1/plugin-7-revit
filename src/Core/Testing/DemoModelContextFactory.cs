using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.Core.Testing;

public static class DemoModelContextFactory
{
    public static ModelContext Create()
    {
        return new ModelContext
        {
            ProjectInfo = new ProjectInfoDto
            {
                Name = "Centro Empresarial Procol",
                Number = "PRJ-2025-042",
                ClientName = "Grupo Inversiones del Pacífico S.A.S.",
                BuildingName = "Torre Norte",
                Author = "Ing. Carlos Mendoza",
                IssueDate = "2025-03-15",
                Status = "Detailed Design",
                Address = "Calle 5 #38-25, Cali, Valle del Cauca"
            },
            Levels = new List<LevelDto>
            {
                new() { Name = "Basement", Elevation = -3.00 },
                new() { Name = "Ground Floor", Elevation = 0.00 },
                new() { Name = "Level 1", Elevation = 4.20 },
                new() { Name = "Level 2", Elevation = 8.40 },
                new() { Name = "Level 3", Elevation = 12.60 },
                new() { Name = "Roof", Elevation = 16.80 }
            },
            CategoryCounts = new List<CategoryCountDto>
            {
                new() { CategoryName = "Walls", Count = 187 },
                new() { CategoryName = "Floors", Count = 12 },
                new() { CategoryName = "Roofs", Count = 2 },
                new() { CategoryName = "Doors", Count = 64 },
                new() { CategoryName = "Windows", Count = 48 },
                new() { CategoryName = "Structural Columns", Count = 32 },
                new() { CategoryName = "Structural Framing", Count = 96 },
                new() { CategoryName = "Structural Foundation", Count = 16 },
                new() { CategoryName = "Ceilings", Count = 18 },
                new() { CategoryName = "Stairs", Count = 4 },
                new() { CategoryName = "Curtain Wall Panels", Count = 24 },
                new() { CategoryName = "Railings", Count = 8 },
                new() { CategoryName = "Furniture", Count = 42 },
                new() { CategoryName = "Plumbing Fixtures", Count = 28 },
                new() { CategoryName = "Mechanical Equipment", Count = 6 },
                new() { CategoryName = "Electrical Equipment", Count = 12 },
                new() { CategoryName = "Duct Curves", Count = 134 },
                new() { CategoryName = "Pipe Curves", Count = 89 }
            },
            Materials = new List<MaterialDto>
            {
                new() { Name = "Concrete - Cast-in-Place", MaterialClass = "Concrete" },
                new() { Name = "Concrete - Precast", MaterialClass = "Concrete" },
                new() { Name = "Steel - ASTM A992", MaterialClass = "Metal" },
                new() { Name = "Brick - Clay Common", MaterialClass = "Masonry" },
                new() { Name = "Glass - Clear Float 6mm", MaterialClass = "Glass" },
                new() { Name = "Glass - Low-E Insulated", MaterialClass = "Glass" },
                new() { Name = "Gypsum Board - 12.5mm", MaterialClass = "Finish" },
                new() { Name = "Ceramic Tile - Floor", MaterialClass = "Finish" },
                new() { Name = "Aluminum - Anodized", MaterialClass = "Metal" },
                new() { Name = "Waterproofing Membrane", MaterialClass = "Miscellaneous" },
                new() { Name = "Insulation - Mineral Wool", MaterialClass = "Insulation" },
                new() { Name = "Paint - Interior Latex", MaterialClass = "Finish" }
            },
            MepSystems = new List<MepSystemDto>
            {
                new() { Name = "Supply Air System 1", SystemType = "Mechanical", ElementCount = 45 },
                new() { Name = "Return Air System 1", SystemType = "Mechanical", ElementCount = 38 },
                new() { Name = "Exhaust Air System", SystemType = "Mechanical", ElementCount = 22 },
                new() { Name = "Domestic Cold Water", SystemType = "Piping", ElementCount = 34 },
                new() { Name = "Domestic Hot Water", SystemType = "Piping", ElementCount = 28 },
                new() { Name = "Sanitary Drainage", SystemType = "Piping", ElementCount = 27 },
                new() { Name = "Power Distribution Panel A", SystemType = "Electrical", ElementCount = 18 },
                new() { Name = "Lighting Circuit - Floor 1", SystemType = "Electrical", ElementCount = 14 },
                new() { Name = "Lighting Circuit - Floor 2", SystemType = "Electrical", ElementCount = 12 }
            },
            Rooms = new List<RoomDto>
            {
                new() { Name = "Lobby", Number = "001", Level = "Ground Floor", Area = 85.50, Volume = 358.10 },
                new() { Name = "Reception", Number = "002", Level = "Ground Floor", Area = 32.00, Volume = 134.40 },
                new() { Name = "Meeting Room A", Number = "003", Level = "Ground Floor", Area = 28.50, Volume = 119.70 },
                new() { Name = "Office 101", Number = "101", Level = "Level 1", Area = 45.00, Volume = 189.00 },
                new() { Name = "Office 102", Number = "102", Level = "Level 1", Area = 38.20, Volume = 160.44 },
                new() { Name = "Office 103", Number = "103", Level = "Level 1", Area = 42.00, Volume = 176.40 },
                new() { Name = "Conference Room", Number = "104", Level = "Level 1", Area = 55.00, Volume = 231.00 },
                new() { Name = "Restroom M - L1", Number = "105", Level = "Level 1", Area = 18.50, Volume = 77.70 },
                new() { Name = "Restroom F - L1", Number = "106", Level = "Level 1", Area = 18.50, Volume = 77.70 },
                new() { Name = "Corridor - L1", Number = "107", Level = "Level 1", Area = 34.00, Volume = 142.80 },
                new() { Name = "Office 201", Number = "201", Level = "Level 2", Area = 52.30, Volume = 219.66 },
                new() { Name = "Office 202", Number = "202", Level = "Level 2", Area = 48.00, Volume = 201.60 },
                new() { Name = "Open Plan Office", Number = "203", Level = "Level 2", Area = 120.00, Volume = 504.00 },
                new() { Name = "Server Room", Number = "204", Level = "Level 2", Area = 15.00, Volume = 63.00 },
                new() { Name = "Break Room", Number = "205", Level = "Level 2", Area = 22.00, Volume = 92.40 },
                new() { Name = "Director Office", Number = "301", Level = "Level 3", Area = 65.00, Volume = 273.00 },
                new() { Name = "Board Room", Number = "302", Level = "Level 3", Area = 48.00, Volume = 201.60 },
                new() { Name = "Archive", Number = "B01", Level = "Basement", Area = 95.00, Volume = 285.00 },
                new() { Name = "Parking", Number = "B02", Level = "Basement", Area = 450.00, Volume = 1350.00 },
                new() { Name = "Mechanical Room", Number = "B03", Level = "Basement", Area = 35.00, Volume = 105.00 }
            },
            QuantitySummary = new QuantitySummaryDto
            {
                TotalFloorArea = 1850.75,
                TotalWallArea = 4235.20,
                TotalRoomArea = 1347.50,
                TotalRoomVolume = 5062.50,
                LevelCount = 6,
                RoomCount = 20
            },
            Warnings = new List<ExtractionWarning>(),
            ExtractionTimestamp = DateTime.UtcNow
        };
    }

    public static ModelContext CreateMinimal()
    {
        var context = new ModelContext
        {
            ProjectInfo = new ProjectInfoDto
            {
                Name = "Minimal Test Project",
                Number = "MIN-001"
            },
            Levels = new List<LevelDto>
            {
                new() { Name = "Ground Floor", Elevation = 0.00 }
            },
            CategoryCounts = new List<CategoryCountDto>
            {
                new() { CategoryName = "Walls", Count = 4 }
            },
            ExtractionTimestamp = DateTime.UtcNow
        };

        // Manually add warnings for missing data
        context.Warnings = new List<ExtractionWarning>
        {
            new("NO_ROOMS", "No rooms with area > 0 found in the model."),
            new("NO_FLOOR_AREA", "Total floor area is zero. No floor elements found or areas not computed."),
            new("NO_MEP_SYSTEMS", "No MEP systems found in the model."),
            new("NO_MATERIALS", "No materials found in the model.")
        };

        return context;
    }
}
