# Lunar.Tools.Editor Web Migration Plan

## Current Decisions
- The new editor will be **local first**.
- Hosting, authentication, collaboration, and permissions are **out of scope for now**.
- The new toolset will be organized under **`Lunar.Tools.Editor`**.
- **Dialogues remain XML-based**.
- Other content types may use JSON where that modernization is useful and practical.
- The old script map approach is no longer relevant.
- Scripting is now based on **C# + Roslyn**, not the previous scripting implementation.
- Backward compatibility for legacy editor data formats is **not required**.

## Goals
- Replace the WinForms editor with a browser-based editor backed by a local API.
- Separate editor concerns from runtime/server concerns.
- Clean up and modernize content definitions while the editor is being rebuilt.
- Preserve the ability to edit all major game content locally from a single tool.
- Make future improvements such as collaboration, validation, and richer tooling easier to add later.

## Non-Goals
- Hosted deployment
- User accounts or permissions
- Multi-user editing
- Backward compatibility with old editor data
- Perfect UI parity with the WinForms docking model

## Proposed Solution Structure

### Projects
- `Lunar.Tools.Editor.Web`
  - React + TypeScript browser application
- `Lunar.Tools.Editor.Api`
  - Local `ASP.NET Core` backend running on `localhost`
- `Lunar.Tools.Editor.Core`
  - Editor application/domain layer
- `Lunar.Tools.Editor.Contracts`
  - Shared DTOs, commands, responses, validation results

### Responsibility Split

#### `Lunar.Tools.Editor.Web`
- Shell layout
- Tabs/documents
- Property panels
- Content tree
- Canvas-based map editing
- Script editing UI
- Validation display
- Dirty state and save workflows

#### `Lunar.Tools.Editor.Api`
- Project open/create
- Filesystem access
- Content CRUD
- Validation
- Asset discovery
- File watching
- Save/rename/delete behavior

#### `Lunar.Tools.Editor.Core`
- Project rules
- Content indexing
- Content service layer
- Validation rules
- Reference resolution
- Editor-safe document models
- Mapping between persisted content and editor contracts

## Local-First Architecture
- The browser app will not write files directly.
- The local API will own all filesystem access.
- The API will load project state from disk, expose it over HTTP, and persist all changes.
- External file changes can be handled with file watching and surfaced back to the browser.
- Polling can work initially, but `SignalR` is a good fit if live refresh is needed early.

## Data Modernization Strategy

### High-Level Direction
Use this overhaul to move from legacy, UI-coupled models toward explicit editor-facing content definitions. The editor should edit definitions/documents, not runtime objects with embedded engine behavior.

### Core Principles
- Use explicit document models for authoring.
- Use stable identifiers for content where appropriate.
- Keep loaded graphics/runtime objects out of persisted editor documents.
- Make references explicit and validation-friendly.
- Normalize naming and terminology during migration.
- Add schema/version fields to new formats where practical.

### Content Format Direction

#### Remain XML
- Dialogues

#### Prefer JSON
- Maps
- Items
- NPCs
- Spells
- Animations
- Project manifest, if redesigned

### Data Cleanup Opportunities

#### 1. Separate authoring models from runtime models
Introduce editor-safe document models such as:
- `ProjectDocument`
- `MapDocument`
- `ItemDocument`
- `NpcDocument`
- `SpellDocument`
- `AnimationDocument`
- `DialogueDocument` or equivalent XML-backed contract

Runtime/server-specific behavior should be mapped from these definitions rather than edited directly.

#### 2. Normalize terminology
The current codebase shows signs of age and inconsistency. The migration should standardize:
- `Defense` vs `Defence`
- `AggressiveRange` naming
- requirement/modifier naming
- path/reference conventions
- content identifiers and file naming rules

#### 3. Make references first-class
References should be modeled explicitly:
- animation references
- asset references
- dialogue references
- behavior/script references
- map object or trigger references

#### 4. Remove legacy script map assumptions
The new scripting system no longer needs a script map. The new editor should instead model behavior/script relationships directly in the owning content documents.

#### 5. Keep dialogues editor-safe but XML-based
Dialogues should remain XML-based, but the editor-facing representation should still be isolated from server/runtime execution logic. The XML format can stay while the code structure improves.

## Scripting Direction
- The editor should treat scripts as C# source files.
- Script authoring should use a modern code editor component in the browser.
- Validation can begin with basic parse/build diagnostics surfaced from the local backend.
- Script references should be stored directly on the relevant content documents instead of in a separate sidecar mapping.
- Runtime execution concerns should remain outside the editor UI and core contracts.

## Proposed Workstreams

### Workstream 1: Planning and Contract Design
- Confirm scope for v1
- Define new project layout
- Define document contracts
- Define reference rules
- Define validation model

### Workstream 2: Core Extraction
- Replace the old `Project` monolith with focused services
- Move editor rules out of WinForms event handlers
- Create repositories/services for each content type
- Build content indexing and change tracking

### Workstream 3: Local API
- Create backend project
- Expose project/content CRUD
- Expose validation endpoints
- Expose asset and file index endpoints
- Add external file change detection

### Workstream 4: Web Shell
- Build application shell
- Build document/tab model
- Build project tree
- Build save/save-all flows
- Build problem display and reload flows

### Workstream 5: Content Editors
- Script editor
- Item editor
- Spell editor
- NPC editor
- Dialogue editor
- Map editor
- Animation editor

## Recommended Delivery Order

### Phase 0: Architecture and RFCs
Deliverables:
- agreed local-first architecture
- agreed project structure
- agreed content strategy
- agreed v1 feature scope

Exit criteria:
- team alignment on what is being built first

### Phase 1: `Lunar.Tools.Editor.Core`
Deliverables:
- core service layer
- filesystem repositories
- content catalog/index
- validation primitives
- editor-safe document contracts

Exit criteria:
- backend logic can load and save content without any UI dependency

### Phase 2: Content Model Redesign
Deliverables:
- revised contracts for maps, items, NPCs, spells, animations
- XML dialogue editing contract and serializer strategy
- script reference model aligned to C#/Roslyn
- cleanup of naming and reference conventions

Exit criteria:
- new content documents are coherent and ready to be surfaced through the API

### Phase 3: Local API
Deliverables:
- project open/create endpoints
- content tree endpoint
- load/save/create/delete/rename endpoints
- validation endpoints
- asset listing endpoints

Exit criteria:
- browser can operate as a thin client over the local API

### Phase 4: Web Shell
Deliverables:
- app layout
- project browser
- tabbed documents
- dirty state handling
- keyboard shortcuts
- save flows

Exit criteria:
- a project can be opened and navigated entirely in the browser

### Phase 5: First Editors
Recommended order:
1. Script editor
2. Item editor
3. Spell editor
4. NPC editor
5. Dialogue editor

Rationale:
- these are lower risk than maps
- they prove the backend contracts quickly
- they deliver value before tackling rendering-heavy work

Exit criteria:
- most non-map content can be edited end-to-end in the web editor

### Phase 6: Map Editor
Deliverables:
- map document format finalized
- canvas/WebGL rendering surface
- tileset browser
- layer management
- paint/fill/erase/select tools
- object placement/editing
- attribute editing
- undo/redo

Exit criteria:
- the web map editor is good enough to replace the desktop map workflow

### Phase 7: Animation Editor
Deliverables:
- animation document cleanup if needed
- animation editing UI
- texture/frame controls
- preview playback

Exit criteria:
- animation editing no longer requires the WinForms editor

### Phase 8: Hardening and Cutover
Deliverables:
- project-wide validation
- missing asset/reference reporting
- external change handling
- performance pass
- parity verification

Exit criteria:
- the web editor is suitable as the primary editing workflow

## Initial Milestones

### Milestone 1
Architecture and contracts are defined.

Includes:
- project structure
- document model direction
- scripting/reference direction
- API boundary definition

### Milestone 2
`Lunar.Tools.Editor.Core` can open a project and expose a content catalog.

Includes:
- repositories
- indexing
- validation primitives
- basic create/load/save flows

### Milestone 3
The local API and web shell can open a project and edit scripts/items/spells.

### Milestone 4
NPC and dialogue authoring are functional in the browser.

### Milestone 5
The map editor is functional enough for primary workflow use.

### Milestone 6
Animation editing and hardening are complete enough to retire the WinForms editor.

## Suggested First Backlog
1. Create the new editor solution/projects
2. Write architecture and data RFCs
3. Define editor-safe contracts for content types
4. Define the project manifest format
5. Define script reference conventions for C#/Roslyn scripts
6. Extract content repositories/services from the current editor
7. Implement project open + content catalog in the local API
8. Build the web shell and project tree
9. Build the script editor
10. Build the item editor
11. Build the spell editor
12. Build the NPC editor
13. Build the dialogue editor
14. Build the map editor
15. Build the animation editor

## Open Questions for Review
- Should all non-dialogue content move to new JSON contracts immediately, or should some current formats be preserved temporarily?
- Should content ids be separate from filenames from the beginning?
- Should dialogues stay branch/response based, or should the XML structure evolve while remaining XML?
- How much validation should block saving versus only showing warnings?
- Should the new project manifest replace `.lproj`, or should the current project file remain until later?
- Should map attributes remain tile-embedded, or be reworked into clearer trigger/attribute structures?
- Should behavior references be uniform across items, NPCs, spells, and dialogues?

## Recommended Immediate Next Step
Refine and approve this document, then turn Phase 0 and Phase 1 into a concrete implementation spec with:
- proposed folder structure
- project references
- first-pass contracts
- repository interfaces
- initial API route list
