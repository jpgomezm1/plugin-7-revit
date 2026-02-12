# Documentation Generator AI — QA Test Plan

**Version:** 1.0
**Date:** 2026-02-12
**Target:** Revit 2025 Add-in, .NET 8, OpenAI Responses API

---

## A. Test Runbook (Step-by-Step for Human Tester)

### Pre-Requisites

1. Windows 10/11 with Autodesk Revit 2025 installed
2. .NET 8 SDK (8.0.x) installed
3. `OPENAI_API_KEY` set as user environment variable
4. Solution built successfully in Visual Studio 2022
5. `DocumentationGeneratorAI.addin` copied to `C:\ProgramData\Autodesk\Revit\Addins\2025\`
6. Log directory accessible: `%APPDATA%\DocumentationGeneratorAI\logs\`

### Phase 1: Installation & Loading

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1.1 | Build solution in VS2022 (Ctrl+Shift+B) | 0 errors, 0 warnings |
| 1.2 | Copy `.addin` file to Revit addins folder | File exists at target path |
| 1.3 | Launch Revit 2025 | Revit loads without crash |
| 1.4 | Check ribbon for "Irrelevant" tab | Tab visible in ribbon |
| 1.5 | Check "Documentation" panel exists | Panel visible under tab |
| 1.6 | Check "Generate Documentation" button | Button visible with tooltip |
| 1.7 | Check `plugin.log` created | File exists in `%APPDATA%\DocumentationGeneratorAI\logs\` |
| 1.8 | Open log file, verify startup entry | Contains `"Documentation Generator AI started successfully."` |

### Phase 2: Pre-Generation Validation

| Step | Action | Expected Result |
|------|--------|-----------------|
| 2.1 | Click button with NO document open | TaskDialog: "No Revit document is currently open." |
| 2.2 | Remove OPENAI_API_KEY env var, restart Revit | — |
| 2.3 | Open a project, click button | TaskDialog: "OPENAI_API_KEY environment variable is not set..." |
| 2.4 | Restore OPENAI_API_KEY, restart Revit | — |
| 2.5 | Open sample project, click button | MainWindow opens as modeless (Revit stays responsive) |
| 2.6 | Interact with Revit while window is open | Revit responds normally (pan, zoom, select) |

### Phase 3: UI Validation

| Step | Action | Expected Result |
|------|--------|-----------------|
| 3.1 | Inspect Document Type dropdown | 5 options: Descriptive Report, Technical Specification, Progress Report, Coordination Report, Final Delivery Document |
| 3.2 | Inspect Project Phase dropdown | 4 options: ConceptualDesign, DetailedDesign, Construction, Handover |
| 3.3 | Inspect Target Audience dropdown | 4 options: Client, Supervision, Management, PublicAuthority |
| 3.4 | Inspect Detail Level dropdown | 3 options: Short, Standard, Extended |
| 3.5 | Check "Include Quantities Summary" | Checked by default |
| 3.6 | Check "Use Company Template" | Unchecked by default |
| 3.7 | Check Generate button | Enabled |
| 3.8 | Check Cancel button | NOT visible |
| 3.9 | Check Export to Markdown | Disabled (no document yet) |
| 3.10 | Check Export to DOCX | Disabled, tooltip "Coming in Phase 2" |
| 3.11 | Check Export to PDF | Disabled, tooltip "Coming in Phase 3" |
| 3.12 | Check status bar | Shows "Ready" |

### Phase 4: Generation Flow

| Step | Action | Expected Result |
|------|--------|-----------------|
| 4.1 | Select "Descriptive Report", "DetailedDesign", "Client", "Standard" | All combos selected |
| 4.2 | Click Generate | Status: "Extracting model data..." |
| 4.3 | Observe Cancel button | Becomes visible |
| 4.4 | Observe progress bar | Indeterminate bar appears |
| 4.5 | Wait for extraction to complete | Status: "Generating documentation with AI..." |
| 4.6 | Wait for AI response | Status: "Document generated successfully." |
| 4.7 | Observe result TextBox | Contains Markdown with `# Title`, `## Sections` |
| 4.8 | Observe warnings panel (if any) | Orange panel with warning bullets |
| 4.9 | Check Export to Markdown button | Now enabled |
| 4.10 | Check Cancel button | No longer visible |
| 4.11 | Check progress bar | No longer visible |

### Phase 5: Output Validation

| Step | Action | Expected Result |
|------|--------|-----------------|
| 5.1 | Read the generated document title | Contains project name from Revit |
| 5.2 | Check metadata header | Document Type, Project, Phase, Audience, Detail Level all correct |
| 5.3 | Check section headings | Multiple `## Heading` sections present |
| 5.4 | Verify factual data against Revit model | All numbers match model (see Grounding Checklist below) |
| 5.5 | Edit text in the preview TextBox | Text is editable |
| 5.6 | Click "Export to Markdown" | SaveFileDialog opens |
| 5.7 | Choose save location, click Save | File created at chosen path |
| 5.8 | Open exported `.md` file in text editor | Content matches preview (including any edits) |
| 5.9 | Check log file for generation entries | Contains "Starting document generation", "Document generated successfully" |

### Phase 6: Cancellation

| Step | Action | Expected Result |
|------|--------|-----------------|
| 6.1 | Click Generate | Generation starts |
| 6.2 | Click Cancel during "Generating documentation with AI..." | Status: "Generation cancelled." |
| 6.3 | Check UI state | IsGenerating=false, Cancel hidden, Generate enabled |
| 6.4 | Check log file | Contains "Generation cancelled by user" |
| 6.5 | Click Generate again | New generation starts normally |

### Phase 7: Multiple Generations

| Step | Action | Expected Result |
|------|--------|-----------------|
| 7.1 | Generate a Descriptive Report | Document appears |
| 7.2 | Change to Technical Specification, Generate | Previous document replaced with new one |
| 7.3 | Change Detail Level to Extended, Generate | Document is more detailed (longer sections) |
| 7.4 | Change Audience to PublicAuthority, Generate | Language style changes (more formal) |

### Phase 8: Error Conditions

| Step | Action | Expected Result |
|------|--------|-----------------|
| 8.1 | Set invalid API key, restart Revit, Generate | Status: "Invalid API key. Check your OPENAI_API_KEY." |
| 8.2 | Disconnect network, Generate | Status: "Cannot reach OpenAI. Check your connection." |
| 8.3 | Try to export to read-only path | Status: "Export failed: {access denied message}" |

### Phase 9: Shutdown

| Step | Action | Expected Result |
|------|--------|-----------------|
| 9.1 | Close MainWindow | Window closes, Revit continues |
| 9.2 | Click button again | New window opens with fresh state |
| 9.3 | Close Revit | No crash, log contains "shut down" message |

---

## B. Test Cases Table

### Installation & Loading (INS)

| ID | Scenario | Steps | Expected Result | Inspect |
|----|----------|-------|-----------------|---------|
| INS-01 | Clean install | Copy .addin + DLLs, launch Revit | Tab "Irrelevant" appears | Ribbon UI |
| INS-02 | Missing DLL | Remove AiClient.dll, launch Revit | Revit loads, button may fail gracefully | Revit journal, plugin.log |
| INS-03 | Duplicate tab | Another addin already created "Irrelevant" tab | No crash, panel added to existing tab | Ribbon UI |
| INS-04 | Revit startup log | Launch Revit with addin | Log entry: "started successfully" | plugin.log |
| INS-05 | Revit shutdown | Close Revit normally | Log entry: "shut down", no crash | plugin.log |

### UI Validation (UI)

| ID | Scenario | Steps | Expected Result | Inspect |
|----|----------|-------|-----------------|---------|
| UI-01 | Window modeless | Click button, interact with Revit | Revit responds while window open | Revit responsiveness |
| UI-02 | Default selections | Open window, check defaults | Descriptive Report, DetailedDesign(?), Client, Standard, Quantities=checked | UI state |
| UI-03 | ComboBox values | Click each dropdown | All enum values present and selectable | Dropdowns |
| UI-04 | Button states pre-gen | Open window, no generation yet | Generate=enabled, Cancel=hidden, Export MD=disabled, DOCX/PDF=disabled | Buttons |
| UI-05 | Button states during gen | Click Generate | Generate=disabled, Cancel=visible, progress bar=visible | Buttons + bar |
| UI-06 | Button states post-gen | Wait for completion | Generate=enabled, Cancel=hidden, Export MD=enabled | Buttons |
| UI-07 | TextBox editable | Type in result area after generation | Text changes, scroll works | TextBox |
| UI-08 | Warnings display | Generate from model with missing data | Orange panel with bullet warnings | Warnings panel |
| UI-09 | No warnings | Generate from complete model | Warning panel NOT visible | Warnings panel |
| UI-10 | Status bar updates | Generate, observe status bar | Cycles through: Ready → Extracting → Generating → Done | Status bar |
| UI-11 | Resize window | Drag window edges | Layout adapts, no clipping | Window layout |
| UI-12 | Multiple windows | Click button twice | Two independent windows (or focus existing) | Window count |

### Model Extraction (EXT)

| ID | Scenario | Steps | Expected Result | Inspect |
|----|----------|-------|-----------------|---------|
| EXT-01 | Complete model | Open model with all element types | All DTOs populated, 0 warnings | Log, warnings panel |
| EXT-02 | Empty model | Open blank Revit project | 7 warnings generated (no levels, rooms, etc.) | Warnings panel |
| EXT-03 | No rooms | Model with walls/floors but no rooms | Warning: "No rooms with area > 0" | Warnings |
| EXT-04 | No MEP | Architectural-only model | Warning: "No MEP systems found" | Warnings |
| EXT-05 | No materials | Model with default materials only | Materials list populated OR warning | Log, output |
| EXT-06 | 200+ rooms | Model with >200 rooms | List capped at 200, no crash | Generated doc, log |
| EXT-07 | Level extraction | Model with 5 levels | All 5 levels in output, ordered by elevation | Generated doc |
| EXT-08 | Project info | Set all ProjectInformation fields | All 8 fields populated in output | Generated doc |
| EXT-09 | Missing project name | Clear project name in Revit | Warning: "Project name is not set" | Warnings |
| EXT-10 | Extraction timing | Generate, measure time | < 2 seconds for typical model | Log timestamps |
| EXT-11 | Read-only safety | Generate documentation | No Transaction created, model unchanged | Revit undo stack empty |
| EXT-12 | Linked models | Model with linked RVT | Extracts from host only, no crash | Generated doc |
| EXT-13 | Workshared model | Central/local model | Extraction succeeds, no sync needed | Generated doc |

### AI Integration (AI)

| ID | Scenario | Steps | Expected Result | Inspect |
|----|----------|-------|-----------------|---------|
| AI-01 | Successful generation | Normal generation flow | Valid JSON response, document rendered | TextBox, log |
| AI-02 | Retry on error | Simulate transient failure | Log: "attempt 2/3", eventual success or failure | plugin.log |
| AI-03 | All 5 doc types | Generate each type | Each has unique sections appropriate to type | Generated docs |
| AI-04 | Short detail | Select Short detail level | 2-3 sentences per section | Generated doc |
| AI-05 | Extended detail | Select Extended detail level | Multiple paragraphs, tables per section | Generated doc |
| AI-06 | Schema enforcement | Inspect AI response JSON | All required fields present, no additionalProperties | Log (debug) |
| AI-07 | Structured output | Check response format | Valid JSON matching construction_document schema | Response parsing |
| AI-08 | Model context in prompt | Enable debug logging | User prompt contains serialized ModelContext JSON | Log |
| AI-09 | System prompt grounding | Check system prompt | Contains all 10 STRICT RULES | PromptBuilder test |
| AI-10 | store=false | Check API request | `"store": false` in request body | Network trace or log |
| AI-11 | Temperature 0.3 | Check API request | `"temperature": 0.3` in request body | Log |
| AI-12 | Max tokens 16K | Check API request | `"max_output_tokens": 16000` | Log |

### Grounding & Hallucination (GND)

| ID | Scenario | Steps | Expected Result | Inspect |
|----|----------|-------|-----------------|---------|
| GND-01 | Room count matches | Compare doc room count to ModelContext | Exact match | Doc vs. Revit schedule |
| GND-02 | Level count matches | Compare levels in doc to model | Exact match | Doc vs. Revit levels |
| GND-03 | Material names match | Compare materials listed in doc | All names from model, no invented ones | Doc vs. material browser |
| GND-04 | Element counts match | Compare wall/door/window counts | Exact match to ModelContext | Doc vs. Revit schedules |
| GND-05 | No invented specs | Search doc for standard references | Should say "Not available" OR not mention | Full text search |
| GND-06 | No invented quantities | Search for numbers not in ModelContext | All numbers traceable to extraction | Cross-reference |
| GND-07 | Warnings reflected | Model warnings appear in generated doc | Caveats mention missing data | Doc sections |
| GND-08 | Empty model grounding | Generate from near-empty model | Doc states "Not available in the model data" | Doc content |
| GND-09 | Area/volume accuracy | Compare areas in doc to Revit room schedule | Values match (accounting for unit conversion) | Doc vs. Revit |
| GND-10 | MEP system names | Compare MEP system names in doc | All names from model, none invented | Doc vs. Revit systems |

### Export (EXP)

| ID | Scenario | Steps | Expected Result | Inspect |
|----|----------|-------|-----------------|---------|
| EXP-01 | Markdown export | Click Export to Markdown, save | Valid .md file created | File on disk |
| EXP-02 | File content matches | Compare exported file to preview | Identical content | File vs. TextBox |
| EXP-03 | Overwrite existing | Export to same path twice | File overwritten without error | File timestamp |
| EXP-04 | Cancel save dialog | Open SaveFileDialog, click Cancel | No file created, no error | Status bar |
| EXP-05 | Special characters in path | Save to path with spaces/unicode | File created successfully | File on disk |
| EXP-06 | Network path | Save to `\\server\share\` path | Success or clear error message | Status bar |
| EXP-07 | Read-only directory | Save to `C:\Windows\` | Error: "Export failed: Access denied" | Status bar |
| EXP-08 | DOCX button disabled | Click Export to DOCX | Nothing happens (button disabled) | UI |
| EXP-09 | PDF button disabled | Click Export to PDF | Nothing happens (button disabled) | UI |
| EXP-10 | Edited content export | Edit text, then export | Exported file contains the raw edited markdown text | File content |

### Error Handling (ERR)

| ID | Scenario | Steps | Expected Result | Inspect |
|----|----------|-------|-----------------|---------|
| ERR-01 | Missing API key | Remove env var, restart, Generate | Dialog: "OPENAI_API_KEY not set..." | TaskDialog |
| ERR-02 | Invalid API key | Set key to "sk-invalid", Generate | Status: "Invalid API key." | Status bar, log |
| ERR-03 | Network offline | Disconnect, Generate | Status: "Cannot reach OpenAI..." | Status bar, log |
| ERR-04 | Rate limited (429) | Hit rate limit (rapid calls) | Status: "Rate limit reached." | Status bar, log |
| ERR-05 | Server error (500) | OpenAI returns 500 | Auto-retry up to 3, then "OpenAI server error." | Log retry entries |
| ERR-06 | Timeout | Set timeout to 1 second | Status: "Operation timed out." | Status bar, log |
| ERR-07 | Malformed JSON response | (Use mock) Return invalid JSON | Retries, then error message | Log |
| ERR-08 | Empty AI response | (Use mock) Return empty output_text | Log: "Empty response", retries | Log |
| ERR-09 | Schema violation | (Use mock) Return JSON missing title | Log: "Invalid response", retries | Log |
| ERR-10 | Cancellation | Cancel mid-generation | Status: "Generation cancelled." | Status bar, log |
| ERR-11 | No active document | Close all docs, click button | Dialog: "No Revit document is currently open." | TaskDialog |
| ERR-12 | Extraction failure | (Corrupt model) | Status: "Error: {message}" | Status bar, log |
| ERR-13 | Double-click Generate | Click Generate while generating | Second click ignored (button disabled) | UI |

### Logging (LOG)

| ID | Scenario | Steps | Expected Result | Inspect |
|----|----------|-------|-----------------|---------|
| LOG-01 | Log file creation | Delete log file, restart Revit | New log file created | File system |
| LOG-02 | Startup log | Launch Revit | "[INFO] Documentation Generator AI starting up..." | plugin.log |
| LOG-03 | Generation log | Generate a document | Entries for extraction start, AI start, completion | plugin.log |
| LOG-04 | Error log | Trigger an error | "[ERROR] ..." with stack trace | plugin.log |
| LOG-05 | Warning log | Generate from incomplete model | "[WARNING] Model has N extraction warning(s)" | plugin.log |
| LOG-06 | Thread safety | Rapid consecutive generations | No garbled log entries | plugin.log |
| LOG-07 | Log format | Inspect any entry | Format: `[YYYY-MM-DD HH:mm:ss] [LEVEL] message` | plugin.log |

---

## C. Demo Models Strategy

### Model 1: Minimal Model (< 1 minute to create)

**Purpose:** Baseline extraction, warning generation, empty-model behavior
**Elements:**
- 1 Level ("Ground Floor" at 0.0)
- 4 Walls forming a rectangle
- 0 Rooms (no room separation lines or placed rooms)
- 0 MEP systems
- Default materials only
- Project Information: Set only Name and Number

**Expected Warnings:** NO_ROOMS, NO_FLOOR_AREA, NO_MEP_SYSTEMS
**Quick Setup:**
1. File → New → Project → Architectural Template
2. Draw 4 walls in plan view
3. Set Project Name = "Minimal Test" and Number = "MIN-001"
4. Save as "Test_Minimal.rvt"

### Model 2: Small Residential (5-10 minutes)

**Purpose:** Complete extraction with all data types, standard test case
**Elements:**
- 3 Levels (Ground 0.0, First 3.5m, Roof 7.0m)
- 15-20 Walls (exterior + interior)
- 2 Floors
- 1 Roof
- 4-6 Rooms (Living Room, Kitchen, Bedroom 1, Bedroom 2, Bathroom, Hallway) with Room Separation Lines
- 8-10 Doors
- 6-8 Windows
- No MEP systems
- Set all 8 Project Information fields
- Add 2-3 materials (Concrete, Brick, Glass)

**Expected Warnings:** NO_MEP_SYSTEMS only
**Quick Setup:**
1. File → New → Project → Architectural Template
2. Create 3 levels in section view
3. Draw floor plans with walls, doors, windows
4. Place rooms in each enclosed space
5. Add floors and roof
6. Fill all Project Information fields
7. Save as "Test_SmallResidential.rvt"

### Model 3: Medium Commercial (15-20 minutes)

**Purpose:** MEP systems, larger element counts, performance testing
**Elements:**
- 5 Levels (Basement -3.0m, Ground 0.0, L1 4.0m, L2 8.0m, Roof 12.0m)
- 50+ Walls
- 5 Floors
- 1 Roof
- 15-20 Rooms across multiple levels
- 20+ Doors, 15+ Windows
- Structural columns (6-8)
- MEP: 2-3 Mechanical Systems, 1 Piping System, 1 Electrical System (use MEP template or add systems)
- 10+ Materials
- Furniture in some rooms

**Expected Warnings:** None (all data present)
**Quick Setup:**
1. File → New → Project → Architectural Template (or Systems template for MEP)
2. Build multi-level building with varied spaces
3. Add structural columns on grid
4. Add duct/pipe runs (even short stubs trigger system creation)
5. Complete all project info
6. Save as "Test_MediumCommercial.rvt"

### Model 4: Large / Stress Test (use Revit sample files)

**Purpose:** Performance testing, list capping (200 items), large JSON
**Source:** Use Autodesk-provided sample files:
- `rac_basic_sample_project.rvt` (ships with Revit)
- `rst_basic_sample_project.rvt` (structural)
- Or download from Autodesk Knowledge Network

**Expected Behavior:**
- Extraction < 2 seconds
- Room/material lists may hit 200-item cap
- JSON size should be checked (< 30KB threshold)
- All document types generate successfully

### Model 5: Edge Case Model (5 minutes)

**Purpose:** Boundary conditions, unusual data
**Elements:**
- 1 Level at elevation 0.0
- Walls with NO materials assigned
- Room with Area = 0 (unenclosed room)
- Room with very long name (50+ characters)
- Project Name with special characters: `Test "Project" <#1> & Co.`
- Duplicate level names (if allowed)
- 0 structural elements

**Expected Warnings:** NO_MEP_SYSTEMS, potentially NO_FLOOR_AREA
**Purpose:** Tests string escaping, empty filtering, edge case handling

---

## D. Grounding Validation Checklist

### Procedure: Compare Generated Document Against ModelContext

For each generated document, perform these checks:

#### Step 1: Capture ModelContext

Before generating, enable debug logging. The `PromptBuilder` serializes `ModelContext` as JSON in the user prompt. Capture this JSON from the log or add a temporary `File.WriteAllText` in `MainViewModel.GenerateAsync` right after extraction:

```
// Temporary debug line in MainViewModel.GenerateAsync after extraction:
System.IO.File.WriteAllText(
    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
    "modelcontext_debug.json"),
    System.Text.Json.JsonSerializer.Serialize(modelContext, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
```

#### Step 2: Cross-Reference Checklist

| Check | How to Verify | Pass Criteria |
|-------|---------------|---------------|
| **Project Name** | Compare doc header `**Project:**` to `ModelContext.projectInfo.name` | Exact match |
| **Room Count** | Count rooms mentioned in doc vs. `ModelContext.rooms.length` | Match or doc says "X rooms" with correct X |
| **Room Names** | List room names in doc vs. `ModelContext.rooms[].name` | All names from context, no invented rooms |
| **Room Areas** | Check area values in doc vs. `ModelContext.rooms[].area` | Values match (within rounding) |
| **Level Count** | Count levels in doc vs. `ModelContext.levels.length` | Exact match |
| **Level Names** | List level names in doc vs. `ModelContext.levels[].name` | All from context, none invented |
| **Wall Count** | Find wall count in doc vs. `ModelContext.categoryCounts` where category="Walls" | Exact match |
| **Door Count** | Find door count vs. `ModelContext.categoryCounts` where category="Doors" | Exact match |
| **Window Count** | Find window count vs. context | Exact match |
| **Material Names** | List materials in doc vs. `ModelContext.materials[].name` | All from context, none invented |
| **MEP System Names** | List systems vs. `ModelContext.mepSystems[].name` | All from context, none invented |
| **Total Floor Area** | Find total area vs. `ModelContext.quantitySummary.totalFloorArea` | Exact match |
| **Warnings Reflected** | Check if doc mentions missing data | Warnings from context mentioned as caveats |
| **No External Standards** | Search for "ASHRAE", "IBC", "NEC", "BS EN" | None present unless in ModelContext |
| **No Invented Quantities** | Search for numbers not in ModelContext | All numbers traceable |
| **"Not Available" Phrases** | If data missing, doc should say so | Present where data is absent |

#### Step 3: Red Flags (Automatic Fail)

- Document mentions a room name not in ModelContext
- Document states a specific wall count that differs from context
- Document references a building code or standard not in model data
- Document contains a material specification not in the materials list
- Document invents a project address or client name not in ProjectInfo

#### Step 4: Acceptable AI Interpretation

These are NOT grounding violations:
- Describing walls generically ("The building contains 42 wall elements")
- Using synonyms ("floor slabs" for "Floors")
- Aggregating provided data ("Across 3 levels, there are 15 rooms")
- Qualitative observations based on quantities ("The building has a moderate number of MEP systems")

---

## E. Performance & Stability Checklist

### Timings to Record

| Metric | Where to Measure | Method |
|--------|-----------------|--------|
| **Extraction Time** | Time between "Extracting model data..." and "Generating documentation with AI..." | Log timestamps OR Stopwatch |
| **AI Call Time** | Time between sending request and receiving response | Log timestamps in OpenAiDocumentGenerator |
| **Total Generation Time** | Time between clicking Generate and "Document generated successfully" | Stopwatch |
| **Render Time** | Time to display Markdown in TextBox | Visual (should be instant) |
| **Export Time** | Time to write .md file to disk | Visual (should be instant) |
| **Memory Delta** | Memory before/after extraction | Task Manager → Revit process |
| **Revit Responsiveness** | Can user pan/zoom during generation | Manual interaction test |

### Acceptable Thresholds

| Model Size | Extraction | AI Call | Total | Memory |
|------------|------------|---------|-------|--------|
| **Minimal** (< 50 elements) | < 0.5s | 5-30s | < 35s | < 10 MB |
| **Small** (< 500 elements) | < 1s | 10-45s | < 50s | < 20 MB |
| **Medium** (< 5000 elements) | < 2s | 15-60s | < 65s | < 50 MB |
| **Large** (> 5000 elements) | < 5s | 20-90s | < 100s | < 100 MB |

### Stability Tests

| Test | Procedure | Pass Criteria |
|------|-----------|---------------|
| **Repeated generation** | Generate 5 documents in a row | No memory leak, consistent timing |
| **Cancel + retry** | Cancel 3 times, then complete | Completes normally on 4th attempt |
| **Rapid clicking** | Click Generate rapidly 5 times | Only one generation runs, button properly disabled |
| **Long session** | Keep window open 30+ minutes | No degradation, Generate still works |
| **Model switch** | Generate, close doc, open new doc, Generate | New model extracted correctly |
| **Multiple windows** | Open 2 MainWindows, generate in both | Both work independently (or second blocked gracefully) |

---

## F. Failure Injection Plan

### F.1: Missing API Key

**Setup:** Remove `OPENAI_API_KEY` environment variable
```powershell
[System.Environment]::SetEnvironmentVariable("OPENAI_API_KEY", $null, "User")
```
**Steps:** Restart Revit → Open project → Click Generate Documentation
**Expected:** TaskDialog with message about missing key. Button returns `Result.Failed`.
**Restore:** Re-set the environment variable and restart Revit.

### F.2: Invalid API Key

**Setup:** Set an invalid key
```powershell
[System.Environment]::SetEnvironmentVariable("OPENAI_API_KEY", "sk-invalid-key-12345", "User")
```
**Steps:** Restart Revit → Open project → Click Generate Documentation → Click Generate
**Expected:** Status bar: "Invalid API key. Check your OPENAI_API_KEY."
**Inspect:** Log contains HTTP 401 error entry.

### F.3: Network Offline

**Setup:** Disable network adapter (Settings → Network → Disable Wi-Fi/Ethernet)
**Steps:** Click Generate (with valid API key)
**Expected:** Status bar: "Cannot reach OpenAI. Check your connection." after timeout/connection failure
**Inspect:** Log contains HttpRequestException
**Restore:** Re-enable network adapter.

### F.4: Rate Limit (429)

**Setup:** Difficult to trigger naturally. Options:
1. Use a low-tier API key and send rapid requests
2. Use the Demo Mode mock (see section G) with a mock returning HTTP 429
3. Temporarily modify `ResponsesApiClient` to always return 429 for testing

**Expected:** Status bar: "Rate limit reached. Please wait and retry." Exponential backoff in logs.

### F.5: OpenAI Timeout

**Setup:** Set very short timeout
```powershell
[System.Environment]::SetEnvironmentVariable("DOCGEN_TIMEOUT_SECONDS", "1", "User")
```
**Steps:** Restart Revit → Generate
**Expected:** Status bar: "Operation timed out. Please retry."
**Restore:** Remove the environment variable.

### F.6: Invalid JSON from AI

**Setup:** Use Demo Mode with `DemoAiDocumentGenerator` configured to return malformed JSON.
Modify `DemoAiDocumentGenerator.GenerateAsync` to simulate:
```csharp
// Return a response with invalid JSON
throw new System.Text.Json.JsonException("Simulated malformed JSON");
```
**Expected:** Retry logic engages, eventually shows error after 3 attempts.
**Inspect:** Log shows 3 retry attempts.

### F.7: Schema Mismatch

**Setup:** Use Demo Mode or modify the mock to return valid JSON missing required fields (e.g., no `title` field).
**Expected:** `ResponseValidator` catches it, retries, eventually fails with user message.

### F.8: Cancellation Mid-Request

**Setup:** Normal generation with a model that takes time to extract.
**Steps:**
1. Click Generate
2. Wait for "Generating documentation with AI..."
3. Immediately click Cancel
**Expected:** Status: "Generation cancelled." No orphaned tasks or hung UI.
**Inspect:** CancellationToken properly propagated through HttpClient.

### F.9: Export Path Denied

**Setup:** Try to export to a protected path.
**Steps:**
1. Generate a document
2. Click Export to Markdown
3. Navigate to `C:\Windows\System32\` or another protected folder
4. Try to save
**Expected:** Status: "Export failed: Access to the path '...' is denied."

### F.10: Disk Full

**Setup:** Use a nearly-full USB drive as export target.
**Steps:** Export to the full drive.
**Expected:** Clear error message about disk space.

---

## G. Helper Tools & Code Additions

### G.1: Demo Mode

A `DOCGEN_DEMO_MODE` environment variable toggles demo mode. When set to `"true"`:
- Model extraction returns a pre-built `ModelContext` (no Revit needed)
- AI generation returns a pre-built response (no OpenAI needed)
- Full UI testing possible without external dependencies

**Files added:**
- `src/Core/Testing/DemoModelContextFactory.cs` — Generates realistic sample ModelContext
- `src/Core/Testing/DemoAiDocumentGenerator.cs` — Returns pre-built GeneratedDocument
- Modified `ServiceLocator.cs` to check `DOCGEN_DEMO_MODE` environment variable
- Modified `ExternalEventManager.cs` to support demo extraction

**Activation:**
```powershell
[System.Environment]::SetEnvironmentVariable("DOCGEN_DEMO_MODE", "true", "User")
# Restart Revit
```

### G.2: Additional Unit Tests

**Files added/modified:**
- `tests/Core.Tests/Models/GroundingValidationTests.cs` — Tests that serialized ModelContext round-trips correctly
- `tests/AiClient.Tests/ResponseValidatorEdgeCaseTests.cs` — Edge case validation tests
- `tests/Core.Tests/Models/MarkdownRenderingTests.cs` — Comprehensive markdown output tests

### G.3: Critical Bug Fixes

**Fixes applied:**
1. `OpenAiDocumentGenerator.cs` — Catch `JsonException` in retry loop
2. `ResponseValidator.cs` — Validate metadata field contents
3. `MainViewModel.cs` — Export uses raw edited text instead of broken ParseEditedDocument
