#!/bin/sh
set -e

DOTNET_URL="http://dotnet-api:8080/health"
RETRIES=60
SLEEP=2

echo "Waiting for dotnet API at $DOTNET_URL..."
count=0
while [ $count -lt $RETRIES ]; do
  if curl -sS -f "$DOTNET_URL" >/dev/null 2>&1; then
    echo "Dotnet API is healthy. Starting Go API."
    # Execute the original command
    exec "$@"
  fi
  count=$((count+1))
  echo "Waiting... ($count/$RETRIES)"
  sleep $SLEEP
done

echo "Timed out waiting for dotnet API after $((RETRIES * SLEEP)) seconds." >&2
exit 1
