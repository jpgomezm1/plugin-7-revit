namespace DocumentationGeneratorAI.Core.Models;

public sealed class QuantitySummaryDto
{
    public double TotalFloorArea { get; set; }
    public double TotalWallArea { get; set; }
    public double TotalRoomArea { get; set; }
    public double TotalRoomVolume { get; set; }
    public int LevelCount { get; set; }
    public int RoomCount { get; set; }
}
