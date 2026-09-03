# Lunar Engine: working notes for Claude sessions

Read this first, then `docs/lunar-3d-revamp-assessment.md` (decisions and architecture),
`docs/lunar-3d-revamp-milestone-checklists.md` (the work, in order) and
`docs/lunar-testing-pipeline.md` (how everything is verified).

## Where things stand

- Active branch: `claude/lunar-engine-3d-revamp-xp1jmv`, branched from `modernization`. No pull
  request has been opened. `master`/`main` is far behind `modernization`.
- Milestone 0 of the 3D revamp is complete (math types, MonoGame 3.8.5.1, plan docs, test
  pipeline). Milestone 1 (3D actor model) has not been started.
- The owner's decisions are final and recorded in the assessment's Decision Record: MonoGame stays
  as the backend, no third-party engine as the front end, seamless streamed open world, heightmap
  plus glTF set pieces, navmesh-only server movement, PostgreSQL, delete 2D code rather than shim
  it. Do not reopen these without being asked.

## Build and test

```
dotnet build "src/Lunar Engine.sln"
dotnet test  "src/Lunar Engine.sln" --filter "Category!=E2E"     # unit: Core + Server tests
dotnet test  src/Lunar.E2E.Tests                                  # end-to-end (real server + client)
```

- Projects target `net9.0`. If only the .NET 10 runtime is installed (true of the hosted Claude
  containers), set `DOTNET_ROLL_FORWARD=Major`; the E2E harness passes it to child processes.
- The .NET SDK is not preinstalled in the hosted container and `dot.net` downloads are blocked by
  the egress policy there. `apt-get update && apt-get install -y dotnet-sdk-10.0` works.
- The E2E suite on Linux needs `xvfb libgl1 libgl1-mesa-dri libglu1-mesa` and the X client
  libraries listed in `docker/e2e.Dockerfile`. The client renders with Mesa software OpenGL.
- Run the E2E suite serially (it already disables xunit parallelization). It takes about 45
  seconds for 8 tests. Artifacts (server log, per-client logs, screenshots) go to
  `artifacts/e2e/<timestamp>/<test>/`. Each test class's fixture makes its own timestamp folder, so
  one run spreads across two or three folders. Open the artifacts before assuming a flake.
- Goldens live in `src/Lunar.E2E.Tests/Goldens`. `LUNAR_UPDATE_GOLDENS=1` rewrites them.
- `docker compose run --rm e2e` runs unit + E2E in a container. The image was written against the
  exact packages verified in the hosted container but has **not yet been built on the owner's
  Docker host**; treat its first run there as setup work. The CI E2E job expects a self-hosted
  runner labelled `self-hosted, linux, docker`.

## Conventions that are easy to miss

- Server and client are driven in tests through real processes and the client's automation
  endpoint (`LUNAR_AUTOMATION_PORT`). Automation must go through player paths: `Input` facade for
  keys/mouse/text, widgets by name for UI. Never add an endpoint that calls gameplay code.
- All client input reads go through `Lunar.Client/Utilities/Input/Input.cs`. Do not call
  `Keyboard.GetState()` or `Mouse.GetState()` directly; automation injection depends on it.
- Every `Pack`/`Unpack` pair and data-manager DTO gets a round-trip test in the same change. The
  wire format is positional with no field tags.
- Files under `src/Lunar.Server/Server Data/` are runtime content, including the `.cs` gameplay
  scripts compiled by the Roslyn host at startup. They are excluded from the server's own compile
  by `<Compile Remove>`; do not undo that.
- Textboxes ignore special keys for 200 ms after activation (`Textbox._activatedInputCooldown`).
  Automation waits it out before tapping Enter; a human never notices.
- Texture paths in descriptors and maps are relative to `Client Data/` (for example
  `gfx/Characters/soldier.png`). Tiles, players and NPCs all use this convention now.
- Register on this server logs the new account straight into the world; there is no separate
  "registered, now log in" step.
- Commit messages carry no model identifiers. Keep the existing attribution trailer style.

## Known loose ends (not yet fixed, not blocking)

- The server logs `Error: Invalid player connection socket.` when it broadcasts to a peer that has
  just disconnected. Harmless noise; the cleanup belongs with the interest-management work in
  Milestone 3.
- `GameScene.Update` has a no-op `messageBox.Active = messageBox.Active;` on Enter. Chat works
  because the chat box is activated by clicking it; pressing Enter to open chat does nothing.
- Two players spawning at once stand on the same tile (single spawn point on the default map).
- `menu_interface.xml` references `gfx/Interface/sliderContainer.png` and `sliderControl.png`,
  which do not exist; the loader substitutes a 1x1 dummy texture.
- `Lunar.Editor` and `Lunar.UnitTests` are unbuildable and outside the solution; `Lunar.Client.Mobile`
  is an empty stub. All three are scheduled for deletion in Milestone 1.
- `Lunar.Client.Desktop/Content/Content.mgcb` still lists every 2D PNG but is not used for
  textures; fonts and audio are the only pipeline assets.
- The `TimerHelper` sleep test has a generous bound because it is timing-sensitive under load.
- One E2E run early on showed a single unexplained failure before the classes were serialized;
  three consecutive clean runs since, but keep an eye on it.

## First thing a new session should do

Build, run the unit suite, run the E2E suite once, and read the artifacts. If all green, start
Milestone 1 from the checklist. If the E2E suite cannot run in the environment (no Xvfb or Mesa),
say so rather than skipping it silently.
