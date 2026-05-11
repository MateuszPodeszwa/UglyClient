#!/usr/bin/env bash
set -euo pipefail

while true; do
  dotnet /app/client/BeautifulClient.dll
  printf '\nSession ended. Restarting terminal app in 2 seconds...\n'
  sleep 2
done
