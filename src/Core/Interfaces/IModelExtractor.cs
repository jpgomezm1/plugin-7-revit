namespace DocumentationGeneratorAI.Core.Interfaces;

using DocumentationGeneratorAI.Core.Models;

public interface IModelExtractor
{
    ModelContext Extract(object document);
}
