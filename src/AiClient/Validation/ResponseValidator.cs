namespace DocumentationGeneratorAI.AiClient.Validation;

using DocumentationGeneratorAI.AiClient.Models;

public sealed class ResponseValidator
{
    public (bool IsValid, string? Error) Validate(AiDocumentResponse? response)
    {
        if (response is null)
            return (false, "Response is null.");

        if (string.IsNullOrWhiteSpace(response.Title))
            return (false, "Response title is empty.");

        if (string.IsNullOrWhiteSpace(response.DocumentType))
            return (false, "Response document type is empty.");

        if (response.Sections is null || response.Sections.Count == 0)
            return (false, "Response contains no sections.");

        foreach (var section in response.Sections)
        {
            if (string.IsNullOrWhiteSpace(section.Heading))
                return (false, $"Section at order {section.Order} has empty heading.");

            if (string.IsNullOrWhiteSpace(section.Content))
                return (false, $"Section '{section.Heading}' has empty content.");
        }

        if (response.Metadata is null)
            return (false, "Response metadata is null.");

        return (true, null);
    }
}
