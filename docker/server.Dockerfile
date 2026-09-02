# Lunar.Server runtime image.
#   docker build -f docker/server.Dockerfile -t lunar-server .
#   docker run --rm -p 25566:25566/udp lunar-server
# Configuration: LUNAR_SERVER_PORT overrides the port in Server Data/config.json; mount a volume at
# /data containing a "Server Data" directory and set LUNAR_DATA_ROOT=/data to supply your own content.

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/ ./
RUN dotnet publish Lunar.Server/Lunar.Server.csproj -c Release -o /out --nologo

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /out ./
EXPOSE 25566/udp
# SIGTERM is handled by Program.cs: the world is saved before exit.
STOPSIGNAL SIGTERM
ENTRYPOINT ["dotnet", "Lunar.Server.dll"]
