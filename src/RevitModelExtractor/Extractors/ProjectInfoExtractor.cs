using Autodesk.Revit.DB;
using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.RevitModelExtractor.Extractors;

internal static class ProjectInfoExtractor
{
    public static ProjectInfoDto Extract(Document doc)
    {
        var info = doc.ProjectInformation;
        return new ProjectInfoDto
        {
            Name = info.Name ?? string.Empty,
            Number = info.Number ?? string.Empty,
            ClientName = GetParameterValue(info, BuiltInParameter.CLIENT_NAME),
            BuildingName = info.BuildingName ?? string.Empty,
            Author = GetParameterValue(info, BuiltInParameter.PROJECT_AUTHOR),
            IssueDate = GetParameterValue(info, BuiltInParameter.PROJECT_ISSUE_DATE),
            Status = GetParameterValue(info, BuiltInParameter.PROJECT_STATUS),
            Address = GetParameterValue(info, BuiltInParameter.PROJECT_ADDRESS)
        };
    }

    private static string GetParameterValue(ProjectInformation info, BuiltInParameter param)
    {
        var p = info.get_Parameter(param);
        return p?.AsString() ?? string.Empty;
    }
}
