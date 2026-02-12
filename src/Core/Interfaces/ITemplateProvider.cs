namespace DocumentationGeneratorAI.Core.Interfaces;

using DocumentationGeneratorAI.Core.Enums;

public interface ITemplateProvider
{
    string GetTemplate(DocumentType documentType);
    bool HasTemplate(DocumentType documentType);
}
