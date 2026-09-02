# Lunar Engine Testing Pipeline

## Purpose

Every milestone of the 3D revamp changes the world model, the wire protocol and the renderer at
the same time. Without a pipeline that exercises a real server and a real rendering client on
every change, regressions in those layers surface only when someone plays. This document
describes the pipeline that is in place, how to run it locally and on the Docker host, and the
conventions every future change must follow.

The pipeline has three layers. Each catches a different class of defect and each is cheap enough
to run on every push.

| Layer | What runs | Where | Time |
|---|---|---|---|
| Unit | xunit projects (`Lunar.Core.Tests`, and per-project test projects as they are added) | Any machine, GitHub-hosted runner | seconds |
| End-to-end | Real `Lunar.Server` + real `Lunar.Client.Desktop` rendering under Xvfb, driven through the client's automation endpoint, asserting on state and on captured frames | Linux with Mesa, or the `lunar-e2e` container on the Docker host | about a minute |
| Load (Milestone 4) | Headless bot clients against a server | Docker host | minutes |

## What the first run found

The pipeline was built against the `modernization` tip and was run before any 3D work. Getting
five end-to-end tests green required fixing the following defects in the engine, none of which
were visible from unit tests or from reading code:

- The server P/Invoked `ntdll.dll` for timer resolution and crashed on Linux at startup.
- The client loaded interface layouts from `Interface/` while the directory is `interface/`, a
  crash on any case-sensitive file system.
- The C# gameplay scripts under `Server Data/World/Scripts` were compiled into the server
  assembly by the default project glob and never copied to the output, so the Roslyn script host
  found no scripts and every player ran without a behaviour.
- The script compiler mixed .NET 9 reference assemblies with the running framework's
  implementation assemblies, so Roslyn could not resolve `System.Void` and no script compiled.
- `Layer` built its tiles before its `Map` was assigned, so any map containing a tile attribute
  (a spawn point, a warp) threw during load.
- An exception in any packet handler terminated the server process; there was no isolation.
- Registration succeeded but the client's success handler threw `NotImplementedException`.
- Login refused to send while already connected, so register-then-login on one connection
  could never work.
- Login failures were sent unreliably and immediately followed by a forced disconnect, so the
  reason never reached the client.
- The client identified "its own" player by comparing the server's peer id with its own local
  peer id. LiteNetLib numbers peers independently on each side, so this only worked for the
  first connection to a fresh server.
- The logger dereferenced a null exception on the error path and repositioned the console
  cursor even when output was redirected.
- The default player sprite path did not resolve under the client's asset path convention.
- The repository shipped no map, so joining the world always failed.

All of these are fixed on this branch, and each is now covered by a test that fails if it
regresses.

## Layer 1: Unit tests

- Projects: `src/Lunar.Core.Tests` today; `Lunar.Server.Tests`, `Lunar.Rendering.Tests` and
  `Lunar.Tools.*.Tests` are added by the milestones that create the code they cover.
- Framework: xunit, `net9.0`. Every test project is in `Lunar Engine.sln`.
- Run everything except end-to-end:

```
dotnet test "src/Lunar Engine.sln" --filter "Category!=E2E"
```

- Conventions:
  - Any `Pack`/`Unpack` pair, data-manager DTO, or message record ships with a round-trip test
    in the same change. The wire format is positional with no field tags; the compiler cannot
    catch a mismatch.
  - Pure logic (math, pathing, interest-management windows, stat calculation, spawn selection)
    gets unit tests before integration coverage. Prefer testing through public types in
    `Lunar.Core`.
  - Content that ships in `Server Data` is loaded in a unit test so schema drift is caught
    without starting a server.

## Layer 2: End-to-end tests

### Architecture

```
 Lunar.E2E.Tests (xunit)
   |-- ServerInstance  -> dotnet Lunar.Server.dll   (own UDP port, private copy of "Server Data")
   |-- XvfbDisplay     -> Xvfb :N                    (Linux only; no-op elsewhere)
   |-- ClientInstance  -> dotnet Lunar.Client.Desktop.dll  (DISPLAY=:N, software GL)
   |        ^ HTTP on 127.0.0.1:<port>  (LUNAR_AUTOMATION_PORT)
   |-- Frame / Golden  -> SkiaSharp image analysis
```

Everything is a real process. Nothing is mocked. The server and client are the same binaries a
player would run; the only differences are environment variables.

### The client automation endpoint

`Lunar.Client/Automation/AutomationServer.cs` is compiled into the client but inert unless
`LUNAR_AUTOMATION_PORT` is set. It listens on loopback only and marshals every call onto the game
thread.

| Endpoint | Purpose |
|---|---|
| `GET /health` | 200 once the game loop is running |
| `GET /state` | JSON: active scene, connection state, menu status text, frames rendered, back-buffer size, local player (name, position, health) |
| `GET /screenshot` | PNG of the next rendered back buffer |
| `POST /login`, `POST /register` | `{"username","password"}`; fills the menu boxes and submits exactly as a click would |
| `POST /quit` | Exits the client through its normal path (disconnects from the server) |

As the 3D client is built, `/state` grows (camera, loaded cells, entity views) and input
injection is added behind the input service from Milestone 5b. The rule is that automation
drives the same code paths a player does; it never reaches into internals to fake a result.

### Environment hooks

| Variable | Read by | Effect |
|---|---|---|
| `LUNAR_SERVER_PORT` | server, client | UDP port (server listens, client connects) |
| `LUNAR_SERVER_HOST` | client | Host to connect to |
| `LUNAR_DATA_ROOT` | server | Directory containing `Server Data` |
| `LUNAR_RESOLUTION` | client | `WxH` back buffer |
| `LUNAR_AUTOMATION_PORT` | client | Enables the automation endpoint |
| `LUNAR_SERVER_BIN`, `LUNAR_CLIENT_BIN` | tests | Override build-output discovery |
| `LUNAR_E2E_ARTIFACTS` | tests | Where screenshots and logs are written (default `artifacts/e2e`) |
| `LUNAR_UPDATE_GOLDENS` | tests | `1` rewrites golden images instead of asserting |

The server prints `Server ready on port N` when listening and stops cleanly on SIGTERM or
Ctrl+C, saving the world before exit. Both behaviours are asserted by tests.

### Image analysis

`Harness/Frame.cs` wraps a decoded PNG and offers:

- `LitFraction()`: share of pixels brighter than a threshold. Catches a black or unrendered frame.
- `DistinctColorBuckets()`: colour variety after quantisation. Catches a frame that is a flat
  fill.
- `AverageColor(rect)`: for asserting a UI element or region (health bar, minimap) shows the
  expected colour.
- `MeanAbsoluteDifference(other)`: whole-frame similarity for golden comparison.

`Harness/Golden.cs` compares against committed images in `Lunar.E2E.Tests/Goldens`. A missing
golden is written on first run; a mismatch writes `.actual.png` and `.expected.png` next to the
other artifacts for inspection. Tolerances are loose (mean difference of a few units) so font
hinting and driver differences between machines do not cause noise, while a missing panel or a
wrong scene fails clearly.

Planned additions, in the order they become useful: region masks so dynamic areas (chat, frame
counter) are excluded from golden comparison; OCR of the status label and chat via Tesseract
for text assertions; and, for the 3D client, structural checks (horizon line present, character
silhouette within the expected screen region) before any pixel comparison.

### Test inventory

`MenuAndLoginTests` (one server per class, one client per test):

- Client boots into the menu, renders a non-blank frame at the requested resolution, matches the
  menu golden.
- Registering enters the world with the player on the default map's spawn tile at the expected
  coordinates and full health; the account file exists on the server.
- Logging in from a second client with an account the first client created and left; asserts the
  server released the account promptly on clean exit.
- Logging in with a nonexistent account stays on the menu, shows the failure reason, and leaves
  the connection open for a retry.

`ServerLifecycleTests` (own server):

- SIGTERM with a player online: exit code 0, clean shutdown messages, no unhandled exceptions,
  player saved.

### Running locally

Linux (needs `xvfb`, `libgl1-mesa-dri` and the X client libraries; see `docker/e2e.Dockerfile`
for the exact package list):

```
dotnet build "src/Lunar Engine.sln"
dotnet test src/Lunar.E2E.Tests
```

Windows or macOS: the same command, rendering on the real display (Xvfb is skipped).

If only the .NET 10 runtime is installed, set `DOTNET_ROLL_FORWARD=Major`; the harness passes it
through to the child processes.

Artifacts land in `artifacts/e2e/<timestamp>/<test name>/` with `server.log`, one log per
client, and every screenshot taken. That folder is the first thing to open when a test fails.

## Layer 3: Docker host

`docker/e2e.Dockerfile` builds a runner image with the .NET SDK, Xvfb and Mesa and compiles the
solution at image-build time. `docker/run-tests.sh` runs the unit suite and then the end-to-end
suite, writing TRX results and artifacts to the mounted `/artifacts` volume.

```
docker compose run --rm e2e          # unit + end-to-end
docker compose run --rm e2e unit     # unit only
docker compose run --rm e2e e2e      # end-to-end only
```

`docker/server.Dockerfile` is the deployable server image (runtime only, SIGTERM-aware). The
compose file also defines a `server` service for a standalone game server on UDP 25566.

`.github/workflows/ci.yml` runs the unit job on a GitHub-hosted runner and the end-to-end job on
a self-hosted runner labelled `self-hosted, linux, docker`, which is the Docker host. Both upload
their results as workflow artifacts.

The Docker files were written to match the packages verified on the development container in
this session; the image itself has not yet been built on the target host. The first run there
should be treated as part of setting up the runner.

## Conventions for future work

- **Every milestone adds tests at all three layers before its exit check.** The checklists in
  `lunar-3d-revamp-milestone-checklists.md` name them.
- **Automation drives player paths.** New client features expose state through `/state` and
  actions through endpoints that call the same code a player's input would.
- **Goldens are updated deliberately.** Run with `LUNAR_UPDATE_GOLDENS=1`, inspect the diff in
  the pull request, and say why the frame changed.
- **A failing end-to-end test is a defect until proven otherwise.** The run captured above
  found thirteen. Treat "flaky" as a hypothesis to disprove with the artifacts, not a label.
- **Keep the suite under a few minutes.** Share a server per test class, start clients per test,
  and put slow scenarios (load, long walks across cells) in a separate nightly job.
