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
- **Decisions taken**: Lunar stays the engine and owns its renderer; no third-party engine is
  used as the front end; **MonoGame 3.8.5 is the graphics backend**; the visual target is
  **retro 3D with modern spins**, which caps the renderer's scope. The project is unreleased,
  so there are no compatibility or transition constraints: 2D code is deleted rather than
  shimmed.
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
- **Client-side simulation**: local player controller with prediction and reconciliation
  against server snapshots; remote entity interpolation; navmesh projection for local movement
  using the same DotRecast bake the server uses.
- **Camera**: third person orbit follow; the existing 2D lerp follow is the reference for feel.
- **Platforms**: the native backend (Vulkan on Linux/macOS via MoltenVK where applicable,
  DirectX 12 on Windows) as primary and DesktopGL as the compatibility target. Mobile is not a
  goal; the `Lunar.Client.Mobile` stub is deleted.

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

Each milestone leaves the solution building and the server tests passing. The project is
unreleased, so the 2D client is not kept working: 2D code is removed as soon as the 3D
replacement for that piece exists, and shims are avoided.

### Milestone 0: Foundations (done in this branch)

- `Vector3` and `Box` in `Lunar.Core.Utilities.Data` with the coordinate convention documented.
- `Packet.Write(Vector3)/ReadVector3()` and `Write(Box)/ReadBox()`.
- `Lunar.Core.Tests` xunit project added to the solution with round-trip tests for the wire
  helpers and unit tests for the new math.
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

### Milestone 5a: Renderer proof scene

Time-boxed to about two weeks. Stand up `Lunar.Rendering` on MonoGame 3.8.5 with the native
backend and DesktopGL and render one scene that exercises every item in the retro feature list:

- heightmap terrain with a two-layer splat material, distance fog and a directional shadow map,
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
- Load a zone (terrain + static meshes + lights), connect, log in, spawn, walk with prediction,
  see other players interpolated, chat.
- Basic UI: login, chat, target frame.

Exit: two clients walk around one zone together.

### Milestone 6: Zone editor

- Contracts, repository, API and Three.js viewport as described above.
- Replaces editor spec Milestones 6 and 7.
- Optionally a native placement tool on `Lunar.Rendering` once the renderer is stable.

### Milestone 7: Gameplay breadth

- Combat (server-side ability system replacing the stub melee action), spells (descriptors exist
  but no server implementation), NPC behaviours in 3D (aggro by `PlanarDistance`, leash, patrol
  paths), loot, quests.

## Explicit Open Decisions

1. **Graphics backend**: decided, MonoGame 3.8.5. NeoVeldrid is the fallback only if the API
   ceiling is hit; third-party engines as the front end are ruled out.
2. **Zone size and sharding**: one large seamless world with streaming, or discrete zones with
   warp triggers. The plan assumes discrete zones first; seamless streaming is an extension of
   the interest grid.
3. **Terrain representation**: heightmap (cheap, editable in the web editor) versus arbitrary
   mesh (flexible, authored in Blender). The `ZoneModel` allows either; pick heightmap for the
   first slice.
4. **Server physics**: navmesh-only movement (recommended for MMO scale) versus a full physics
   engine (Bepu).
5. **Account storage**: stay on JSON files or move to SQLite/Postgres before Milestone 4.
6. **Fate of `Lunar.Editor` and `Lunar.UnitTests`**: decided, delete in Milestone 1. Both are
   unbuildable and outside the solution, and the legacy map editor code is not relevant to a
   zone editor. NPC and dialogue editing arrive in the web editor per its own spec.

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
- Do not add tile-specific concepts to `ZoneModel`. If a feature needs a grid (for example
  building placement), model it as a component of the zone, not as the zone.
- Server owns truth. The client never sends a position, only inputs.
