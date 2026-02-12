using System.Text;
using System.Text.Json;
using DocumentationGeneratorAI.Core.Enums;
using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.AiClient;

public sealed class PromptBuilder
{
    public string BuildSystemPrompt()
    {
        return """
            You are an expert construction documentation writer with deep knowledge of building construction, architecture, structural engineering, and MEP systems.

            YOUR TASK:
            Generate a professional construction document based EXCLUSIVELY on the Revit model data provided in the user message. The model data is a JSON object called ModelContext.

            STRICT RULES:
            1. Use ONLY facts present in the ModelContext JSON. Do not invent, assume, or hallucinate any data.
            2. If specific information is not available in the ModelContext, explicitly state: "Not available in the model data."
            3. Do not reference external standards, codes, or specifications unless directly mentioned in the model data.
            4. All quantities, counts, and measurements must come directly from the ModelContext. Do not perform calculations beyond simple aggregation of provided values.
            5. Use professional, clear language appropriate for the specified target audience.
            6. Structure the document with clear headings and sections as specified by the document type.
            7. Include relevant data from every section of the ModelContext that is applicable to the requested document type.
            8. If the ModelContext contains warnings, incorporate them as caveats in the relevant sections.
            9. Format all content in Markdown within the JSON string fields.
            10. Respond ONLY with the JSON structure matching the provided schema. Do not include any text outside the JSON.

            FORMATTING:
            - Use ## for main headings within section content.
            - Use bullet points for lists.
            - Use Markdown tables for quantitative data.
            - "Short" detail: 2-3 sentences per section.
            - "Standard" detail: 1-2 paragraphs per section.
            - "Extended" detail: Multiple paragraphs, tables, thorough analysis per section.
            """;
    }

    public string BuildUserPrompt(ModelContext context, DocumentRequest request)
    {
        var sb = new StringBuilder();

        var docTypeName = FormatDocumentType(request.DocumentType);
        var phaseName = FormatPhase(request.Phase);
        var audienceName = FormatAudience(request.Audience);
        var detailName = request.DetailLevel.ToString();

        sb.AppendLine($"Generate a {docTypeName} document for the following Revit model.");
        sb.AppendLine();
        sb.AppendLine("Configuration:");
        sb.AppendLine($"- Document Type: {docTypeName}");
        sb.AppendLine($"- Project Phase: {phaseName}");
        sb.AppendLine($"- Target Audience: {audienceName}");
        sb.AppendLine($"- Detail Level: {detailName}");
        sb.AppendLine($"- Include Quantities Summary: {(request.IncludeQuantitiesSummary ? "Yes" : "No")}");
        sb.AppendLine($"- Apply Company Template Style: {(request.UseCompanyTemplate ? "Yes" : "No")}");
        sb.AppendLine();

        sb.AppendLine(GetDocumentTypeInstructions(request.DocumentType));
        sb.AppendLine();

        var contextJson = JsonSerializer.Serialize(context, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        sb.AppendLine("Revit Model Data (ModelContext):");
        sb.AppendLine("```json");
        sb.AppendLine(contextJson);
        sb.AppendLine("```");

        return sb.ToString();
    }

    private static string FormatDocumentType(DocumentType type) => type switch
    {
        DocumentType.DescriptiveReport => "Descriptive Report",
        DocumentType.TechnicalSpecification => "Technical Specification",
        DocumentType.ProgressReport => "Progress Report",
        DocumentType.CoordinationReport => "Coordination Report",
        DocumentType.FinalDeliveryDocument => "Final Delivery Document",
        _ => type.ToString()
    };

    private static string FormatPhase(ProjectPhase phase) => phase switch
    {
        ProjectPhase.ConceptualDesign => "Conceptual Design",
        ProjectPhase.DetailedDesign => "Detailed Design",
        ProjectPhase.Construction => "Construction",
        ProjectPhase.Handover => "Handover",
        _ => phase.ToString()
    };

    private static string FormatAudience(AudienceType audience) => audience switch
    {
        AudienceType.Client => "Client",
        AudienceType.Supervision => "Supervision / Inspector",
        AudienceType.Management => "Management",
        AudienceType.PublicAuthority => "Public Authority",
        _ => audience.ToString()
    };

    private static string GetDocumentTypeInstructions(DocumentType type) => type switch
    {
        DocumentType.DescriptiveReport => """
            Document-Specific Instructions (Descriptive Report):
            Include these sections in order:
            1. Project Overview (name, number, client, building, address, status)
            2. Building Description (levels, overall dimensions from quantities)
            3. Architectural Elements (walls, floors, roofs, doors, windows, columns - counts and descriptions)
            4. Structural Systems (columns, framing, foundations - counts)
            5. MEP Systems Overview (mechanical, piping, electrical systems)
            6. Space Summary (room schedule with areas)
            7. Materials Summary (material types and classes)
            8. Quantities Summary (if requested - aggregate areas, volumes, counts)
            9. Notes and Observations (warnings, missing data caveats)
            """,
        DocumentType.TechnicalSpecification => """
            Document-Specific Instructions (Technical Specification):
            Include these sections in order:
            1. General Information (project info, scope)
            2. Structural Systems (columns, beams, framing, foundations - types and counts)
            3. Architectural Elements (walls, floors, roofs, ceilings, doors, windows - types and counts)
            4. MEP Overview (all mechanical, piping, electrical systems with element counts)
            5. Materials Specification (all materials with classes)
            6. Room Schedule (complete room list with areas and volumes)
            7. Quantities Summary (all aggregate measurements)
            8. Technical Notes (warnings, data gaps, assumptions)
            """,
        DocumentType.ProgressReport => """
            Document-Specific Instructions (Progress Report):
            Include these sections in order:
            1. Executive Summary (project status overview)
            2. Project Information (name, number, client, current status)
            3. Model Status (element counts by category, completeness indicators)
            4. Space Planning Status (rooms defined, areas calculated)
            5. Systems Status (MEP systems defined)
            6. Material Definitions (materials assigned)
            7. Quantities Overview (key metrics)
            8. Issues and Warnings (extraction warnings, missing data)
            9. Recommendations (based on warnings and gaps)
            """,
        DocumentType.CoordinationReport => """
            Document-Specific Instructions (Coordination Report):
            Include these sections in order:
            1. Project Summary (key project details)
            2. Model Overview (total elements, levels, rooms)
            3. Architectural Status (element counts for architectural categories)
            4. Structural Status (element counts for structural categories)
            5. MEP Coordination (all MEP systems, element counts, system types)
            6. Space Coordination (room list, area verification)
            7. Material Coordination (materials in use)
            8. Coordination Issues (warnings, potential conflicts based on data)
            9. Action Items (recommendations based on gaps)
            """,
        DocumentType.FinalDeliveryDocument => """
            Document-Specific Instructions (Final Delivery Document):
            Include these sections in order:
            1. Project Identification (complete project info)
            2. Building Description (levels, overall building data)
            3. Complete Element Inventory (all categories with counts)
            4. Room Schedule (complete room list with all data)
            5. MEP Systems Inventory (all systems with details)
            6. Materials Registry (all materials)
            7. Final Quantities (complete quantities summary)
            8. Model Quality Notes (all warnings, data completeness assessment)
            9. Delivery Certification (statement of model contents and extraction date)
            """,
        _ => string.Empty
    };
}
