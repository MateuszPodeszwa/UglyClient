#!/usr/bin/env bash
set -euo pipefail

export PORT="${PORT:-10000}"
export SENSOR_SERVER_URL="${SENSOR_SERVER_URL:-http://127.0.0.1:5077}"
export ApiSettings__BaseUrl="${ApiSettings__BaseUrl:-$SENSOR_SERVER_URL}"
export ApiSettings__ApiKey="${ApiSettings__ApiKey:-u007-key}"

dotnet /app/server/SensorServer.dll --urls "$SENSOR_SERVER_URL" &
SENSOR_PID=$!

cleanup() {
  kill "$SENSOR_PID" 2>/dev/null || true
}
trap cleanup EXIT

server_ready=false
for _ in $(seq 1 30); do
  if curl -fsS "$SENSOR_SERVER_URL/swagger/index.html" >/dev/null 2>&1; then
    server_ready=true
    break
  fi
  sleep 1
done

if [[ "$server_ready" != "true" ]]; then
  echo "SensorServer failed to start at $SENSOR_SERVER_URL" >&2
  exit 1
fi

exec /usr/local/bin/gotty --port "$PORT" --permit-write --reconnect /app/run-client-loop.sh
