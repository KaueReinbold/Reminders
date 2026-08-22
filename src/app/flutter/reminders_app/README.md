# Reminders Flutter App

Mobile app for the Reminders solution, following the design system in
`docs/design/reminders-redesign/design-system.md`. Android only for now;
other platforms can be re-added with `flutter create --platforms <p> .`.

## Development

```bash
flutter pub get
flutter analyze
flutter test
```

CI runs analyze + test on PRs touching this app
(`.github/workflows/flutter-pull-request.yml`).

## Structure

- `lib/theme/tokens.dart`: design tokens (colors, radii, spacing) and
  `buildAppTheme()`. Only approved values from the design system; do not
  invent new ones.
- `lib/api/`: `Reminder` model and `RemindersApi` REST client (nginx
  `:9999`). Base URL via `--dart-define=API_BASE_URL=...`; default
  `http://10.0.2.2:9999` reaches the host from the Android emulator.
- `lib/main.dart`: app entry.

## Test on a phone (LAN install)

```bash
scripts/mobile-dev.sh
```

Builds a debug APK and serves it on `:8090` (override with `PORT=...`) with a
download page. Open the printed phone URL on the same WiFi, download
`reminders-debug.apk`, install.

The script detects your environment and prints any extra steps:

- **WSL**: the phone cannot reach the WSL address directly. The script prints
  the Windows LAN URL plus two one-time commands for an admin PowerShell
  (portproxy + firewall rule), run each line separately. The WSL IP changes
  across reboots; when it does, delete and re-add the portproxy entry as
  printed.
- **Native Linux/macOS**: open TCP 8090 in your firewall if the phone cannot
  connect.

Requirements: Flutter with the Android toolchain (`flutter doctor`) and
`python3`. If the Gradle build fails with a `javaCompiler`/toolchain error,
point Flutter at a full JDK: `flutter config --jdk-dir <path-to-jdk>`.
