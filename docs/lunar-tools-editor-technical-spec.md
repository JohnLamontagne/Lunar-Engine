# Lunar.Tools.Editor Technical Specification

## Purpose
This document is the implementation-oriented companion to `docs/lunar-tools-editor-web-migration-plan.md`.

## Source Plan
- Primary planning document: `docs/lunar-tools-editor-web-migration-plan.md`

## Current Constraints and Decisions
- The new editor is **local first**.
- The new tool family is **`Lunar.Tools.Editor`**.
- The editor will be a **browser UI + local backend API**.
- Dialogues remain **XML-based**.
- Scripting is **C# + Roslyn**, not the legacy system.
- The old script map approach is **obsolete** and must not be reintroduced.
- Backward compatibility for old editor data formats is **not required**.
- The implementation should prefer modern .NET and strongly typed contracts.

## Success Criteria
- A developer can launch a local editor backend and open a project in a browser.
- The editor backend owns filesystem access and content persistence.
- The editor no longer depends on WinForms, DarkUI, or MonoGame for non-rendering concerns.
- The first shipped browser editors can edit scripts, items, spells, NPCs, and dialogues end-to-end.
- The map editor is rebuilt as a web-native canvas/WebGL surface.

## Architecture Overview

### Proposed Projects
- `src/Lunar.Tools.Editor.Contracts`
- `src/Lunar.Tools.Editor.Core`
- `src/Lunar.Tools.Editor.Api`
- `src/Lunar.Tools.Editor.Web`

### Responsibilities

#### `Lunar.Tools.Editor.Contracts`
Contains:
- DTOs returned over HTTP
- command/request contracts
- validation issue contracts
- change notification payloads
- editor-facing document contracts where they are transport-safe

Should not contain:
- filesystem access
- UI logic
- server/runtime execution logic

#### `Lunar.Tools.Editor.Core`
Contains:
- project loading/opening logic
- repositories
- content catalog/indexing
- asset reference discovery
- validation rules
- document serialization/deserialization
- orchestration services used by the API

Should not contain:
- ASP.NET controllers
- React/TypeScript code
- WinForms dependencies

#### `Lunar.Tools.Editor.Api`
Contains:
- HTTP endpoints
- endpoint-to-service mapping
- local host startup
- DI registration
- file watcher integration
- optional SignalR hub

Should not contain:
- business logic duplicated from `Core`

#### `Lunar.Tools.Editor.Web`
Contains:
- application shell
- tabs/documents
- content tree
- Monaco-based script editing
- form-based content editors
- canvas/WebGL map editor
- diagnostics UI

Should not contain:
- direct file writes
- hidden serialization rules duplicated from backend

## Project and Solution Layout

### Recommended Solution Additions
- Add the new projects to `src/Lunar Engine.sln`.
- Keep the legacy `Lunar.Editor` project during migration.
- Do not attempt to replace `Lunar.Editor` until milestone-based cutover criteria are met.

### Recommended Folder Layout
Within `src/`:

- `Lunar.Tools.Editor.Contracts/`
  - `Projects/`
  - `ContentTree/`
  - `Documents/`
  - `Validation/`
  - `Scripts/`
  - `Assets/`
  - `Changes/`

- `Lunar.Tools.Editor.Core/`
  - `Projects/`
  - `Repositories/`
  - `Services/`
  - `Serialization/`
  - `Validation/`
  - `Indexing/`
  - `Watching/`
  - `Mappings/`

- `Lunar.Tools.Editor.Api/`
  - `Endpoints/` or `Controllers/`
  - `Hubs/`
  - `Configuration/`
  - `Extensions/`

- `Lunar.Tools.Editor.Web/`
  - `src/app/`
  - `src/features/project-tree/`
  - `src/features/documents/`
  - `src/features/scripts/`
  - `src/features/items/`
  - `src/features/spells/`
  - `src/features/npcs/`
  - `src/features/dialogues/`
  - `src/features/maps/`
  - `src/features/animations/`
  - `src/components/`
  - `src/lib/api/`
  - `src/lib/state/`
  - `src/lib/contracts/`

## Technology Recommendations

### Backend
- .NET 9
- `ASP.NET Core Minimal APIs`
- `System.Text.Json`
- `FileSystemWatcher`
- `SignalR`

### Frontend
- React
- TypeScript
- Vite
- TanStack Query
- Zustand or Redux Toolkit
- Monaco Editor
- PixiJS

## Core Implementation Principles

### 1. Backend is source of truth
The browser is a client of the local editor API. The backend owns:
- absolute paths
- filesystem permissions
- serialization
- create/delete/rename logic
- validation
- external change detection

### 2. Editor documents are authoring models
Do not directly reuse runtime-heavy or rendering-heavy objects as persisted editor contracts.

Preferred pattern:
- backend repository loads raw content
- mapper converts to editor document contract
- frontend edits editor document contract
- backend validates and persists

### 3. References are explicit
Script, asset, animation, dialogue, and behavior relationships should be stored directly on content definitions rather than in ad hoc sidecar mappings.

### 4. No desktop UI assumptions in Core
Any logic currently expressed through WinForms events, dialogs, or document controls must be extracted into services or validators before being considered migrated.

### 5. Avoid runtime leakage
The editor should not depend on server execution behavior for authoring workflows except where static inspection is needed for diagnostics.

## Data and Contract Strategy

### Project Manifest
The implementation may either:
- keep `.lproj` temporarily behind a repository abstraction, or
- introduce a new JSON manifest immediately

Recommendation:
- abstract project manifest loading behind `IProjectManifestRepository`
- support current format internally first if that speeds migration
- allow later replacement without frontend changes

### Content Formats

#### XML
- Dialogues remain XML

#### JSON-preferred
- Maps
- Items
- NPCs
- Spells
- Animations

### Versioning
New or redesigned formats should include:
- `schemaVersion`
- `contentType`

This does not imply backward compatibility. It exists for future evolution and validation.

## Scripting Model

### Current Direction
The server now compiles scripts from `.cs` source using Roslyn and discovers behaviors/scripts from compiled types and attributes.

Current behavior to preserve conceptually:
- script source files live on disk
- backend can compile all script sources
- diagnostics can be surfaced per file
- content references scripts/behaviors directly

### Editor Expectations
- Scripts are edited as plain C# source files.
- The editor should support:
  - syntax highlighting
  - search
  - dirty state
  - save
  - compile/diagnostics view
- The API should expose script diagnostics as structured results, not only raw strings.

### Script Reference Direction
Prefer content-owned references such as:
- `NpcDocument.BehaviorKey`
- `ItemDocument.BehaviorKey`
- `DialogueDocument.ScriptTypeName` or equivalent XML-mapped reference

Avoid:
- centralized script maps
- hidden editor-only script attachment state

## Dialogue Direction

### Required Constraints
- Dialogue persistence remains XML.
- Dialogue authoring must still be separated from runtime execution code.

### Recommended Structure
Create editor-safe XML-oriented models in `Lunar.Tools.Editor.Core`, for example:
- `DialogueXmlDocument`
- `DialogueBranchDocument`
- `DialogueResponseDocument`

These are authoring models only. Runtime objects in `Lunar.Server` should not be edited directly by the browser stack.

### Validation Targets
- duplicate branch names
- missing next-branch targets
- missing script method references
- invalid condition/function names
- malformed XML

## Recommended Core Interfaces

These names are suggestions, not mandates. The important part is the separation of concerns.

### Project Layer
```csharp
public interface IProjectManifestRepository
{
    ProjectManifest Load(string projectPath);
    ProjectManifest Create(CreateProjectRequest request);
    void Save(ProjectManifest manifest);
}

public interface IProjectWorkspaceService
{
    ProjectWorkspace Open(string projectPath);
    ProjectWorkspace Create(CreateProjectRequest request);
}
```

### Catalog and Indexing
```csharp
public interface IContentCatalogService
{
    ContentTreeDocument BuildTree(ProjectWorkspace workspace);
    ContentIndexDocument BuildIndex(ProjectWorkspace workspace);
}

public interface IWorkspaceWatcherService
{
    void Start(ProjectWorkspace workspace);
    void Stop();
}
```

### Repositories
```csharp
public interface IItemRepository
{
    ItemEditorDocument Load(ProjectWorkspace workspace, ContentId id);
    ItemEditorDocument Create(ProjectWorkspace workspace, CreateItemRequest request);
    void Save(ProjectWorkspace workspace, ItemEditorDocument document);
    void Delete(ProjectWorkspace workspace, ContentId id);
    RenameResult Rename(ProjectWorkspace workspace, ContentId id, string newName);
}
```

Equivalent repositories should exist for:
- maps
- spells
- NPCs
- dialogues
- animations
- scripts

### Validation
```csharp
public interface IDocumentValidator<TDocument>
{
    IReadOnlyList<ValidationIssueDto> Validate(TDocument document, ProjectWorkspace workspace);
}

public interface IProjectValidationService
{
    ProjectValidationReport ValidateProject(ProjectWorkspace workspace);
}
```

### Script Diagnostics
```csharp
public interface IScriptDiagnosticsService
{
    ScriptCompilationReport Compile(ProjectWorkspace workspace);
    ScriptFileDiagnosticsReport GetDiagnostics(ProjectWorkspace workspace, string relativePath);
}
```

## HTTP API Shape

The exact route style can vary, but the capabilities below should exist.

### Projects
- `POST /api/projects/open`
- `POST /api/projects/create`
- `GET /api/projects/current`
- `GET /api/projects/current/tree`
- `GET /api/projects/current/index`

### Scripts
- `GET /api/scripts`
- `GET /api/scripts/{id}`
- `PUT /api/scripts/{id}`
- `POST /api/scripts`
- `DELETE /api/scripts/{id}`
- `POST /api/scripts/compile`
- `GET /api/scripts/diagnostics`

### Items
- `GET /api/items/{id}`
- `PUT /api/items/{id}`
- `POST /api/items`
- `DELETE /api/items/{id}`
- `POST /api/items/{id}/rename`

### Spells
- `GET /api/spells/{id}`
- `PUT /api/spells/{id}`
- `POST /api/spells`
- `DELETE /api/spells/{id}`
- `POST /api/spells/{id}/rename`

### NPCs
- `GET /api/npcs/{id}`
- `PUT /api/npcs/{id}`
- `POST /api/npcs`
- `DELETE /api/npcs/{id}`
- `POST /api/npcs/{id}/rename`

### Dialogues
- `GET /api/dialogues/{id}`
- `PUT /api/dialogues/{id}`
- `POST /api/dialogues`
- `DELETE /api/dialogues/{id}`
- `POST /api/dialogues/{id}/rename`

### Maps
- `GET /api/maps/{id}`
- `PUT /api/maps/{id}`
- `POST /api/maps`
- `DELETE /api/maps/{id}`
- `POST /api/maps/{id}/rename`

### Animations
- `GET /api/animations/{id}`
- `PUT /api/animations/{id}`
- `POST /api/animations`
- `DELETE /api/animations/{id}`
- `POST /api/animations/{id}/rename`

### Validation and Assets
- `GET /api/validation/project`
- `POST /api/validation/document`
- `GET /api/assets`
- `GET /api/assets/textures`
- `GET /api/assets/tilesets`

### Change Notifications
If SignalR is used:
- `/hubs/workspace`

Suggested message types:
- `workspaceChanged`
- `contentChanged`
- `assetChanged`
- `scriptDiagnosticsChanged`

## Contract Shapes

### Shared Rules
- DTOs should use relative content identifiers, not absolute machine paths.
- Absolute paths must remain backend-only.
- Documents should include enough metadata for tab titles, dirty checks, and diagnostics.

### Suggested Common Fields
For content documents:
- `id`
- `name`
- `contentType`
- `schemaVersion`
- `sourcePath`
- `lastModifiedUtc`

For validation issues:
- `severity`
- `code`
- `message`
- `path`
- `field`
- `line`
- `column`

### Suggested Example Types
```csharp
public sealed record ContentRef(string Id, string Name, string ContentType);
public sealed record AssetRef(string Path, string AssetType);
public sealed record ScriptRef(string RelativePath, string? TypeName);
```

## Milestone-Oriented Implementation Sequence

## Milestone 1: Solution Scaffolding
Deliver:
- new projects created
- solution updated
- base DI setup in API
- base frontend app created
- contracts project referenced cleanly

Acceptance:
- solution builds
- API runs locally
- web app runs locally

## Milestone 2: Core Project Open and Catalog
Deliver:
- `ProjectWorkspace`
- project manifest loader
- content catalog service
- content tree DTOs
- basic repositories for scripts/items/spells/NPCs/dialogues

Acceptance:
- backend can open a project
- backend returns a stable content tree
- no UI dependencies in core

## Milestone 3: Script Editing Vertical Slice
Deliver:
- script repository
- load/save endpoints
- Monaco integration
- compile diagnostics endpoint
- diagnostics UI

Acceptance:
- user can open, edit, save, and compile C# script files in browser

## Milestone 4: Item and Spell Editors
Deliver:
- item/spell document contracts
- repositories
- validators
- frontend forms

Acceptance:
- user can create/edit/save item and spell documents end-to-end

## Milestone 5: NPC and Dialogue Editors
Deliver:
- NPC and dialogue repositories
- XML dialogue serializer/editor model
- reference-aware validation
- frontend editors

Acceptance:
- user can edit NPCs and dialogues end-to-end
- script references are visible and validated

## Milestone 6: Map Editor Foundations
Deliver:
- finalized map authoring contract
- backend load/save contract
- frontend map canvas renderer
- tileset browser and layer panel

Acceptance:
- map opens and renders in browser

## Milestone 7: Map Tooling
Deliver:
- paint/fill/erase/select
- object placement
- attribute editing
- undo/redo

Acceptance:
- map workflow is viable for day-to-day use

## Milestone 8: Animation and Hardening
Deliver:
- animation editor
- project-wide validation
- external file change handling
- performance pass

Acceptance:
- legacy editor is no longer required for normal workflows

## Implementation Guardrails for AI Agents

### Required Behavior
- Do not reintroduce WinForms patterns into new projects.
- Do not couple frontend code to absolute file paths.
- Do not duplicate serialization rules across frontend and backend.
- Keep dialogue XML but isolate it behind editor-safe models and repositories.
- Keep script handling aligned with the current Roslyn-based system.
- Prefer incremental vertical slices over broad unfinished scaffolding.

### Preferred Order of Work
1. Scaffold projects
2. Define contracts
3. Implement repositories and services
4. Expose HTTP endpoints
5. Build one complete frontend feature at a time

### Vertical Slice Strategy
The first complete vertical slice should be:
- open project
- view project tree
- open script
- edit script
- save script
- compile scripts
- display diagnostics

This is the best proof that the architecture is correct before investing in more editors.

## Risks and Watch Items
- Runtime/editor coupling may still exist in current models and should be removed deliberately.
- Dialogue logic is historically tied to server code and must be separated carefully.
- Map editor complexity is high; defer it until the backend document model is stable.
- Legacy naming inconsistencies should be resolved early before API contracts harden.
- Filesystem assumptions must be isolated to the backend from the beginning.

## Explicit Open Decisions
These need review before implementation hardens:
- Whether content IDs are distinct from filenames in v1
- Whether `.lproj` is retained temporarily or replaced immediately
- Whether map attributes remain tile-embedded
- Whether dialogue XML structure remains branch/response as-is or evolves while staying XML
- How strict save-blocking validation should be

## Recommended Immediate Tasks
1. Create the four new editor projects.
2. Add them to the solution.
3. Implement `ProjectWorkspace` and `IProjectManifestRepository`.
4. Implement `IContentCatalogService`.
5. Define initial contracts for content tree, validation, script documents, and project open.
6. Expose project-open and content-tree API endpoints.
7. Build the script editing vertical slice.

## Definition of Done for the Technical Foundation
The foundation phase is complete when:
- a local API can open a project
- the browser can display the content tree
- a script can be edited and saved from the browser
- Roslyn diagnostics are visible in the browser
- no new implementation depends on WinForms editor code
