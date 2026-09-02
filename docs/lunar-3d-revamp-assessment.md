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
  cell/navmesh based server simulation, a 3D wire protocol, and a world editor. That work is the
  same whichever 3D client technology is chosen, so it should start first.
- **Decisions taken**: Lunar stays the engine and owns its renderer; no third-party engine is
  used as the front end; **MonoGame 3.8.5 is the graphics backend**; the visual target is
  **retro 3D with modern spins**, which caps the renderer's scope. The world is a **seamless,
  streamed open world** built from fixed-size cells, terrain is **heightmap plus glTF set
  pieces**, server movement is **navmesh only**, and accounts live in **PostgreSQL**. The
  project is unreleased, so there are no compatibility or transition constraints: 2D code is
  deleted rather than shimmed.
- Several pre-existing server issues (no synchronization between the net and world threads,
  no area-of-interest filtering, inventory not persisted, no autosave) are not 3D problems but
  they become MMORPG blockers. They are listed so they can be scheduled alongside the revamp.

## What Exists Today

### Solution layout (net9.0 unless noted)

| Project | Role | Key dependencies |
|---|---|---|
| `Lunar.Core` | Shared data model, descriptors, JSON data managers, packet buffer | `Microsoft.Extensions.DependencyInjection.Abstractions` only |
| `Lunar.Server` | Authoritative simulation, LiteNetLib transport, Roslyn scripting | LiteNetLib 2.1.3, Microsoft.CodeAnalysis.CSharp 4.12 |
| `Lunar.Graphics` | Sprite, SpriteSheet, animation, MonoGame interop | MonoGame.Framework.DesktopGL 3.8.5.1 (upgraded in this branch) |
| `Lunar.Client` | Scenes, world view, GUI, client net | MonoGame DesktopGL 3.8.5.1, Penumbra 3.0.0, LiteNetLib |
| `Lunar.Client.Desktop` | Executable, Penumbra wiring, dev console, content | `Lunar.Client` |
| `Lunar.Client.Mobile` | Stub. Not buildable; does not reference `Lunar.Client` | MonoGame Android/iOS |
| `Lunar.Tools.Editor.{Contracts,Core,Api,Web}` | Local-first web editor (items, spells, scripts) | ASP.NET Core minimal API, React 19, Vite, Monaco |
| `Lunar.Editor` | Legacy WinForms editor. .NET Framework 4.8, IronPython, **not in the solution** | DarkUI, MonoGame 3.7 |
| `Lunar.UnitTests` | Legacy MSTest project. .NET Framework 4.8, **not in the solution**, references removed APIs | |
| `Lunar.Core.Tests` | **New in this branch.** xunit, net9.0 | |

All solution projects build with zero errors on Linux with the .NET 10 SDK; `Lunar.Client` carries a handful of pre-existing unused-member warnings.

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

The guiding constraint is that **Lunar stays the engine**: the client is Lunar's own code
(scenes, world view, entity views, UI, input, networking) and the only question was which
library talks to the GPU underneath it. Adopting a whole third-party engine as the front end
(Godot, Unity, or Stride through Game Studio) was rejected: two scene models, two asset
pipelines, two sets of conventions, and a permanent impedance mismatch at the boundary with
`Lunar.Core`. Godot in particular was judged a poor fit because its .NET layer is a guest in
a GDScript-first engine and its editor would displace the tooling this project wants to own.

Whatever the backend, the renderer itself is Lunar's code: scene representation and culling,
camera, materials, glTF loading (`SharpGLTF.Core` 1.0.6), skeletal animation and skinning,
heightmap terrain, lighting and shadows, particles, text and UI. The backend only supplies
device, buffers, textures, pipeline state, shaders, render targets and the swapchain.

### Decision: MonoGame 3.8.5

MonoGame is already in the tree and its 3.8.5 release (July 2026; `3.8.5.1` on NuGet, and
this branch is upgraded to it) replaced the OpenTK/SharpDX dependencies with a native C++
backend (`MonoGame.Framework.Native`) that adds cross-platform Vulkan and Windows DirectX 12
targets alongside DesktopGL and WindowsDX. The common claim that "MonoGame is built on an old
OpenGL renderer" is therefore out of date at the backend level. What remains true is that the
**API surface is XNA 4**: no compute shaders in the official release (they exist only in a
community fork), no storage buffers or indirect draws, and a fixed `Effect` model. For the
retro-with-modern-spins target below none of those gaps is blocking. The alternatives that
were weighed and their standing:

- **NeoVeldrid** (maintained Veldrid fork; Vulkan/D3D11/GL, runtime GLSL to SPIR-V, compute).
  Technically the cleaner fit for a custom renderer, rejected in favour of the lower-risk,
  already-integrated option. Remains the fallback if MonoGame's API ceiling is ever hit.
- **SDL3 GPU** via C# bindings. Native Metal and strong upstream backing, but the .NET
  ecosystem around it is young. Watch item.
- **Raw Silk.NET Vulkan/OpenGL**. Maximum work; only appropriate if the renderer is the
  product.
- **Stride code-only**. Same engine-as-front-end cost that ruled out Godot, in a Windows-centric
  package.

### Known MonoGame headaches and their mitigations

The bet behind this decision is that every MonoGame limitation relevant to this project has a
workable answer. They are listed so none of them is a surprise later.

| Headache | Mitigation |
|---|---|
| Shaders are HLSL compiled offline by the effect compiler; 3.8.5 dropped automatic Wine relaunching and out-of-the-box DirectX shader compilation on Linux/macOS | Author shaders on Windows or in CI; keep the shader count small (retro target needs roughly a dozen); use `MonoGame.Tool.Dxc` for the native/Vulkan path; add a `dotnet build` target that rebuilds only changed `.fx` files so iteration is one command |
| No compute shaders | Skinning in the vertex shader with bone matrices in a constant buffer or a bone texture; particles simulated on the CPU or as vertex-shader-animated quads; culling on the CPU. All appropriate at retro scale |
| No glTF import, weak `Model` content pipeline | Load glTF at runtime with `SharpGLTF.Core` and build `VertexBuffer`/`IndexBuffer` directly; do not use the `Model` class or the model content processors at all |
| `SkinnedEffect` capped at 72 bones and fixed lighting | Never use the stock effects for world rendering; write Lunar's own effects (unlit, vertex-lit, skinned, terrain, post) |
| `Effect` parameter updates are per-parameter and comparatively slow | Group per-frame and per-object constants; sort draws by material; instance static props; retro scene sizes keep draw counts low |
| Content pipeline changed to a code-centric project system in 3.8.5 | The client already loads textures from raw files; keep the pipeline for fonts, audio and effects only, and adopt the new project system when the 2D `Content.mgcb` is removed |
| Texture arrays, MRT and some formats vary by backend | Target the native backend (Vulkan/DX12) as primary and DesktopGL as compatibility; feature-check at startup and disable optional effects rather than branching everywhere |
| Penumbra and other 2D-only dependencies | Deleted with the 2D world view; nothing 3D depends on them |

### Retro renderer scope

Fixing the visual target is what keeps a custom renderer bounded. The following is the
renderer's feature list; anything outside it is a deliberate later addition.

Retro base:

- Low-poly meshes with small palettes, nearest-neighbour texture sampling, optional per-vertex
  lighting or flat shading.
- Optional PS1-style effects as switches: vertex position snapping, affine texture mapping,
  reduced-precision colour with ordered dithering, distance fog.
- Fixed internal resolution (for example 640x360 or 960x540) upscaled with integer scaling.

Modern spins:

- Directional light with a shadow map (single cascade to start) and a small number of point
  lights.
- Post-processing chain on render targets: bloom, colour grading LUT, screen-space outlines,
  vignette, the dither/quantise pass above.
- Hardware instancing for props, frustum culling, distance-based LOD as a mesh swap.
- Skeletal animation with clip blending, GPU skinning in the vertex shader.
- Heightmap terrain with a splat material and simple water.
- Particle effects as CPU-simulated billboards.

Explicitly out of scope until proven necessary: PBR, screen-space reflections or ambient
occlusion, global illumination, volumetrics, cascaded shadow maps beyond one or two cascades,
compute-based anything.

## Target Architecture

### Coordinate convention

Right-handed, Y up, metres. `X` east, `Y` up, `Z` south. A legacy 2D map maps onto the ground
plane with 2D `Y` becoming 3D `Z`. This convention is encoded in `Vector3.FromGroundPlane` and
`Vector3.ToGroundPlane` and matches the glTF specification's right-handed Y-up convention, so
imported models need no axis swap.

### Core (`Lunar.Core`)

- `Vector3` and `Box` (axis-aligned bounding box) replace `Vector` and `Rect` for world
  positions and volumes. **Added in this branch**; the 2D types are deleted once their last
  user is gone.
- `IActorModel.Position` becomes `Vector3`; `CollisionBounds` becomes a `Box` or a capsule
  (radius, height) built by `Box.FromFootprint`.
- `Direction` is replaced by a yaw angle plus a facing-vector helper; the enum is deleted.
- `MapModel`/`LayerModel`/`TileModel` are replaced by a **world of cells**. There is one
  global coordinate space; the world is partitioned into fixed-size square cells (working
  value: 128 m, tunable in the world manifest). Authoring documents:
  - `WorldModel` (one per world): `Id`, `Name`, `CellSize`, `Bounds: Box` (whole-world extent),
    `SchemaVersion`, global `Ambient` defaults (sky, fog, time-of-day), and the list of
    populated cell coordinates.
  - `CellModel` (one JSON file per populated cell, addressed by integer `(cx, cz)`):
    - `Terrain`: heightmap tile reference (a 16-bit grayscale image covering exactly this cell,
      plus height scale) and a splat/material reference. Cells share edge rows so seams match.
    - `StaticGeometry[]`: glTF model reference + transform for set pieces (caves, cliffs,
      buildings, overhangs). Rendered by the client; voxelised into the navmesh bake on the
      server side.
    - `NavMeshTile`: reference to this cell's baked Detour tile(s).
    - `SpawnPoints[]`: `Vector3 Position`, yaw, kind (player/NPC), NPC key, respawn timing.
    - `Triggers[]`: `Box` volume + kind (script event, dialogue, teleport for instances) +
      parameters. Warps between regions of the open world are not needed; teleports remain for
      dungeons or instanced content.
    - `Lights[]`: position, colour, radius, type. `AmbientOverride` for local fog/sky changes.
  - `Region` is an editor-only grouping of cells (a named rectangle) for authoring and for
    server thread scheduling; it has no runtime data of its own.
- All positions are stored in global world coordinates as `float`. World extent is capped at
  16 km per axis so that `float` keeps sub-centimetre precision everywhere; the client renders
  relative to a floating origin (see Client) so GPU precision is never an issue.
- Tile attributes (`Blocked`, `Warp`, `NPCSpawn`, `PlayerSpawn`, `StartDialogue`) become
  trigger volumes and spawn points. Collision comes from the navmesh, not from flags.
- Data managers gain `WorldFSDataManager` and `CellFSDataManager` using the existing JSON DTO
  pattern, with `schemaVersion` fields.

### Server (`Lunar.Server`)

- **One world, many cells.** `Map` is replaced by a `World` that owns a sparse grid of loaded
  `Cell`s. A cell holds its navmesh tile, static geometry bounds, resident actors, triggers,
  spawners and ground items. Cells load on demand when a player approaches and unload after an
  idle timeout, so the whole world is never resident at once.
- **Navigation and movement validation via DotRecast** (`DotRecast.Detour` and
  `DotRecast.Recast`, 2026.3.1 on NuGet; ZLib licence). The navmesh is a **tiled Detour mesh**
  with one Detour tile per world cell, baked offline by a `Lunar.Tools.NavBake` command-line
  tool from the cell's heightmap plus voxelised static geometry, and added/removed at runtime
  with the cell. Player movement is validated by projecting the requested position onto the
  navmesh (`findNearestPoly`, `moveAlongSurface`); NPC pathing uses Detour path queries, which
  cross tile boundaries natively. `Detour.Dynamic` is available later for doors and destructible
  obstacles.
- **Navmesh only.** The server runs no physics engine. Height comes from the navmesh polygon.
  Jumping is an animation with a scripted vertical arc over a navmesh-validated horizontal
  path; knockback is a navmesh-constrained displacement; projectiles and areas of effect are
  geometric tests, not rigid bodies. If a feature ever needs real collision response, a
  kinematic capsule-sweep layer is the next step, not a physics engine.
- **Interest management is the cell grid.** A player receives state from their own cell and
  the eight neighbours (a 3x3 window; the client streams the same window of terrain).
  Crossing a cell boundary emits enter/leave packets for entities that fall out of or into the
  window. Broadcasts never address "the map"; they address a cell window.
- **Cross-cell handoff.** Within one process an actor moving between cells is a dictionary
  move. Cells are grouped into regions, each region driven by one worker thread with a
  single-writer rule; an actor crossing a region boundary is handed off through a command
  queue at the tick boundary. Cross-process sharding by region is the later extension, and is
  why accounts live in PostgreSQL from the start.
- **Movement protocol.** Client sends input (move vector, yaw, jump/action flags, sequence
  number) at a fixed rate; the server simulates, then sends authoritative snapshots at 10 to
  20 Hz with the last processed sequence number; the client predicts locally and reconciles;
  remote entities are interpolated from snapshots. This replaces `PLAYER_MOVING` intent
  transitions and `NPC_MOVING` waypoint dumps.
- **Threading.** Packet handlers enqueue commands to the owning region instead of mutating
  state on the net thread. Regions are scheduled across a thread pool each tick.
- **Persistence in PostgreSQL** via `Npgsql.EntityFrameworkCore.PostgreSQL` (10.x) with EF
  Core migrations: accounts, characters (stats, position, cell, inventory, equipment,
  experience), and later guilds, mail and auction data. Autosave dirty characters on a timer
  and on logout; a shutdown signal handler flushes everything. World content (world manifest,
  cells, items, NPCs, spells, dialogues) stays as JSON files under version control. A
  `docker-compose.yml` provides the development database.

### Client

The client stays in this repository and stays Lunar's code. The work splits into a renderer
that does not know about gameplay and a game layer that does not know about the GPU:

- **`Lunar.Rendering`** (new project, references MonoGame directly): the 3D renderer scoped by
  the retro feature list above. Scene graph with frustum culling, camera, material system,
  glTF loading via `SharpGLTF.Core`, skeletal animation and vertex-shader skinning, heightmap
  terrain, directional light plus shadow map and a few point lights, particle billboards, a
  post-processing chain, and a text/2D overlay path for UI. Lunar's own `.fx` effects live here.
  No gameplay types and no networking.
- **`Lunar.Client`** (rebuilt): `IActor` loses `Draw`, `SpriteSheet`, `Light` and `Emitter`;
  an `IEntityView` created by a view factory from the actor's descriptor owns presentation.
  Input moves behind an input service so gameplay stops calling `Keyboard.GetState()` directly.
  The XML GUI layouts and widget event model are kept; widgets render through
  `Lunar.Rendering`'s overlay path. Penumbra, `Lunar.Graphics`, the 2D world view and the
  2D `Content.mgcb` are deleted, not abstracted.
- **World streaming**: the client keeps the 3x3 cell window around the player loaded
  (terrain tile, set pieces, lights, navmesh tile), prefetching the next ring as the player
  nears a boundary. Terrain uses a skirt or shared edge rows so cell seams never show.
- **Floating origin**: world positions are `float` in global coordinates; the renderer
  re-bases everything relative to the camera's cell origin each frame so GPU-side precision
  is constant regardless of where the player is in the world.
- **Client-side simulation**: local player controller with prediction and reconciliation
  against server snapshots; remote entity interpolation; navmesh projection for local movement
  using the same DotRecast tiles the server uses.
- **Camera**: third person orbit follow; the existing 2D lerp follow is the reference for feel.
- **Platforms**: the native backend (Vulkan on Linux/macOS via MoltenVK where applicable,
  DirectX 12 on Windows) as primary and DesktopGL as the compatibility target. Mobile is not a
  goal; the `Lunar.Client.Mobile` stub is deleted.

### Editor (`Lunar.Tools.Editor`)

The spec's Milestones 6 and 7 (tile map editor) are replaced by a **world editor** that edits
cells:

- Contracts: `WorldManifestDocument` and `CellEditorDocument` mirroring the Core models.
- Core: `IWorldRepository`, `ICellRepository`, validation rules (spawn inside the cell, trigger
  references resolve, heightmap tile dimensions match the cell size, navmesh tile present and
  not stale relative to its inputs).
- Api: `/api/world`, `/api/cells/{cx}/{cz}` CRUD, `/api/assets/models`,
  `/api/assets/heightmaps`, and `/api/navmesh/bake` which shells out to `Lunar.Tools.NavBake`.
- Web: a Three.js (or Babylon.js) viewport that shows the selected cell and its neighbours,
  for placing set pieces, spawn points, trigger volumes and lights over the terrain, with a
  properties panel and a world overview grid for jumping between cells. Heightmap sculpting
  and splat painting are in scope for the web editor because heightmaps are images; mesh
  modelling and rigging stay in Blender and arrive as glTF.
- A native Lunar placement tool built on `Lunar.Rendering` is a later option, since it would
  render cells exactly as the client does.

## Milestone Plan

Each milestone leaves the solution building and the server tests passing. The project is
unreleased, so the 2D client is not kept working: 2D code is removed as soon as the 3D
replacement for that piece exists, and shims are avoided.

### Milestone 0: Foundations (done in this branch)

- `Vector3` and `Box` in `Lunar.Core.Utilities.Data` with the coordinate convention documented.
- `Packet.Write(Vector3)/ReadVector3()` and `Write(Box)/ReadBox()`.
- `Lunar.Core.Tests` xunit project added to the solution with round-trip tests for the wire
  helpers and unit tests for the new math.
- The testing pipeline described in `lunar-testing-pipeline.md`: unit tests, end-to-end tests
  that run a real server and a real rendering client under Xvfb with screenshot analysis, and
  the Docker/CI wiring for the Docker host. Its first run found and fixed thirteen engine
  defects (Linux-only crashes, script loading, identity handling, auth messaging).
- This document.

### Milestone 1: 3D actor model

- `IActorModel.Position` becomes `Vector3`; `CollisionBounds` becomes a `Box` built by
  `Box.FromFootprint`; `Direction` is replaced by a yaw angle plus a facing helper.
- Player/NPC DTOs and their data managers move to `Vector3`.
- Position packets carry `Vector3`; the string layer name is removed from the wire.
- Packet round-trip tests for every position-bearing packet.
- Delete `Lunar.Editor`, `Lunar.UnitTests` and `Lunar.Client.Mobile` (all unbuildable and
  outside the solution or stubs). The 2D client compiles against the new types with minimal
  edits until Milestone 5 removes it.

Exit: all positions are 3D in Core, Server and on the wire; no `Vector`/`Rect` remain in
actor or packet code.

### Milestone 2: World and cell model

- `WorldModel`, `CellModel`, `WorldFSDataManager` and `CellFSDataManager` in Core with schema
  versions.
- A converter that turns an existing `.map` into one or more flat cells (blocked tiles into
  static collision boxes, tile attributes into spawn points and triggers) so existing test
  content survives and validates the model.
- Server `World` with sparse cell loading and unloading; `Map`, `Layer` and `Tile` deleted.

Exit: converted cells load on demand on the server with spawns and triggers working.

### Milestone 3: Navigation, streaming and movement

- `Lunar.Tools.NavBake`: bakes one Detour tile per cell from heightmap plus voxelised glTF set
  pieces using `DotRecast.Recast`; output stored beside the cell.
- Server loads/unloads Detour tiles with cells; `Pathfinder` replaced by Detour queries;
  `Layer.CheckCollision` replaced by navmesh projection.
- Input/snapshot movement protocol with sequence numbers.
- Cell-window interest management with enter/leave packets; the full-map dump is removed.
- Region worker threads with per-region command queues; packet handlers stop touching world
  state directly; actor handoff across region boundaries.

Exit: NPCs path across cell boundaries on the tiled navmesh; players move by input packets with
server reconciliation; walking across the world streams cells in and out with no load screen.

### Milestone 4: Persistence and server hardening

- PostgreSQL via EF Core: accounts, characters with inventory, equipment and experience;
  migrations; `docker-compose.yml` for development; the JSON account files are deleted.
- Autosave on a timer and on logout; shutdown handler that flushes dirty characters.
- Fix the bugs listed above.
- Load test harness: headless bot clients using `Lunar.Core` + LiteNetLib walking across cell
  and region boundaries.

Exit: 200 bots roaming across several regions with stable tick time and no data loss on
restart.

### Milestone 5a: Renderer proof scene

Time-boxed to about two weeks. Stand up `Lunar.Rendering` on MonoGame 3.8.5 with the native
backend and DesktopGL and render one scene that exercises every item in the retro feature list:

- heightmap terrain built from two adjacent cell tiles with a two-layer splat material,
  distance fog and a directional shadow map (proves the seam handling and floating origin),
- one glTF skinned character loaded with `SharpGLTF.Core`, playing a blended animation clip
  with vertex-shader skinning,
- 500 instanced static props with frustum culling,
- the post-processing chain: fixed internal resolution, dither/quantise, bloom, outlines,
- a text overlay with frame time.

Also establish the shader build loop (one command rebuilds changed effects) and record the
per-platform feature check results. Any headache from the table above that turns out to lack a
workable answer is raised here, while the renderer is still small.

Exit: the scene runs on the native backend and DesktopGL with the shader loop documented.

### Milestone 5b: 3D client vertical slice

- `Lunar.Rendering` grows from the proof scene into the renderer described above.
- `Lunar.Client` rebuild: `IEntityView`, input service, GUI on the overlay path; Penumbra,
  `Lunar.Graphics`, the 2D world view and `Content.mgcb` deleted.
- Stream cells (terrain + set pieces + lights) around the player, connect, log in, spawn, walk
  across cell boundaries with prediction, see other players interpolated, chat.
- Basic UI: login, chat, target frame.

Exit: two clients walk across several cells together with no load screens.

### Milestone 6: World editor

- Contracts, repositories, API, navmesh bake endpoint and Three.js viewport as described
  above, including heightmap sculpting and splat painting.
- Replaces editor spec Milestones 6 and 7.
- Optionally a native placement tool on `Lunar.Rendering` once the renderer is stable.

### Milestone 7: Gameplay breadth

- Combat (server-side ability system replacing the stub melee action), spells (descriptors exist
  but no server implementation), NPC behaviours in 3D (aggro by `PlanarDistance`, leash, patrol
  paths), loot, quests.

## Decision Record

Decisions taken so far, with the reasoning that should be revisited if circumstances change.

1. **Graphics backend: MonoGame 3.8.5.** Already integrated, native Vulkan/DX12 backend
   available, every known limitation has a mitigation for the retro target. NeoVeldrid is the
   fallback only if the API ceiling is hit; third-party engines as the front end are ruled out.
2. **World shape: seamless open world from the start.** Chosen over discrete zones despite the
   higher up-front cost (cell streaming, tiled navmesh, region handoff, floating origin) so
   that the world never has to be re-partitioned later. Instanced content (dungeons) uses
   separate small worlds reached by teleport triggers.
3. **Terrain: heightmap plus glTF set pieces.** Heightmaps keep terrain cheap and editable in
   the web editor; set pieces from Blender supply caves, cliffs and interiors; the tiled
   navmesh unifies both for movement.
4. **Server physics: navmesh only.** No physics engine on the server. Jumping, knockback,
   projectiles and areas of effect are scripted or geometric. A kinematic capsule layer is the
   escalation path if ever needed.
5. **Account storage: PostgreSQL from the start**, via Npgsql and EF Core, because
   cross-process sharding by region is anticipated and a shared database is its prerequisite.
   World content stays in JSON files.
6. **Legacy projects: delete in Milestone 1.** `Lunar.Editor`, `Lunar.UnitTests` and the
   mobile stub are unbuildable or empty and not relevant to the new world model.

Still open, to be decided when their milestone starts:

- **Cell size** (working value 128 m) and **world extent cap** (16 km per axis): confirm
  against the intended world size before Milestone 2 fixes the heightmap tile resolution.
- **Snapshot rate and prediction window** for the movement protocol (Milestone 3).
- **Fixed internal resolution** for the retro look (Milestone 5a).
- **Web editor viewport library**: Three.js versus Babylon.js (Milestone 6).

## Guardrails for Agents Working on This Plan

- Prefer deletion over shims. There is no released data or client to protect; when a 2D type
  or code path is replaced, remove it in the same change.
- Every change to a `Pack`/`Unpack` pair must come with a round-trip test in `Lunar.Core.Tests`
  or a server test project. The wire format has no field tags; the compiler will not catch a
  mismatch.
- Keep `Lunar.Core` free of graphics dependencies. Renderer-facing types (`Vector3`, `Box`,
  descriptors) stay engine-neutral; MonoGame types belong in `Lunar.Rendering` and
  `Lunar.Client` only, and `Lunar.Rendering` must not reference gameplay or networking types.
- Stay inside the retro renderer scope. A feature outside it needs a written reason and a
  check that MonoGame's API can carry it before work starts.
- Never use MonoGame's stock effects or `Model` pipeline for world rendering; Lunar owns its
  effects and mesh loading.
- Do not add 2D tile concepts to `CellModel`. Cells are a streaming and scheduling partition,
  not a gameplay grid; if a feature needs a placement grid (for example housing), model it as
  a component inside a cell.
- No load screens inside the open world. Anything that would need one (a huge set piece, a
  slow bake) is a content or tooling problem to solve, not a reason to add a warp.
- Server owns truth. The client never sends a position, only inputs.
