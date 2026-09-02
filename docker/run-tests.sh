#!/usr/bin/env bash
# Runs the full test pipeline inside the e2e image (or on any Linux box with the same packages).
# Usage: run-tests [unit|e2e|all]   (default: all)
set -euo pipefail

MODE="${1:-all}"
CONFIG="${LUNAR_BUILD_CONFIG:-Release}"
RESULTS="${LUNAR_E2E_ARTIFACTS:-/artifacts/e2e}/test-results"
mkdir -p "$RESULTS"

cd /repo/src 2>/dev/null || cd "$(dirname "$0")/../src"

if [[ "$MODE" == "unit" || "$MODE" == "all" ]]; then
  echo "==> Unit tests"
  dotnet test "Lunar Engine.sln" -c "$CONFIG" --no-build --nologo \
    --filter "Category!=E2E" \
    --logger "trx;LogFileName=unit.trx" --results-directory "$RESULTS"
fi

if [[ "$MODE" == "e2e" || "$MODE" == "all" ]]; then
  echo "==> End-to-end tests"
  dotnet test Lunar.E2E.Tests/Lunar.E2E.Tests.csproj -c "$CONFIG" --no-build --nologo \
    --filter "Category=E2E" \
    --logger "trx;LogFileName=e2e.trx" --results-directory "$RESULTS"
fi
