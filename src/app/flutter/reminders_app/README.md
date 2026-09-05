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

Flutter version: **3.41.2** (stable). CI pins the same version so
`pubspec.lock` is resolved and tested with the toolchain used locally. When
upgrading Flutter, bump `flutter-version` in the workflow and regenerate
`pubspec.lock` in the same PR.

## Structure

- `lib/theme/tokens.dart`: design tokens (colors, radii, spacing) and
  `buildAppTheme()`. Only approved values from the design system; do not
  invent new ones.
- `lib/api/`: `Reminder` model and `RemindersApi` REST client (nginx
  `:9999`). Base URL via `--dart-define=API_BASE_URL=...`; default
  `http://10.0.2.2:9999` reaches the host from the Android emulator.
- `lib/reminders/`: `grouping.dart` (bucketing, filtering, week progress,
  mirroring the React `reminderGroups.ts` rules) and `list_screen.dart` (the
  mobile list: search, filter chips, progress strip, grouped cards, FAB).
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

### HTTP and the local API

`android.permission.INTERNET` is declared in
`android/app/src/main/AndroidManifest.xml`, so every build type can reach the
API. Plain HTTP is a separate switch: Android denies cleartext since API 28,
and `android/app/src/main/res/xml/network_security_config.xml` keeps that deny
as the default. Only `localhost` and `10.0.2.2` (the emulator's alias for the
host machine) are allowed, so the committed config carries no one's address and
a store build stays HTTPS-only. See [ADR-0014](../../../../docs/adr/0014-android-cleartext-policy.md).

Testing a build against an API on your own LAN needs a temporary local edit:
add your machine's address to that `domain-config` block, build, then revert
the file. Do not commit it.

```xml
<domain includeSubdomains="false">192.168.1.10</domain>
```

```bash
flutter build apk --release --target-platform android-arm64 \
  --dart-define=API_BASE_URL=http://192.168.1.10:9999
git checkout android/app/src/main/res/xml/network_security_config.xml
```

Requirements: Flutter with the Android toolchain (`flutter doctor`) and
`python3`. If the Gradle build fails with a `javaCompiler`/toolchain error,
point Flutter at a full JDK: `flutter config --jdk-dir <path-to-jdk>`.
