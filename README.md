# Documentation Generator AI

Revit 2025 Add-in (.NET 8) that generates professional construction documentation from BIM model data using OpenAI's Responses API with structured outputs.

## Overview

Construction projects require extensive technical documentation (specs, progress reports, coordination reports, delivery documents) that is currently written manually. This plugin acts as an intelligent translation layer between structured BIM data and professional technical writing.

**Key features:**
- Extracts project data from Revit models (read-only, no transactions)
- Generates 5 document types: Descriptive Report, Technical Specification, Progress Report, Coordination Report, Final Delivery Document
- Uses OpenAI Responses API with strict JSON schema for zero-hallucination output
- WPF modeless window with MVVM pattern
- Editable document preview with Markdown export
- Thread-safe Revit API interaction via ExternalEvent pattern

## Architecture

| Project | Purpose |
|---|---|
| **RevitAddin** | Ribbon UI, WPF window (MVVM), ExternalEvent thread marshaling |
| **RevitModelExtractor** | Revit API data extraction (FilteredElementCollector, parameters) |
| **Core** | Interfaces, DTOs, enums, orchestration pipeline |
| **AiClient** | OpenAI Responses API client, prompt builder, JSON schema validation |
| **DocTemplates** | Markdown templates per document type (embedded resources) |
| **DocExport** | Markdown exporter (MVP), DOCX/PDF stubs |
| **Shared** | Configuration, logging, string/JSON utilities |

**Dependency direction:** `Shared` ← `Core` ← `{RevitModelExtractor, AiClient, DocTemplates, DocExport}` ← `RevitAddin`

## Prerequisites

- Visual Studio 2022 (17.8+) with .NET 8 SDK and ".NET desktop development" workload
- Autodesk Revit 2025 installed
- OpenAI API key

## Setup

### 1. Set Environment Variable

```powershell
[System.Environment]::SetEnvironmentVariable("OPENAI_API_KEY", "sk-your-key", "User")
```

Restart Revit after setting.

### 2. Build

1. Open `DocumentationGeneratorAI.sln` in Visual Studio 2022
2. Verify `HintPath` in `RevitAddin.csproj` and `RevitModelExtractor.csproj` points to your Revit 2025 installation (`C:\Program Files\Autodesk\Revit 2025\`)
3. Build solution (Ctrl+Shift+B)

### 3. Install Add-in

Copy `src/RevitAddin/DocumentationGeneratorAI.addin` to:
```
C:\ProgramData\Autodesk\Revit\Addins\2025\
```

Update the `<Assembly>` path in the `.addin` file to point to your build output DLL.

### 4. Debug

1. Set RevitAddin as startup project
2. Properties → Debug → Executable: `C:\Program Files\Autodesk\Revit 2025\Revit.exe`
3. Set breakpoints, press F5
4. Open a Revit project → Irrelevant tab → Generate Documentation

## Usage

1. Open a Revit project
2. Click **Irrelevant** tab → **Generate Documentation**
3. Configure: Document Type, Project Phase, Target Audience, Detail Level
4. Click **Generate** — model data is extracted, then AI generates the document
5. Review and edit the generated Markdown in the preview panel
6. Click **Export to Markdown** to save

## Running Tests

```powershell
dotnet test tests/Core.Tests
dotnet test tests/AiClient.Tests
```

## Data Privacy

- `store: false` is set in all API requests — OpenAI does not retain your data
- All Revit API access is read-only
- API key is read from environment variables, never stored in code

## Project Phases

| Phase | Status | Features |
|---|---|---|
| **Phase 1 (MVP)** | Current | 5 doc types, model extraction, AI generation, Markdown export |
| **Phase 2** | Planned | DOCX export, settings UI, prompt tuning |
| **Phase 3** | Planned | PDF export, batch generation, localization |
