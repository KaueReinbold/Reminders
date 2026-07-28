# Product Vision

Reminders is a task-reminder product built as the same application across multiple stacks: three API implementations (.NET, Go, C++) behind one contract, two web frontends, a mobile app, and a blockchain audit trail. It is for people who want a simple, fast way to capture reminders and trust that they resurface at the right time. The multi-stack setup keeps every feature honest: if it cannot be expressed in the shared REST contract, it is not ready.

The board and the Ideas discussions are the roadmap. This page only sets direction.

## Pillars

### Reliable delivery

Reminders should fire, not just sit in a list. Scheduled delivery over email, push, or webhook, with retries, dead-letter handling, and idempotent processing. See [discussion #363](https://github.com/kauereinbold/Reminders/discussions/363).

### Assistive intelligence

Capturing a reminder should take one natural sentence, and the app should help triage what is overdue. Natural language parsing, semantic search, and evaluated LLM features. See [discussion #364](https://github.com/kauereinbold/Reminders/discussions/364).

### Event-driven core

Domain events give the polyglot services a real role: consumers in Go and C++, history and audit views built from the event stream. See [discussion #365](https://github.com/kauereinbold/Reminders/discussions/365).

### Explorations

Smaller ideas that sharpen the product without a pillar of their own: authentication, rate limiting, health checks. See [discussion #366](https://github.com/kauereinbold/Reminders/discussions/366).
