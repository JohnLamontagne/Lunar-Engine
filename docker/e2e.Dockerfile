# End-to-end test runner: .NET SDK plus Xvfb and Mesa so the real MonoGame client can render with
# software OpenGL inside the container. The test project starts the server and client itself.
#   docker build -f docker/e2e.Dockerfile -t lunar-e2e .
#   docker run --rm -v "$PWD/artifacts:/artifacts" lunar-e2e
# Screenshots, logs and any golden diffs land in ./artifacts/e2e.

FROM mcr.microsoft.com/dotnet/sdk:9.0

RUN apt-get update && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
        xvfb libgl1 libgl1-mesa-dri libglu1-mesa \
        libx11-6 libxext6 libxi6 libxrandr2 libxcursor1 libxinerama1 libxss1 libxxf86vm1 libxkbcommon0 \
        libasound2 libpulse0 libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /repo
COPY src/ ./src/

# Build everything once at image build time so test runs start fast.
RUN dotnet build "src/Lunar Engine.sln" -c Release --nologo -v q

ENV LUNAR_E2E_ARTIFACTS=/artifacts/e2e \
    SDL_AUDIODRIVER=dummy \
    LIBGL_ALWAYS_SOFTWARE=1 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1

# Runs unit tests first (fast, no display), then the end-to-end suite.
COPY docker/run-tests.sh /usr/local/bin/run-tests
RUN chmod +x /usr/local/bin/run-tests
ENTRYPOINT ["run-tests"]
