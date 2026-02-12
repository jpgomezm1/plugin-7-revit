namespace DocumentationGeneratorAI.Core.Models;

public sealed class ModelContext
{
    public ProjectInfoDto ProjectInfo { get; set; } = new();
    public List<LevelDto> Levels { get; set; } = new();
    public List<CategoryCountDto> CategoryCounts { get; set; } = new();
    public List<MaterialDto> Materials { get; set; } = new();
    public List<MepSystemDto> MepSystems { get; set; } = new();
    public List<RoomDto> Rooms { get; set; } = new();
    public QuantitySummaryDto QuantitySummary { get; set; } = new();
    public List<ExtractionWarning> Warnings { get; set; } = new();
    public DateTime ExtractionTimestamp { get; set; } = DateTime.UtcNow;
}
