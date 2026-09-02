# Lunar Engine 3D Revamp: Assessment and Roadmap

## Purpose

This document records an architecture assessment of the `modernization` branch and lays out a
path for turning Lunar Engine from a 2D tile-based online RPG engine into a 3D MMORPG engine.
It is written in the same spirit as `lunar-tools-editor-technical-spec.md`: decisions first,
then the evidence, then a milestone sequence that an engineer (or an AI agent) can execute.

The assessment was done against commit `cb748b1` ("Scaffold Lunar.Tools.Editor web editor
stack (Milestones 1-4)"), which is the tip of `modernization`.

## Summary

- The modernization work has already done the hardest prerequisite: `Lunar.Core` has **no
  MonoGame dependency**, the server has **no rendering dependency**, scripting is Roslyn-based
  and dimension-agnostic, and the editor is being rebuilt as a web app whose map editor has
  **not been started yet**. That is the ideal moment to change the world model.
- The 2D assumption is not concentrated in one place. It lives in four primitives (`Vector`,
  `Rect`, `Direction`, the string `Layer`) and one data structure (the `Tile[,]` grid), and
  from there it is woven through collision, pathfinding, spawning, warping, the wire format
  and the on-disk map schema. The client rendering layer is fully 2D (`SpriteBatch`) and the
  gameplay classes on the client are also the render classes.
- Roughly two thirds of the 3D work is **renderer independent**: a 3D world model in Core, a
  zone/navmesh based server simulation, a 3D wire protocol, and a zone editor. That work is the
  same whichever 3D client technology is chosen, so it should start first.
- The one decision that must be made deliberately is **which graphics library sits under the
  3D client**. Lunar stays the engine and owns its renderer; adopting a third-party engine as
  the front end was rejected. The realistic backends are MonoGame 3.8.5 (now with Vulkan and
  DirectX 12 targets) and NeoVeldrid, with SDL3 GPU as a watch item. The plan builds
  `Lunar.Rendering` against a thin backend interface and settles the first backend with a
  short measured spike.
- Several pre-existing server issues (no synchronization between the net and world threads,
  no area-of-interest filtering, inventory not persisted, no autosave) are not 3D problems but
  they become MMORPG blockers. They are listed so they can be scheduled alongside the revamp.

## What Exists Today

### Solution layout (net9.0 unless noted)

| Project | Role | Key dependencies |
|---|---|---|
| `Lunar.Core` | Shared data model, descriptors, JSON data managers, packet buffer | `Microsoft.Extensions.DependencyInjection.Abstractions` only |
| `Lunar.Server` | Authoritative simulation, LiteNetLib transport, Roslyn scripting | LiteNetLib 2.1.3, Microsoft.CodeAnalysis.CSharp 4.12 |
| `Lunar.Graphics` | Sprite, SpriteSheet, animation, MonoGame interop | MonoGame.Framework.DesktopGL 3.8.4.1 |
| `Lunar.Client` | Scenes, world view, GUI, client net | MonoGame DesktopGL, Penumbra 3.0.0, LiteNetLib |
| `Lunar.Client.Desktop` | Executable, Penumbra wiring, dev console, content | `Lunar.Client` |
| `Lunar.Client.Mobile` | Stub. Not buildable; does not reference `Lunar.Client` | MonoGame Android/iOS |
| `Lunar.Tools.Editor.{Contracts,Core,Api,Web}` | Local-first web editor (items, spells, scripts) | ASP.NET Core minimal API, React 19, Vite, Monaco |
| `Lunar.Editor` | Legacy WinForms editor. .NET Framework 4.8, IronPython, **not in the solution** | DarkUI, MonoGame 3.7 |
| `Lunar.UnitTests` | Legacy MSTest project. .NET Framework 4.8, **not in the solution**, references removed APIs | |
| `Lunar.Core.Tests` | **New in this branch.** xunit, net9.0 | |

All solution projects build with zero warnings on Linux with the .NET 10 SDK.

### Where the 2D assumption lives

The following inventory was compiled by reading the code. File references are to the
`modernization` tip so they can be located after refactoring.

**Core primitives (everything downstream depends on these)**

- `Vector` is two floats (`Lunar.Core/Utilities/Data/Vector.cs`). There is no Z anywhere.
- `Rect` is an integer 2D box and is *the* collision primitive (`Lunar.Core/Utilities/Data/Rect.cs`).
- `Direction` is a four-value enum (`Lunar.Core/World/Direction.cs`); movement on both client and
  server is axis-decomposed from it.
- `IActorModel.Position`, `Reach`, `CollisionBounds` are `Vector`/`Rect`
  (`Lunar.Core/World/Actor/Descriptors/IActorModel.cs`).
- Position on the wire is `Write(Vector)` = 2 floats and `Write(Rect)` = 4 ints
  (`Lunar.Core/Net/PacketExtensions.cs`). The format is positional with no field tags.

**The tile grid**

- `LayerModel<T>.Tiles` is a `T[,]` sized from `Dimensions` (`Lunar.Core/World/Structure/LayerModel.cs`).
- `TileModel` carries `Blocked`, `Teleporter`, `LightSource`, `LightRadius`, and a per-tile
  `TileAttribute` (`Lunar.Core/World/Structure/TileModel.cs`).
- Server `Layer.CheckCollision` does a grid-bounds test plus a `BlockedTileAttribute` scan; tile
  lookup is integer division by `TileSize` (`Lunar.Server/World/Structure/Layer.cs`).
- `Layer.Update` iterates **every tile in the layer every tick** at 120 Hz.
- `Pathfinder` is 4-neighbour A* over a `SearchNode[,]` grid, stateful and not reentrant
  (`Lunar.Server/Utilities/Pathfinding/Pathfinder.cs`).
- Spawning, warping, blocking and dialogue triggers are all **per-tile attributes** serialized as
  a hand-rolled binary blob base64-embedded in the JSON map
  (`Lunar.Core/World/Structure/Attribute/TileAttribute.cs`).
- `MapFSDataManager` persists `Dimensions{X,Y}`, sparse `TileEntryDto{X,Y}`, sprite rects and
  tileset paths (`Lunar.Core/Utilities/Data/FileSystem/MapFSDataManager.cs`).

**Layers as a pseudo third dimension**

- Every actor exists on exactly one string-named `Layer`. Collision, pathfinding and broadcast
  are partitioned per layer (`IActor.Layer`, `Map._pathfinders`).
- The layer name is sent as a string on almost every position packet (`PLAYER_DATA`,
  `PLAYER_JOINED`, `POSITION_UPDATE`, `MAP_ITEM_SPAWN`).
- `LayerModel.ZIndex = LayerIndex * PARTS_PER_LAYER` is a `SpriteBatch` depth hack. The codebase
  treats "layer" as both a render sort key and a simulation partition. Those two meanings must be
  separated before a real Y axis can absorb them.

**Client rendering**

- The entire frame is one `SpriteBatch.Begin(FrontToBack, camera matrix)` in
  `ClientBase.Draw`; all ordering is `layerDepth` floats.
- `IActor` on the client mandates `SpriteSheet`, `Penumbra.Light`, `Emitter` and
  `Draw(SpriteBatch)`. The gameplay classes are the render classes.
- Penumbra is hardwired, not behind an interface; lights are constructed during packet
  deserialization (`Tile.Unpack`, `Player.Unpack`). This is also what blocks the mobile build.
- The GUI (`Lunar.Client/GUI`) draws with `SpriteBatch` directly and re-enters the caller's batch
  in `WidgetCollection.Begin/End`. The XML layout format and widget event model are reusable; the
  drawing is not.
- Input is read inline from `Keyboard.GetState()` / `Mouse.GetState()` in gameplay code.
- `Camera` is a 2D translate/rotate/scale matrix clamped to map bounds.
- Textures are loaded from raw PNG files by server-supplied path; the MonoGame content pipeline is
  used only for fonts and audio.

### What is already dimension-agnostic

These survive a 3D port with little or no change:

- Server tick/heartbeat, `GameTimer*`, `ActorStateMachine`, `ActionProcessor`.
- LiteNetLib transport wrappers and the `Packet` buffer.
- The whole Roslyn scripting host (`ScriptCompiler`, `ScriptHost`, `ScriptRegistry`,
  behaviour attributes, hot reload). Only the sample scripts touch coordinates.
- Stats, items, inventory, equipment, classes, roles, auth, chat, commands, dialogue.
- The `IDataManager` / `FSDataFactory` pattern and the JSON DTO approach.
- The editor stack (Contracts / Core / Api / Web) and its milestone plan. `ContentIndexer`
  already classifies `.map` nodes; nothing opens them yet.

### Pre-existing issues that become MMORPG blockers

Not caused by 2D, but each one must be fixed before real players are on a server:

1. **No synchronization between threads.** Packet handlers mutate world state on the net thread
   while the world thread iterates the same collections. `WorldDictionary` only copies on
   enumerate. The scripting host is the one correctly synchronized component.
2. **No area of interest.** `Map.SendPacket` broadcasts to every player on the map; join sends
   the entire map grid. Cost is O(players squared) per map.
3. **Persistence gaps.** Inventory, equipment and experience are never serialized. There is no
   autosave; save happens only on logout and on a graceful shutdown path that nothing triggers.
4. **Single process, single world thread.** All maps update sequentially on one core.
5. **Pathfinder is stateful and non-reentrant**, so it cannot be shared across threads.
6. Smaller bugs found in passing: `ItemManager` roots its data manager at the NPC path;
   `Inventory.Add` stacks by `GetType()` so all items merge; `NPC.ProcessMovement` compares Y to
   X; `Player.Alive` uses `>= 0`; `PlayerFSDataManager` saves by `Name` but loads by `Username`;
   `WebCommunicator.Run` is dead code; six `PacketType` members are unreferenced.

## The Client Renderer Decision

Everything in the server and data model can proceed without this decision, but the client
cannot. The guiding constraint is that **Lunar stays the engine**: the client is Lunar's own
code (scenes, world view, entity views, UI, input, networking) and the decision is only about
which library sits underneath it and talks to the GPU. Adopting a whole third-party engine as
the front end (Godot, Unity, or Stride used through Game Studio) was considered and rejected:
it means two asset pipelines, two scene models, two sets of conventions, and a permanent
impedance mismatch at the boundary with `Lunar.Core`.

That framing also clarifies how much is at stake. Whatever backend is chosen, the following
is Lunar's own code and is the same work under every option: scene representation and
culling, camera, material system, glTF model loading (`SharpGLTF.Core` 1.0.6), skeletal
animation and skinning, terrain from heightmaps, lighting and shadows, particle effects, text,
and a 3D-aware UI. The backend determines only the bottom of that stack: device, buffers,
textures, pipelines/state, shaders, render targets, swapchain, plus windowing and input if the
library provides them. Versions below were checked against NuGet on the day of writing.

### Option A: MonoGame 3.8.5 as the graphics backend

The project is on MonoGame 3.8.4.1. The 3.8.5 release (July 2026, 3.8.5.1 on NuGet) replaced
the OpenTK/SharpDX dependencies with a native C++ backend and added **cross-platform Vulkan
and Windows DirectX 12 targets** alongside DesktopGL, plus a new code-centric content
project system and ARM64 support. It still ships no glTF import, no PBR, and only the
72-bone `SkinnedEffect`, so Lunar would write its own model loading and skinning either way.
Shaders are HLSL compiled through MonoGame's effect compiler; there is no compute shader
access, and the feature set is roughly Direct3D 11 class.

- Pro: already integrated; zero new native dependencies; windowing, input, audio,
  `SpriteBatch`/`SpriteFont` for UI overlays and the existing 2D client all keep working
  during the transition; the XNA-style `GraphicsDevice` API is familiar and well documented;
  modern backends now exist; MonoGame Foundation backing.
- Con: the effect compiler pipeline is the single biggest friction point for a custom 3D
  renderer (every shader change goes through the content build); no compute; the `Model`
  content pipeline is not worth using, so the content pipeline's value drops to fonts and audio.

### Option B: NeoVeldrid as the graphics backend

Veldrid was the leading low-level .NET graphics abstraction until its author stopped
publishing in 2023. **NeoVeldrid** (1.2.1 on NuGet) is the maintained fork with native
bindings replaced by Silk.NET and the public API preserved. Backends: Vulkan, Direct3D 11,
OpenGL and OpenGL ES, with macOS through bundled MoltenVK. Shaders are written once in GLSL
and cross-compiled to each backend through SPIR-V at runtime, which removes the offline
shader build step entirely. Compute shaders are supported. `ImGui.NET` has a Veldrid renderer
and `FontStashSharp` can be driven from it for text.

- Pro: a clean, modern, explicit API (command lists, resource sets, pipelines) that maps
  directly onto how a 3D renderer is structured; runtime shader cross-compilation makes
  iteration fast; compute is available for skinning, culling and particles later; MoltenVK
  covers macOS without a Metal backend of Lunar's own.
- Con: it is a fork with a small maintainer base, so the risk is abandonment rather than
  capability; Lunar must assemble windowing and input (Silk.NET.Windowing or SDL), audio
  (Silk.NET.OpenAL or similar) and text itself; no mobile story beyond OpenGL ES.

### Option C: SDL3 GPU through C# bindings

SDL3's GPU API is a new cross-platform abstraction over Vulkan, Direct3D 12 and Metal,
maintained by the SDL project, with shader cross-compilation via SDL_shadercross. Several
C# binding sets exist (`ppy.SDL3-CS` from the osu! team is the most actively published), and
SDL3 also supplies windowing, input and audio in one dependency. MoonWorks, the C# framework
from the FNA ecosystem, is built on it and is a useful reference for how a .NET renderer sits
on SDL3 GPU.

- Pro: the only option with native Metal; backed by a project that will outlive any .NET
  wrapper; one dependency for window, input, audio and GPU; modern API designed for exactly
  this use.
- Con: the youngest option; the C# ecosystem around it (text, UI, samples) is thin; shader
  cross-compilation tooling from .NET is less turnkey than NeoVeldrid's runtime SPIR-V path.

### Option D: Silk.NET raw Vulkan or OpenGL

Silk.NET 2.23 provides direct bindings (Vulkan 1.4, OpenGL, Assimp, windowing, input, OpenAL).
Writing the renderer straight against Vulkan gives maximum control and maximum work; Silk.NET
3.0 is still in preview. This is the right choice only if the renderer is itself the product.
It is noted for completeness and not recommended.

### Option E: Stride used code-only

Stride 4.3 (net10) can be driven without Game Studio through `Stride.CommunityToolkit`
(1.0.2). It brings PBR, animation, Bepu physics and glTF import as libraries. However
code-only content loading is a known gap, code-only development is not fully supported off
Windows, and Stride's entity/scene/asset model would still become the client's model. It
carries the same "engine as front end" cost that ruled out Godot, in a smaller and less
portable package, so it is not recommended under the current constraint.

### Recommendation

**Lunar owns the renderer. Build it as `Lunar.Rendering` against a thin backend interface, and
decide the first backend with a short measured spike between MonoGame 3.8.5 and NeoVeldrid
rather than on paper.** The two are the only options that are both mature enough and
compatible with Lunar remaining the engine. The interface is intentionally small (device,
buffer, texture, shader/pipeline, render pass, swapchain) so that the spike is cheap and a
later switch to SDL3 GPU, once its .NET ecosystem matures, is a backend port rather than a
renderer rewrite.

Leaning, before the spike: **MonoGame 3.8.5** has the lower risk and the shortest path to
pixels because it is already in the tree, its new Vulkan/DX12 targets remove the old
"OpenGL-only" ceiling, and the 2D client keeps running on the same device during the
transition. **NeoVeldrid** is the better long-term fit for a custom renderer because of
runtime shader cross-compilation and compute. If the spike shows MonoGame's effect pipeline
is the dominant friction, start on NeoVeldrid. The spike is defined in Milestone 5a.

## Target Architecture

### Coordinate convention

Right-handed, Y up, metres. `X` east, `Y` up, `Z` south. A legacy 2D map maps onto the ground
plane with 2D `Y` becoming 3D `Z`. This convention is encoded in `Vector3.FromGroundPlane` and
`Vector3.ToGroundPlane` and matches the glTF specification's right-handed Y-up convention, so
imported models need no axis swap.

### Core (`Lunar.Core`)

- `Vector3` and `Box` (axis-aligned bounding box) live beside the legacy `Vector` and `Rect`
  during migration. **Done in this branch.**
- `IActorModel.Position` becomes `Vector3`; `CollisionBounds` becomes a `Box` or a capsule
  (radius, height) built by `Box.FromFootprint`.
- `Direction` becomes a facing vector or yaw angle. The enum stays only as a compatibility
  shim for the 2D client until it is retired.
- `MapModel`/`LayerModel`/`TileModel` are replaced by a **`ZoneModel`** authoring document:
  - `Id`, `Name`, `Bounds: Box`
  - `Terrain`: heightmap reference (resolution, cell size, height scale) or a static mesh
    reference, plus a material/splat reference for the client
  - `StaticGeometry[]`: model reference + transform (client rendering, server collision source)
  - `NavMesh`: reference to a baked navmesh asset
  - `SpawnPoints[]`: `Vector3 Position`, yaw, kind (player/NPC), NPC key, respawn timing
  - `Triggers[]`: `Box` volume + kind (warp, dialogue, script event) + parameters
  - `Lights[]`: position, colour, radius, type
  - `Ambient`: sky/fog/time-of-day settings
- Tile attributes (`Blocked`, `Warp`, `NPCSpawn`, `PlayerSpawn`, `StartDialogue`) become
  trigger volumes and spawn points. Collision comes from the navmesh and static geometry, not
  from flags.
- Data managers gain a `ZoneFSDataManager` using the existing JSON DTO pattern. A `schemaVersion`
  field is added, as the editor spec already recommends.

### Server (`Lunar.Server`)

- `Map` becomes `Zone` (or `Region` if multiple zones must share a process). One zone owns a
  navmesh, its actors, its triggers and spawners, and its interest-management grid.
- **Navigation and movement validation via DotRecast** (`DotRecast.Detour`, 2026.3.1 on NuGet),
  the C# port of Recast/Detour. Player movement is validated by projecting the requested
  position onto the navmesh (`findNearestPoly`, `moveAlongSurface`); NPC pathing uses Detour
  path queries. The same bake is shipped to the client for local movement prediction, so
  the server and client agree on walkable space.
- Height comes from the navmesh polygon or the terrain heightmap; the server does not run a full
  physics engine. If richer collision is needed later, Bepu (already used by Stride) is the
  .NET candidate.
- Interest management: a uniform grid of cells per zone (cell size around 2x view distance).
  Broadcasts go to players in the neighbouring cells. Entity enter/leave packets replace the
  full-map dump on join.
- Movement protocol: client sends input (move vector, yaw, sequence number) at a fixed rate;
  server simulates, sends authoritative snapshots at 10 to 20 Hz with the last processed
  sequence number; client predicts locally and reconciles; remote entities are interpolated from
  snapshots. This replaces `PLAYER_MOVING` intent transitions and `NPC_MOVING` waypoint dumps.
- Threading: a single writer per zone. Packet handlers enqueue commands to the owning zone
  instead of mutating state on the net thread. Zones can then be scheduled across a thread pool.
- Persistence: add inventory/equipment/experience to `PlayerDto`, periodic autosave, and a
  shutdown signal handler. Consider SQLite via `Microsoft.Data.Sqlite` for accounts and
  characters while keeping content as JSON files.

### Client

The client stays in this repository and stays Lunar's code. The work splits into a renderer
that does not know about gameplay and a game layer that does not know about the GPU:

- **`Lunar.Rendering`** (new project): the 3D renderer. Scene graph with frustum culling,
  camera, material system, glTF loading via `SharpGLTF.Core`, skeletal animation and GPU
  skinning, heightmap terrain with splat materials, directional light plus shadow maps and a
  small number of local lights, particle emitters, text and 2D overlay for UI. It talks to the
  GPU only through **`Lunar.Rendering.Abstractions`**, a small interface set: `IGraphicsDevice`,
  buffers, textures, samplers, shader programs and pipeline state, render passes and targets,
  swapchain. Shaders are authored once in a single source language and translated per backend
  (HLSL through MonoGame's compiler, or GLSL through SPIR-V cross-compilation on NeoVeldrid).
- **Backends**: `Lunar.Rendering.MonoGame` and/or `Lunar.Rendering.Veldrid`, one class per
  abstraction, no engine logic. The spike in Milestone 5a builds the same test scene on both.
- **`Lunar.Client`** (refactored): `IActor` loses `Draw`, `SpriteSheet`, `Light` and `Emitter`;
  those move to an `IEntityView` created by a view factory from the actor's descriptor. Penumbra
  is removed with the 2D world view rather than abstracted. Input moves behind an input service
  so gameplay stops calling `Keyboard.GetState()` directly. The XML GUI layouts and widget
  event model are kept; widgets render through `Lunar.Rendering`'s overlay path instead of
  `SpriteBatch`.
- **Client-side simulation**: local player controller with prediction and reconciliation
  against server snapshots; remote entity interpolation; navmesh projection for local movement
  using the same DotRecast bake the server uses.
- **Camera**: third person orbit follow; the existing 2D lerp follow is the reference for feel.

### Editor (`Lunar.Tools.Editor`)

The spec's Milestones 6 and 7 (tile map editor) are replaced by a **zone editor**:

- Contracts: `ZoneEditorDocument` mirroring `ZoneModel`.
- Core: `IZoneRepository`, validation rules (spawn inside bounds, trigger references resolve,
  navmesh present).
- Api: `/api/zones` CRUD plus `/api/assets/models` and `/api/assets/terrains` discovery.
- Web: a Three.js (or Babylon.js) viewport for placing spawn points, trigger volumes and lights
  over the imported terrain/mesh, with a properties panel. Heavy authoring (terrain sculpting,
  modelling, rigging) is expected to happen in Blender and be imported as glTF and heightmap
  images; the web editor edits gameplay data over the result. A native Lunar zone editor built
  on `Lunar.Rendering` is a later option once the renderer exists, since it would render zones
  exactly as the client does.

## Milestone Plan

Each milestone leaves the solution building and the existing 2D client working until Milestone
5 explicitly retires it.

### Milestone 0: Foundations (done in this branch)

- `Vector3` and `Box` in `Lunar.Core.Utilities.Data` with the coordinate convention documented.
- `Packet.Write(Vector3)/ReadVector3()` and `Write(Box)/ReadBox()`.
- `Lunar.Core.Tests` xunit project added to the solution with round-trip tests for the wire
  helpers and unit tests for the new math.
- This document.

### Milestone 1: 3D actor model behind the existing 2D behaviour

- `IActorModel.Position` becomes `Vector3`; `Vector` accessors remain as `ToGroundPlane()`
  shims so the 2D server and client keep working.
- Player/NPC DTOs gain a `Z`/`Y` height field with a default.
- Position packets carry `Vector3`; the 2D client reads and drops height.
- Replace the string layer name on the wire with `LayerIndex` (already serialized in
  `MAP_DATA`) as a step toward removing layers.
- Packet round-trip tests for every position-bearing packet.

Exit: 2D game still plays end to end; all positions are 3D in Core, Server and on the wire.

### Milestone 2: Zone model and data manager

- `ZoneModel` and `ZoneFSDataManager` in Core with schema version.
- A converter that turns an existing `.map` into a flat `ZoneModel` (bounds from dimensions,
  blocked tiles into static collision boxes, tile attributes into spawn points and triggers).
  This preserves test content and validates the model.
- Server can load zones alongside maps behind a config flag.

Exit: a converted zone loads on the server with spawns and warps working via triggers.

### Milestone 3: Navigation and movement

- Add `DotRecast.Detour` to the server. Bake a navmesh for the converted flat zone (a heightmap
  at zero height with blocked boxes as obstacles) using `DotRecast.Recast`.
- Replace `Pathfinder` with Detour queries; replace `Layer.CheckCollision` with navmesh
  projection.
- Introduce the input/snapshot movement protocol with sequence numbers.
- Interest-management grid and enter/leave packets; remove the full-map dump.
- Command queue per zone; packet handlers stop touching world state directly.

Exit: NPCs path on the navmesh; players move by input packets with server reconciliation;
join no longer sends the whole map.

### Milestone 4: Persistence and server hardening

- Inventory/equipment/experience in `PlayerDto`; autosave; shutdown handler.
- Fix the bugs listed above.
- Zone scheduling across a thread pool with one writer per zone.
- Load test harness: headless bot clients using `Lunar.Core` + LiteNetLib.

Exit: 200 bots on one zone with stable tick time and no data loss on restart.

### Milestone 5a: Renderer spike (backend decision)

Time-boxed to about two weeks. Build the identical test scene twice, once on MonoGame 3.8.5
(DesktopGL and the Vulkan target) and once on NeoVeldrid 1.2.1, both behind the same
`Lunar.Rendering.Abstractions` interfaces:

- heightmap terrain with a two-layer splat material and a directional light with a shadow map,
- one glTF skinned character loaded with `SharpGLTF.Core`, playing an animation clip,
- 500 instanced static props with frustum culling,
- a text overlay with frame time.

Record for each: lines of backend code, shader iteration loop time, frame time at 1080p,
platform coverage achieved, and any wall hit. Decide the first backend from those numbers
and record the decision in this document. The abstraction layer and the shared renderer code
are kept regardless of the outcome.

### Milestone 5b: 3D client vertical slice

- `Lunar.Rendering` grows from the spike into the renderer described above.
- `Lunar.Client` refactor: `IEntityView`, input service, GUI on the overlay path, Penumbra and
  the 2D world view removed.
- Load a zone (terrain + static meshes + lights), connect, log in, spawn, walk with prediction,
  see other players interpolated, chat.
- Basic UI: login, chat, target frame.

Exit: two clients walk around one zone together. The 2D world view is retired.

### Milestone 6: Zone editor

- Contracts, repository, API and Three.js viewport as described above.
- Replaces editor spec Milestones 6 and 7.
- Optionally a native placement tool on `Lunar.Rendering` once the renderer is stable.

### Milestone 7: Gameplay breadth

- Combat (server-side ability system replacing the stub melee action), spells (descriptors exist
  but no server implementation), NPC behaviours in 3D (aggro by `PlanarDistance`, leash, patrol
  paths), loot, quests.

## Explicit Open Decisions

1. **Graphics backend**: MonoGame 3.8.5 or NeoVeldrid, decided by the Milestone 5a spike;
   SDL3 GPU revisited when its .NET ecosystem matures. Third-party engines as the front end
   are ruled out. Blocks Milestone 5b only.
2. **Zone size and sharding**: one large seamless world with streaming, or discrete zones with
   warp triggers. The plan assumes discrete zones first; seamless streaming is an extension of
   the interest grid.
3. **Terrain representation**: heightmap (cheap, editable in the web editor) versus arbitrary
   mesh (flexible, authored in Blender). The `ZoneModel` allows either; pick heightmap for the
   first slice.
4. **Server physics**: navmesh-only movement (recommended for MMO scale) versus a full physics
   engine (Bepu).
5. **Account storage**: stay on JSON files or move to SQLite/Postgres before Milestone 4.
6. **Fate of `Lunar.Editor` and `Lunar.UnitTests`**: both are unbuildable and outside the
   solution. Recommend deleting them once the web editor covers NPCs and dialogues; the legacy
   map editor code is not relevant to a zone editor.

## Guardrails for Agents Working on This Plan

- Do not remove `Vector`/`Rect` until the 2D world view is retired in Milestone 5b. Add, then migrate,
  then delete.
- Every change to a `Pack`/`Unpack` pair must come with a round-trip test in `Lunar.Core.Tests`
  or a server test project. The wire format has no field tags; the compiler will not catch a
  mismatch.
- Keep `Lunar.Core` free of graphics dependencies. Renderer-facing types (`Vector3`, `Box`,
  descriptors) stay engine-neutral; backend types belong in `Lunar.Rendering.*` only.
- Nothing above `Lunar.Rendering.Abstractions` may reference a backend library. If a feature
  needs a backend-specific call, extend the abstraction rather than leaking the type upward.
- Do not add tile-specific concepts to `ZoneModel`. If a feature needs a grid (for example
  building placement), model it as a component of the zone, not as the zone.
- Server owns truth. The client never sends a position, only inputs.
