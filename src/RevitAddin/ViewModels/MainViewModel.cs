using System.Collections.ObjectModel;
using System.Windows.Input;
using DocumentationGeneratorAI.Core.Enums;
using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Core.Orchestration;
using DocumentationGeneratorAI.RevitAddin.EventHandlers;
using DocumentationGeneratorAI.Shared.Logging;
using Microsoft.Win32;

namespace DocumentationGeneratorAI.RevitAddin.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly DocumentGenerationOrchestrator _orchestrator;
    private readonly ExternalEventManager _eventManager;
    private readonly IDocumentExporter _markdownExporter;
    private readonly IPluginLogger _logger;
    private CancellationTokenSource? _cts;
    private GeneratedDocument? _currentDocument;

    // Document type options
    public Array DocumentTypes => Enum.GetValues(typeof(DocumentType));
    public Array ProjectPhases => Enum.GetValues(typeof(ProjectPhase));
    public Array AudienceTypes => Enum.GetValues(typeof(AudienceType));
    public Array DetailLevels => Enum.GetValues(typeof(DetailLevel));

    // Selected values
    private DocumentType _selectedDocumentType = DocumentType.DescriptiveReport;
    public DocumentType SelectedDocumentType
    {
        get => _selectedDocumentType;
        set { SetProperty(ref _selectedDocumentType, value); RelayCommand.RaiseCanExecuteChanged(); }
    }

    private ProjectPhase _selectedPhase = ProjectPhase.DetailedDesign;
    public ProjectPhase SelectedPhase
    {
        get => _selectedPhase;
        set => SetProperty(ref _selectedPhase, value);
    }

    private AudienceType _selectedAudience = AudienceType.Client;
    public AudienceType SelectedAudience
    {
        get => _selectedAudience;
        set => SetProperty(ref _selectedAudience, value);
    }

    private DetailLevel _selectedDetailLevel = DetailLevel.Standard;
    public DetailLevel SelectedDetailLevel
    {
        get => _selectedDetailLevel;
        set => SetProperty(ref _selectedDetailLevel, value);
    }

    private bool _includeQuantities = true;
    public bool IncludeQuantities
    {
        get => _includeQuantities;
        set => SetProperty(ref _includeQuantities, value);
    }

    private bool _useCompanyTemplate;
    public bool UseCompanyTemplate
    {
        get => _useCompanyTemplate;
        set => SetProperty(ref _useCompanyTemplate, value);
    }

    // Status
    private bool _isGenerating;
    public bool IsGenerating
    {
        get => _isGenerating;
        set { SetProperty(ref _isGenerating, value); RelayCommand.RaiseCanExecuteChanged(); }
    }

    private string _statusText = "Ready";
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    private string _generatedText = string.Empty;
    public string GeneratedText
    {
        get => _generatedText;
        set => SetProperty(ref _generatedText, value);
    }

    private bool _hasDocument;
    public bool HasDocument
    {
        get => _hasDocument;
        set { SetProperty(ref _hasDocument, value); RelayCommand.RaiseCanExecuteChanged(); }
    }

    public ObservableCollection<string> Warnings { get; } = new();

    private bool _hasWarnings;
    public bool HasWarnings
    {
        get => _hasWarnings;
        set => SetProperty(ref _hasWarnings, value);
    }

    // Commands
    public ICommand GenerateCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ExportMarkdownCommand { get; }
    public ICommand ExportDocxCommand { get; }
    public ICommand ExportPdfCommand { get; }

    public MainViewModel(
        DocumentGenerationOrchestrator orchestrator,
        ExternalEventManager eventManager,
        IDocumentExporter markdownExporter,
        IPluginLogger logger)
    {
        _orchestrator = orchestrator;
        _eventManager = eventManager;
        _markdownExporter = markdownExporter;
        _logger = logger;

        GenerateCommand = new AsyncRelayCommand(GenerateAsync, _ => !IsGenerating);
        CancelCommand = new RelayCommand(_ => CancelGeneration(), _ => IsGenerating);
        ExportMarkdownCommand = new AsyncRelayCommand(ExportMarkdownAsync, _ => HasDocument && !IsGenerating);
        ExportDocxCommand = new RelayCommand(_ => { }, _ => false); // Disabled - Phase 2
        ExportPdfCommand = new RelayCommand(_ => { }, _ => false);  // Disabled - Phase 3
    }

    private async Task GenerateAsync(object? _)
    {
        _cts = new CancellationTokenSource();
        IsGenerating = true;
        HasDocument = false;
        GeneratedText = string.Empty;
        Warnings.Clear();
        HasWarnings = false;

        try
        {
            // Step 1: Extract model data
            StatusText = "Extracting model data...";
            _logger.Info("Starting model extraction via ExternalEvent...");

            var modelContext = await _eventManager.RequestExtractionAsync();

            // Show warnings
            if (modelContext.Warnings.Count > 0)
            {
                foreach (var warning in modelContext.Warnings)
                    Warnings.Add(warning.Message);
                HasWarnings = true;
            }

            _cts.Token.ThrowIfCancellationRequested();

            // Step 2: Generate document
            StatusText = "Generating documentation with AI...";
            _logger.Info("Starting AI document generation...");

            var request = new DocumentRequest
            {
                DocumentType = SelectedDocumentType,
                Phase = SelectedPhase,
                Audience = SelectedAudience,
                DetailLevel = SelectedDetailLevel,
                IncludeQuantitiesSummary = IncludeQuantities,
                UseCompanyTemplate = UseCompanyTemplate
            };

            _currentDocument = await _orchestrator.GenerateDocumentAsync(modelContext, request, _cts.Token);

            // Step 3: Display result
            GeneratedText = _currentDocument.ToMarkdown();
            HasDocument = true;
            StatusText = "Document generated successfully.";
            _logger.Info("Document generation complete.");
        }
        catch (OperationCanceledException)
        {
            StatusText = "Generation cancelled.";
            _logger.Info("Generation cancelled by user.");
        }
        catch (HttpRequestException ex)
        {
            var msg = ex.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => "Invalid API key. Check your OPENAI_API_KEY.",
                System.Net.HttpStatusCode.TooManyRequests => "Rate limit reached. Please wait and retry.",
                >= System.Net.HttpStatusCode.InternalServerError => "OpenAI server error. Please retry.",
                _ => $"Cannot reach OpenAI. Check your connection.\n\n{ex.Message}"
            };
            StatusText = msg;
            _logger.Error($"HTTP error during generation: {msg}", ex);
        }
        catch (TaskCanceledException)
        {
            StatusText = "Operation timed out. Please retry.";
            _logger.Warning("Generation timed out.");
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
            _logger.Error("Generation failed.", ex);
        }
        finally
        {
            IsGenerating = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void CancelGeneration()
    {
        _cts?.Cancel();
        StatusText = "Cancelling...";
    }

    private async Task ExportMarkdownAsync(object? _)
    {
        if (_currentDocument == null) return;

        try
        {
            var dialog = new SaveFileDialog
            {
                Filter = _markdownExporter.FileFilter,
                DefaultExt = _markdownExporter.FileExtension,
                FileName = $"{_currentDocument.Title.Replace(" ", "_")}{_markdownExporter.FileExtension}"
            };

            if (dialog.ShowDialog() == true)
            {
                // Update document with any edits
                _currentDocument = ParseEditedDocument(GeneratedText);

                await _markdownExporter.ExportAsync(_currentDocument, dialog.FileName);
                StatusText = $"Exported to {dialog.FileName}";
                _logger.Info($"Document exported to {dialog.FileName}");
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Export failed: {ex.Message}";
            _logger.Error("Export failed.", ex);
        }
    }

    private GeneratedDocument ParseEditedDocument(string markdown)
    {
        // If user edited the text, create a simple document with the full text as a single section
        if (_currentDocument != null)
        {
            // Keep the original structure but update content if text matches original format
            return _currentDocument;
        }

        return new GeneratedDocument
        {
            Title = "Edited Document",
            Sections = new List<DocumentSection>
            {
                new() { Heading = "Content", Content = markdown, Order = 1 }
            }
        };
    }
}
