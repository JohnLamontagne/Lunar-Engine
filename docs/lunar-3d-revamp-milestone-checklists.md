# Lunar Engine 3D Revamp: Milestone Checklists

Companion to `lunar-3d-revamp-assessment.md`. That document holds the decisions and the
architecture; this one breaks each milestone into reviewable tasks. Tasks name the classes and
files they touch as of the `modernization` tip so they can be located after refactoring. Each
milestone ends with an exit check that must pass before the next one starts.

Conventions used below:

- Every milestone adds tests at all three layers of `lunar-testing-pipeline.md` (unit,
  end-to-end with screenshot assertions, and bots where relevant) before its exit check.
- **Delete** means remove from the repository in the same change, not deprecate.
- Every task that changes a `Pack`/`Unpack` pair or a data manager DTO includes its round-trip
  test in the same change.
- The 2D desktop client stops being playable at Milestone 2 and returns at Milestone 5b. From
  Milestone 2 onward the server is exercised through tests and the headless bot client.

---

## Milestone 0: Foundations (done)

- [x] `Vector3` in `Lunar.Core/Utilities/Data/Vector3.cs` with the Y-up convention documented.
- [x] `Box` in `Lunar.Core/Utilities/Data/Box.cs`.
- [x] `Packet.Write/Read` for `Vector3` and `Box` in `Lunar.Core/Net/PacketExtensions.cs`.
- [x] `Lunar.Core.Tests` xunit project added to the solution; 20 tests passing.
- [x] Upgrade MonoGame to 3.8.5.1 in `Lunar.Client` and `Lunar.Graphics`.
- [x] Assessment document with decision record.
- [x] Testing pipeline (`lunar-testing-pipeline.md`): client automation endpoint, `Lunar.E2E.Tests`
      launching a real server and Xvfb client with screenshot analysis and goldens,
      `Lunar.Server.Tests`, Docker runner image and compose, CI workflow. Thirteen engine defects
      found and fixed by the first run.

---

## Milestone 1: 3D actor model

Goal: every position in Core, Server and on the wire is 3D; the tile grid is untouched for now.

### Core types

- [ ] `IActorModel.Position` becomes `Vector3`; `Reach` becomes a `float` radius; `CollisionBounds`
      becomes a `Box` produced by `Box.FromFootprint` from `Width`/`Height`/`Depth` on the
      descriptor (`Lunar.Core/World/Actor/Descriptors/IActorModel.cs`).
- [ ] Add `float Yaw` to `IActorModel` and a `Facing` helper that returns the ground-plane unit
      vector for a yaw. Delete `Lunar.Core/World/Direction.cs` and every `Direction` switch.
- [ ] `PlayerModel` and `NPCModel`: `Position` and `Yaw` as above; `NPCModel.MaxRoam` becomes a
      `float` radius; `AggresiveRange` renamed `AggressiveRange` and typed `float`.
- [ ] Introduce `Lunar.Core/Net/Messages/` with one record per position-bearing message and
      `Write(Packet)`/`Read(Packet)` on each: `ActorState`, `PlayerJoined`, `PlayerMoved`,
      `PositionUpdate`, `NpcState`, `NpcMoved`, `MapItemSpawned`, `MapItemDespawned`. Server and
      client serialize through these records instead of hand-written `Pack`/`Unpack`.
- [ ] Remove the layer-name string from every message. Layers cease to exist on the wire.
- [ ] Delete the dead `PacketType` members (`CREATE_CHAR*`, `ALERT_MSG`, `DIAPLAY_ANIMATION`) and
      make `REGISTRATION_FAIL` actually sent, or delete it too.

### Data managers

- [ ] `PlayerFSDataManager`: `PositionX/PositionY` become `X/Y/Z` plus `Yaw`; save path derived
      from the same key as load (fixes the `Name` vs `Username` mismatch).
- [ ] `NPCFSDataManager`: `VectorDto` becomes `Vector3Dto`; `RectDto` for collision becomes
      width/height/depth; `MaxRoam` a float.
- [ ] Round-trip tests for both DTOs in `Lunar.Core.Tests`.

### Server

- [ ] `Player.ProcessMovement`/`CanMove` and `NPC.UpdateMovement`/`CanMove` move by
      `Facing(Yaw) * Speed * dt` on the ground plane instead of a four-way switch
      (`Lunar.Server/World/Actors/Player.cs`, `NPC.cs`). Collision still calls the tile-based
      `Layer.CheckCollision` with the box's ground-plane footprint until Milestone 3.
- [ ] Remove the hard-coded `new Rect(16, 52, 16, 20)` player collision box; build it from the
      descriptor.
- [ ] `NPC.WithinRangeOf` and `NPC.FindTarget<T>` use `Vector3.PlanarDistance` instead of
      rectangle tests. Fix the `targetDest.Y <= this.Position.X` typo while there.
- [ ] `NPC.ProcessMovement` follows a path of `Vector3` waypoints by steering toward the next
      point, not axis by axis.
- [ ] Sample scripts under `Server Data/World/Scripts` updated to the new types.

### Client (minimal, keeps compiling)

- [ ] `Lunar.Client/World/Actors/Player.cs`, `NPC.cs`, `WorldManager.cs` read the new messages and
      project `Vector3` to the ground plane for drawing. No other 2D client work.

### Deletions

- [ ] Delete `src/Lunar.Editor`, `src/Lunar.UnitTests`, `src/Lunar.Client.Mobile` and their
      solution entries.
- [ ] Delete `src/libs/*.dll` (DarkUI, Lidgren, KopiLua, QuakeConsole, ScintillaNET, vendored
      MonoGame and Penumbra); everything comes from NuGet now.
- [ ] Delete the legacy `.xml` twins beside the `.json` files in `Server Data/`.
- [ ] Rewrite `README.md`: 3D engine, project list, build instructions.

### Exit check

- [ ] Solution builds; `Lunar.Core.Tests` green; no `Vector`/`Rect`/`Direction` usage remains in
      `Lunar.Core/World/Actor`, `Lunar.Core/Net`, or `Lunar.Server/World/Actors`.
- [ ] Server starts, a client logs in, spawns and walks in the 2D view using 3D positions.

---

## Milestone 2: World and cell model

Goal: the tile map is gone; the server runs a sparse world of cells loaded on demand.

### Core model

- [ ] `CellCoord` struct (`int X, Z`) with `FromPosition(Vector3, cellSize)`, neighbour
      enumeration, and a `Box Bounds(cellSize)` helper. Tests.
- [ ] `WorldModel`: `Id`, `Name`, `CellSize`, `Bounds`, `SchemaVersion`, `Ambient`, list of
      populated `CellCoord`s, navmesh bake parameters (agent radius/height, max climb, max
      slope).
- [ ] `CellModel`: `Coord`, `Terrain` (heightmap tile path, height scale, splat path),
      `StaticGeometry[]` (model path, `Vector3` position, yaw, scale), `BlockerVolumes[]`
      (`Box[]`, authoring-time no-walk volumes and navmesh bake input), `SpawnPoints[]`,
      `Triggers[]`, `Lights[]`, `AmbientOverride`, `NavMeshTile` path, `SchemaVersion`.
- [ ] `SpawnPoint`, `Trigger` (kind enum: `ScriptEvent`, `Dialogue`, `Teleport`; `Box` volume;
      parameter dictionary), `LightSource` records.
- [ ] `WorldFSDataManager` (`World/world.json`) and `CellFSDataManager`
      (`World/Cells/{x}_{z}.cell.json`) following the existing JSON DTO pattern. Delete
      `MapFSDataManager`, `TileAttribute` and its binary codec, and all `*TileAttribute` classes.
- [ ] Round-trip tests for both managers, including an empty cell and a fully populated cell.

### Converter

- [ ] `Lunar.Tools.MapConvert` console project: reads a legacy `.map`, emits a flat cell (or
      several, if the map exceeds the cell size): blocked tiles become `BlockerVolumes`, spawn
      attributes become `SpawnPoints`, dialogue attributes become `Dialogue` triggers, warps
      become `Teleport` triggers. Test on the existing sample maps.

### Server

- [ ] `World` replaces `Map`/`MapManager`: sparse `Dictionary<CellCoord, Cell>`, `EnsureLoaded`,
      idle unload timer, `CellLoaded`/`CellUnloaded` events.
- [ ] `Cell`: resident actors, ground items, spawners, triggers, `BlockerVolumes` used for
      interim AABB movement blocking until Milestone 3.
- [ ] Spawners: `NPCSpawnAttributeActionHandler` logic becomes a `Spawner` per `SpawnPoint` with
      respawn timers. Player spawn selects the nearest `Player` spawn point to the saved position
      or a designated start point.
- [ ] Triggers: evaluated on actor movement by `Box.Contains`; `ScriptEvent` raises into the
      Roslyn behaviour, `Dialogue` starts a dialogue, `Teleport` moves the actor (and, later,
      switches world for instances).
- [ ] Actors track their `CellCoord`; movement across a boundary moves them between cells.
- [ ] Delete `Map`, `Layer`, `Tile`, `MapObject`, `TorchMapObject`, `MapItem` layer fields,
      `WarpTileAttributeActionHandler`, `NPCSpawnAttributeActionHandler`, `Settings.TileSize`,
      `EngineConstants.TILE_SIZE`, `Constants.MAP_ITEM_WIDTH/HEIGHT`.
- [ ] Packets: `MAP_DATA` deleted; `WORLD_INFO` (cell size, bake params, ambient) sent on join;
      `CELL_DATA` carries a `CellModel` for client streaming; `CELL_UNLOAD` tells the client to
      drop one.

### Verification tooling

- [ ] `Lunar.Server.Tests` xunit project: cell coordinate math, load/unload lifecycle, spawner
      timing, trigger firing on entry and exit, converter output.
- [ ] `Lunar.Tools.Bot` console project: minimal headless client that connects, logs in, spawns,
      and walks a scripted route. Used from here through Milestone 4.

### Exit check

- [ ] Converted cells load on demand as the bot walks; NPCs spawn from spawn points; a dialogue
      trigger fires on entry; idle cells unload; server tests green.

---

## Milestone 3: Navigation, streaming and movement

Goal: navmesh movement, cell-window interest management, input/snapshot protocol, region threads.

### Navmesh bake tool

- [ ] `Lunar.Tools.NavBake` console project using `DotRecast.Recast` and `SharpGLTF.Core`:
      inputs are the cell heightmap, its `StaticGeometry` glTF triangles and `BlockerVolumes`;
      output is one Detour tile per cell written beside the cell file.
- [ ] Bake parameters read from `WorldModel`; tile size equals cell size; neighbouring cells
      included as border geometry so tiles connect.
- [ ] Input hash stored with the tile; `--check` mode reports stale tiles.
- [ ] Unit test: a flat cell with one blocker volume bakes to a tile whose `findNearestPoly`
      rejects a point inside the blocker.

### Server navigation

- [ ] `NavigationService` owns one tiled `DtNavMesh` per world; adds and removes tiles with
      cell load/unload.
- [ ] One `DtNavMeshQuery` per region worker thread (queries are per-thread; the mesh is shared
      read-only).
- [ ] Player movement validation: `moveAlongSurface` from current to requested position, height
      from `getPolyHeight`. `BlockerVolumes` no longer consulted at runtime.
- [ ] NPC pathing: `findPath` + `findStraightPath` replaces `Pathfinder`; path following with
      simple steering and arrival radius. Delete `Lunar.Server/Utilities/Pathfinding`.
- [ ] Jump: an animation-state with a scripted vertical arc over a navmesh-validated horizontal
      path; no physics.

### Movement protocol

- [ ] Fixed simulation tick from config (`Settings.TickRate`), replacing the hard-coded 120 Hz
      heartbeat constant; snapshot rate separately configurable (default 15 Hz).
- [ ] `CLIENT_INPUT` message: sequence number, move vector (ground plane, unit or zero), yaw,
      action flags (jump, interact). Sent at the client's fixed rate.
- [ ] `ENTITY_SNAPSHOT` message: server tick, last processed input sequence per recipient, and
      compact entity states (id, `Vector3`, yaw, animation state, health fraction) for entities
      in the recipient's window.
- [ ] `ENTITY_ENTER`/`ENTITY_LEAVE` messages carrying full descriptors on enter, ids on leave.
- [ ] Delete `PLAYER_MOVING`, `POSITION_UPDATE`, `NPC_MOVING`.
- [ ] Round-trip tests for all new messages; a determinism test that replays a recorded input
      stream and reproduces the same final position.

### Interest management

- [ ] `InterestWindow` per player: the 3x3 cell block around the player's cell; recomputed on
      boundary crossing with enter/leave diffs.
- [ ] `Cell.Broadcast` sends only to players whose window contains the cell. Delete
      `Map.SendPacket`-style whole-world broadcasts.
- [ ] Ground item spawn/despawn and chat use the window.

### Threading

- [ ] `Region` = a configurable square of cells (working value 4x4). `RegionScheduler` runs
      each loaded region's tick on a worker thread with a single-writer rule.
- [ ] Every packet handler becomes a command enqueued to the actor's region; no world mutation
      on the network thread. Delete `WorldDictionary`.
- [ ] Actor handoff across region boundaries at the tick boundary via the target region's
      queue; a test moves a bot back and forth across a region seam under load.
- [ ] Thread-safety review of `PlayerManager`, `NPCManager`, `ItemManager` collections.

### Bot client

- [ ] `Lunar.Tools.Bot` speaks the input protocol and walks a route that crosses cell and region
      boundaries; asserts its reconciled position matches the server's.

### Exit check

- [ ] NPCs path across cell boundaries; bots walk the world with cells streaming in and out;
      no whole-world packet remains; server tests green.

---

## Milestone 4: Persistence and server hardening

Goal: nothing is lost on restart; the server survives 200 bots.

### PostgreSQL

- [ ] `Lunar.Server.Data` project: EF Core `DbContext` with `Account`, `Character`,
      `InventorySlot`, `EquipmentSlot` entities; `Npgsql.EntityFrameworkCore.PostgreSQL`;
      initial migration.
- [ ] `docker-compose.yml` at repo root for the development database; connection string in
      `config.json` with environment-variable override.
- [ ] `PlayerManager` uses repositories instead of `PlayerFSDataManager`; delete the file-based
      account manager and the `Accounts/` directory convention.
- [ ] Characters persist inventory, equipment, experience, position, cell and yaw.
- [ ] `AutosaveService`: dirty-tracked characters flushed on a timer and on logout.
- [ ] Graceful shutdown: `PosixSignalRegistration`/`Console.CancelKeyPress` sets the shutdown
      flag, regions drain, autosave flushes, then exit. Delete the never-set `Server.ShutDown`
      static.

### Bug fixes carried from the assessment

- [ ] `ItemManager` data manager rooted at `FILEPATH_ITEMS`, not the NPC path.
- [ ] `Inventory.Add` stacks by item id, not `GetType()`.
- [ ] `Player.Alive` and `Player.Update` agree on the death condition.
- [ ] `NPC.Behavior` re-created on script reload so the old assembly load context can unload.
- [ ] Delete `WebCommunicator`; remove debug `Console.WriteLine` calls from `NPC` hot paths.
- [ ] `ServerHeartbeat` sleeps instead of spinning; tick rate from config.
- [ ] `PlayerManager.LoginPlayer` duplicate-login check uses a dictionary lookup.

### Load testing

- [ ] `Lunar.Tools.Bot` scales to N bots in one process with randomized routes.
- [ ] Server metrics: tick duration histogram per region, bytes per second per client,
      snapshot counts; logged periodically and exportable as CSV.
- [ ] Run 200 bots across several regions for 30 minutes; record results in the assessment.

### Exit check

- [ ] Kill the server mid-run; restart; every bot's character is within one autosave interval of
      its last position with inventory intact. Tick time stable under 200 bots.

---

## Milestone 5a: Renderer proof scene

Goal: `Lunar.Rendering` exists and proves every item of the retro feature list on MonoGame 3.8.5.

### Project and shader loop

- [ ] `Lunar.Rendering` class library referencing MonoGame 3.8.5.1 (DesktopGL and the native
      backend package); `Lunar.Rendering.Sandbox` executable for the proof scene.
- [ ] Effects folder with the new content project system; a single `dotnet build` target that
      rebuilds only changed `.fx` files; documented Windows/CI authoring path.
- [ ] Startup feature check (render target formats, texture array support, MRT count) logged
      per backend.

### Effects (Lunar-owned, no stock effects for world rendering)

- [ ] `Terrain.fx`: heightmap mesh, two-layer splat, per-vertex or flat lighting toggle,
      distance fog, shadow sampling.
- [ ] `StaticMesh.fx`: instanced, unlit/vertex-lit variants, optional vertex snapping and affine
      UV switches.
- [ ] `Skinned.fx`: bone palette in constants (or a bone texture for large rigs), same retro
      switches.
- [ ] `ShadowDepth.fx` for the directional shadow map.
- [ ] Post chain: `Quantize.fx` (palette reduction + ordered dither), `Bloom.fx` (threshold,
      blur, combine), `Outline.fx` (depth/normal edge), `Composite.fx` (integer upscale from the
      fixed internal resolution).

### Scene systems

- [ ] Heightmap terrain built from two adjacent cell tiles with shared edge rows; no visible
      seam; floating origin applied to all transforms.
- [ ] glTF loading via `SharpGLTF.Core` into `VertexBuffer`/`IndexBuffer`, materials, skins and
      animation clips; clip sampling and two-clip blending; vertex-shader skinning.
- [ ] Instanced props: 500 instances from one mesh; frustum culling against `Box` bounds.
- [ ] Lighting: one directional light with a single-cascade shadow map and PCF; up to 8 point
      lights.
- [ ] Post-processing chain wired end to end at a fixed internal resolution.
- [ ] Orbit camera; debug text overlay with frame time and draw count.

### Exit check

- [ ] Scene runs on the native backend and DesktopGL with frame time recorded at 1080p; the
      shader loop is one command; feature-check results and any hit headache recorded in the
      assessment.

---

## Milestone 5b: 3D client vertical slice

Goal: two clients walk across cells together.

### Client rebuild

- [ ] Delete Penumbra, `Lunar.Graphics`, `Lunar.Client/World/*` 2D view classes, the 2D
      `Content.mgcb` and 2D texture assets, `LightManagerService`, `Camera` (2D).
- [ ] `IActor` on the client becomes a pure state holder; `IEntityView` + `EntityViewFactory`
      create views from descriptors (`ModelKey` replaces the sprite sheet path on `PlayerModel`
      and `NPCModel`; DTOs and messages updated with tests).
- [ ] `IInputService` wraps keyboard/mouse/gamepad; gameplay code stops calling
      `Keyboard.GetState()`.
- [ ] GUI: `WidgetCollection`/`GUIManager` keep the XML layouts and event model; widgets draw
      through `Lunar.Rendering`'s overlay path.
- [ ] Scenes: `MenuScene` (login/register), `WorldScene` (streaming world), `LoadingScene` only
      for the initial connect.

### World streaming and simulation

- [ ] `CellStreamer`: applies `CELL_DATA`/`CELL_UNLOAD`, loads terrain and set pieces
      asynchronously, prefetches the next ring near boundaries.
- [ ] Client navmesh: loads the same Detour tiles for local movement projection.
- [ ] `LocalPlayerController`: fixed-rate input sampling, prediction, reconciliation against
      `ENTITY_SNAPSHOT` using the last processed sequence.
- [ ] `RemoteEntityInterpolator`: buffered interpolation between snapshots.
- [ ] Third-person orbit follow camera with the 2D lerp-follow feel as reference.
- [ ] Asset layout under `Client Data/`: models, heightmaps, splats, effects, fonts, audio.

### UI

- [ ] Login and register, chat window, target frame, basic inventory list reusing the existing
      XML layout format.

### Exit check

- [ ] Two clients connect, spawn, walk across several cells with no load screens, see each
      other interpolated, and chat.

---

## Milestone 6: World editor

Goal: cells can be authored end to end in the web editor.

- [ ] Contracts: `WorldManifestDocument`, `CellEditorDocument`, spawn/trigger/light DTOs,
      validation issues for cells.
- [ ] Core: `IWorldRepository`, `ICellRepository`, validation rules (spawn inside cell, trigger
      references resolve, heightmap tile dimensions match cell size, navmesh tile present and
      not stale).
- [ ] Api: `/api/world`, `/api/cells/{x}/{z}` CRUD, `/api/assets/models`,
      `/api/assets/heightmaps`, `/api/navmesh/bake` shelling out to `Lunar.Tools.NavBake`,
      file watching with change notifications.
- [ ] Web: world overview grid; cell viewport (Three.js or Babylon.js, decided at start of the
      milestone) showing the cell and its neighbours; placement gizmos for set pieces, spawns,
      triggers and lights; properties panel; heightmap sculpt and splat paint brushes; dirty
      state and save.
- [ ] NPC and dialogue editors from the editor spec's Milestone 5, since spawn points reference
      NPC keys.

### Exit check

- [ ] A new cell is created, sculpted, populated, baked and walked in the client without
      touching a JSON file by hand.

---

## Milestone 7: Gameplay breadth

Goal: enough systems to run a playtest.

- [ ] Server-side ability system replacing `PlayerInteractAction`: abilities with cast time,
      cooldown, range (planar distance), cost, effects; targeting by id.
- [ ] Spells implemented on the server from `SpellModel` descriptors through the ability system;
      spell packets and client feedback.
- [ ] NPC behaviours in 3D: aggro by planar distance with a height tolerance, leash back to
      spawn, patrol paths as waypoint lists in the cell.
- [ ] Loot tables and ground items in cells; pickup by proximity.
- [ ] Quest definitions and a minimal quest log.
- [ ] Instanced dungeons as separate small worlds reached by `Teleport` triggers.
- [ ] Bot client extended to cast abilities for load testing.

### Exit check

- [ ] A guided playtest: log in, take a quest, travel across the world, fight NPCs with abilities,
      loot, complete the quest, enter and leave an instance.
