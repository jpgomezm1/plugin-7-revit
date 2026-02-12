namespace DocumentationGeneratorAI.Core.Models;

public sealed class GeneratedDocument
{
    public string Title { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string GeneratedDate { get; set; } = string.Empty;
    public List<DocumentSection> Sections { get; set; } = new();
    public string ProjectName { get; set; } = string.Empty;
    public string Phase { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string DetailLevel { get; set; } = string.Empty;
    public List<string> Warnings { get; set; } = new();

    public string ToMarkdown()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"# {Title}");
        sb.AppendLine();
        sb.AppendLine($"**Document Type:** {DocumentType}");
        sb.AppendLine($"**Project:** {ProjectName}");
        sb.AppendLine($"**Phase:** {Phase}");
        sb.AppendLine($"**Audience:** {Audience}");
        sb.AppendLine($"**Detail Level:** {DetailLevel}");
        sb.AppendLine($"**Generated:** {GeneratedDate}");
        sb.AppendLine();

        if (Warnings.Count > 0)
        {
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("> **Warnings:**");
            foreach (var w in Warnings)
                sb.AppendLine($"> - {w}");
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine();

        foreach (var section in Sections.OrderBy(s => s.Order))
        {
            sb.AppendLine($"## {section.Heading}");
            sb.AppendLine();
            sb.AppendLine(section.Content);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
