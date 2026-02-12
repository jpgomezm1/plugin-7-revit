using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.Core.Testing;

public sealed class DemoAiDocumentGenerator : IAiDocumentGenerator
{
    public async Task<GeneratedDocument> GenerateAsync(
        ModelContext context,
        DocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        // Simulate network delay
        await Task.Delay(1500, cancellationToken);

        var projectName = context.ProjectInfo.Name;
        if (string.IsNullOrWhiteSpace(projectName))
            projectName = "Untitled Project";

        var docTypeName = request.DocumentType.ToString();
        var sections = new List<DocumentSection>();
        int order = 1;

        // Project Overview
        sections.Add(new DocumentSection
        {
            Heading = "Project Overview",
            Content = $"**Project Name:** {projectName}\n" +
                      $"**Project Number:** {context.ProjectInfo.Number}\n" +
                      $"**Client:** {context.ProjectInfo.ClientName}\n" +
                      $"**Building:** {context.ProjectInfo.BuildingName}\n" +
                      $"**Address:** {context.ProjectInfo.Address}\n" +
                      $"**Status:** {context.ProjectInfo.Status}\n" +
                      $"**Author:** {context.ProjectInfo.Author}\n" +
                      $"**Issue Date:** {context.ProjectInfo.IssueDate}",
            Order = order++
        });

        // Building Description
        var levelsList = string.Join("\n", context.Levels.Select(l =>
            $"- {l.Name}: Elevation {l.Elevation:F2}"));
        sections.Add(new DocumentSection
        {
            Heading = "Building Description",
            Content = $"The building comprises {context.Levels.Count} levels:\n\n{levelsList}\n\n" +
                      $"Total floor area: {context.QuantitySummary.TotalFloorArea:F2} sq units.\n" +
                      $"Total wall area: {context.QuantitySummary.TotalWallArea:F2} sq units.",
            Order = order++
        });

        // Element Inventory
        var elementTable = "| Category | Count |\n|---|---|\n" +
            string.Join("\n", context.CategoryCounts.Select(c =>
                $"| {c.CategoryName} | {c.Count} |"));
        sections.Add(new DocumentSection
        {
            Heading = "Element Inventory",
            Content = elementTable,
            Order = order++
        });

        // Room Schedule
        if (context.Rooms.Count > 0)
        {
            var roomTable = "| Number | Name | Level | Area | Volume |\n|---|---|---|---|---|\n" +
                string.Join("\n", context.Rooms.Select(r =>
                    $"| {r.Number} | {r.Name} | {r.Level} | {r.Area:F2} | {r.Volume:F2} |"));
            sections.Add(new DocumentSection
            {
                Heading = "Room Schedule",
                Content = $"The model contains {context.Rooms.Count} rooms.\n\n{roomTable}",
                Order = order++
            });
        }

        // Materials
        if (context.Materials.Count > 0)
        {
            var matTable = "| Material | Class |\n|---|---|\n" +
                string.Join("\n", context.Materials.Select(m =>
                    $"| {m.Name} | {m.MaterialClass} |"));
            sections.Add(new DocumentSection
            {
                Heading = "Materials Summary",
                Content = matTable,
                Order = order++
            });
        }

        // MEP Systems
        if (context.MepSystems.Count > 0)
        {
            var mepTable = "| System | Type | Elements |\n|---|---|---|\n" +
                string.Join("\n", context.MepSystems.Select(s =>
                    $"| {s.Name} | {s.SystemType} | {s.ElementCount} |"));
            sections.Add(new DocumentSection
            {
                Heading = "MEP Systems",
                Content = mepTable,
                Order = order++
            });
        }

        // Quantities
        if (request.IncludeQuantitiesSummary)
        {
            sections.Add(new DocumentSection
            {
                Heading = "Quantities Summary",
                Content = $"| Metric | Value |\n|---|---|\n" +
                          $"| Total Floor Area | {context.QuantitySummary.TotalFloorArea:F2} |\n" +
                          $"| Total Wall Area | {context.QuantitySummary.TotalWallArea:F2} |\n" +
                          $"| Total Room Area | {context.QuantitySummary.TotalRoomArea:F2} |\n" +
                          $"| Total Room Volume | {context.QuantitySummary.TotalRoomVolume:F2} |\n" +
                          $"| Level Count | {context.QuantitySummary.LevelCount} |\n" +
                          $"| Room Count | {context.QuantitySummary.RoomCount} |",
                Order = order++
            });
        }

        // Warnings / Notes
        var warningLines = context.Warnings.Count > 0
            ? string.Join("\n", context.Warnings.Select(w => $"- **{w.Code}:** {w.Message}"))
            : "No warnings. Model data appears complete.";
        sections.Add(new DocumentSection
        {
            Heading = "Notes and Observations",
            Content = warningLines,
            Order = order++
        });

        return new GeneratedDocument
        {
            Title = $"{projectName} — {docTypeName} [DEMO]",
            DocumentType = docTypeName,
            GeneratedDate = DateTime.Now.ToString("yyyy-MM-dd"),
            Sections = sections,
            ProjectName = projectName,
            Phase = request.Phase.ToString(),
            Audience = request.Audience.ToString(),
            DetailLevel = request.DetailLevel.ToString(),
            Warnings = context.Warnings.Select(w => w.Message).ToList()
        };
    }
}
