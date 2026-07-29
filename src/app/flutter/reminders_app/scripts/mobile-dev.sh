#!/usr/bin/env bash
#
# Reminders - mobile dev launcher
#
# Builds a debug APK of the Flutter app and serves it over the LAN so a phone
# can install it from the browser. Usage:
#
#   src/app/flutter/reminders_app/scripts/mobile-dev.sh
#
set -euo pipefail

APP_DIR="$(cd "$(dirname "$0")/.." && pwd)"
APK="$APP_DIR/build/app/outputs/flutter-apk/app-debug.apk"
SERVE_DIR="/tmp/reminders-apk-serve"
PORT="${PORT:-8090}"

command -v flutter >/dev/null 2>&1 || { echo "FAIL: flutter not found in PATH"; exit 1; }
command -v python3 >/dev/null 2>&1 || { echo "FAIL: python3 not found in PATH"; exit 1; }

echo "Building debug APK..."
(cd "$APP_DIR" && flutter build apk --debug)

# LAN IP: Linux via default-route lookup, macOS via ipconfig
if command -v ip >/dev/null 2>&1; then
  LAN_IP="$(ip route get 1.1.1.1 | awk '{for(i=1;i<NF;i++) if($i=="src") print $(i+1); exit}')"
else
  LAN_IP="$(ipconfig getifaddr en0 2>/dev/null || ipconfig getifaddr en1 2>/dev/null || true)"
fi
[[ -z "${LAN_IP:-}" ]] && { echo "FAIL: could not detect LAN IP"; exit 1; }
mkdir -p "$SERVE_DIR"
cp "$APK" "$SERVE_DIR/reminders-debug.apk"
cat > "$SERVE_DIR/index.html" << HTML
<!DOCTYPE html>
<html><head>
  <meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Reminders - Download APK</title>
  <style>
    body { font-family: 'DM Sans', system-ui, sans-serif; text-align: center;
           padding: 40px 20px; background: #F3F0E9; color: #1B1A17; }
    .btn { display: inline-block; padding: 16px 32px; background: #1B1A17;
           color: #F7F5F0; text-decoration: none; border-radius: 999px;
           font-size: 1.2em; margin: 20px 0; }
  </style>
</head><body>
  <h1>Reminders</h1>
  <p>Built $(date '+%H:%M:%S %d/%m/%Y')</p>
  <a class="btn" href="reminders-debug.apk">Download APK</a>
</body></html>
HTML

# Under WSL the LAN IP lives on the Windows side; the phone cannot reach the
# WSL address directly, so detect the Windows IP and print the portproxy setup.
WIN_IP=""
if command -v powershell.exe >/dev/null 2>&1; then
  WIN_IP="$(powershell.exe -NoProfile -Command \
    "(Get-NetIPConfiguration | Where-Object {\$_.IPv4DefaultGateway -ne \$null -and \$_.NetAdapter.Status -eq 'Up'}).IPv4Address.IPAddress" \
    2>/dev/null | tr -d '\r' | head -n1)"
fi

echo
if [[ -n "$WIN_IP" ]]; then
  echo "Phone URL (same WiFi): http://$WIN_IP:$PORT"
  echo
  echo "WSL detected: one-time setup, run each line in Windows PowerShell (admin):"
  echo "  netsh interface portproxy add v4tov4 listenport=$PORT connectaddress=$LAN_IP connectport=$PORT"
  echo "  New-NetFirewallRule -DisplayName \"Reminders APK $PORT\" -Direction Inbound -Protocol TCP -LocalPort $PORT -Action Allow"
  echo "Note: WSL IP ($LAN_IP) changes across reboots; re-run the netsh line when it does:"
  echo "  netsh interface portproxy delete v4tov4 listenport=$PORT"
else
  echo "Phone URL (same WiFi): http://$LAN_IP:$PORT"
  echo "If unreachable, open TCP $PORT in your firewall (e.g. ufw allow $PORT/tcp)."
fi
echo "Ctrl+C to stop."
python3 -m http.server "$PORT" --directory "$SERVE_DIR"
