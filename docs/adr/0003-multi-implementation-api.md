# ADR-0003: Same REST API in .NET, Go and C++ behind Nginx

- **Status**: accepted
- **Date**: 2026-07-28

## Context

Portfolio project whose goal is to demonstrate system design skills and language breadth. There is one reminders REST contract; the question is how many implementations to maintain and how clients reach them.

## Options considered

### Option 1: Single .NET API

Least maintenance, one deploy target, one place to evolve the contract. Shows depth in one stack but no cross-language comparison, which is a core goal of the portfolio.

### Option 2: Multiple implementations behind a load balancer

Same contract implemented in .NET, Go and C++, with Nginx balancing across them. Demonstrates polyglot backend skills and real load balancing. Costs: every contract change lands three times, and behavior drift between implementations must be watched.

## Decision

Option 2. `dotnet-api` (5000), `go-api` (5001) and `cpp-api` (5002) serve the same REST contract; Nginx (9999) load balances across them. The .NET implementation is the reference and carries the full feature set, including blockchain integration.

## Consequences

- Easier: comparing stacks side by side; demonstrating infra skills (load balancing, health checks, compose profiles).
- Harder: triple cost for contract changes; parity gaps as Go and C++ trail the .NET reference.
- Watch: clients must route through Nginx and never depend on implementation-specific behavior.
