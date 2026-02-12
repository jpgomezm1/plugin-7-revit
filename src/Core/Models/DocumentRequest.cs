using DocumentationGeneratorAI.Core.Enums;

namespace DocumentationGeneratorAI.Core.Models;

public sealed class DocumentRequest
{
    public required DocumentType DocumentType { get; set; }
    public required ProjectPhase Phase { get; set; }
    public required AudienceType Audience { get; set; }
    public required DetailLevel DetailLevel { get; set; }
    public bool IncludeQuantitiesSummary { get; set; } = true;
    public bool UseCompanyTemplate { get; set; }
}
