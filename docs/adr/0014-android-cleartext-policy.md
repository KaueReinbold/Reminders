# ADR-0014: Android network security config with a loopback-only cleartext allow-list

- **Status**: accepted
- **Date**: 2026-09-05
- **Issue**: #430 (blocks #383)

## Context

`android.permission.INTERNET` was declared only in the profile manifest, so release APKs shipped without it and every network call failed. The app had only ever worked under debug tooling, and #383 (publish to the Play Store) was blocked, because a store build is a release build.

Declaring the permission in the main manifest is not enough on its own: Android denies cleartext HTTP since API 28, and the local stack serves the API over plain HTTP on `:9999`. So the fix has to say what the app is allowed to reach in the clear, for every build type, on a public repository.

## Options considered

### Option 1: `android:usesCleartextTraffic="true"` in the main manifest

One attribute, works everywhere. Costs: a shipped build accepts plain HTTP to any host, which is the unsafe default the Play Store flags.

### Option 2: cleartext scoped to a debug manifest overlay

Nothing about cleartext reaches a release build, the strongest shipping posture. Costs: a release APK can no longer reach the local HTTP stack, so the very bug in #430 cannot be reproduced or verified with the build type that had it, and device testing of a release build stops working.

### Option 3: network security config in the main manifest with an allow-list

`res/xml/network_security_config.xml` sets `cleartextTrafficPermitted="false"` as the base config, and lists exceptions. Costs: whatever is on the list ships. Buys: HTTPS-only against every real host, one file that documents the policy, and a release build that still reaches a loopback API.

## Decision

Option 3, with the allow-list held to hosts that mean the same thing on every machine: `localhost` and `10.0.2.2`, the emulator's alias for the host loopback. No LAN address is committed. An earlier revision of this branch carried a maintainer's home address in the list, which would have shipped inside a release manifest and published a personal network detail; the list is now free of any one machine.

Option 2 was rejected on verification: the acceptance criterion in #430 is a release APK talking to the local stack from a physical device.

## Consequences

- Easier: a release build is HTTPS-only against every routable host, and the emulator can still hit the local API over HTTP with no change.
- Harder: testing a release build against an API on a real LAN needs a local, uncommitted edit adding that address to the config, as the app README describes.
- Watch: a store build must never carry a cleartext allow-list beyond these loopback entries. Review the file on any change; an address that is not loopback does not belong in it.
