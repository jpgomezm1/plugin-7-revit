namespace DocumentationGeneratorAI.Core.Models;

public sealed class RoomDto
{
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public double Area { get; set; }
    public double Volume { get; set; }
}
