# ADR-0005: Solidity contracts with a local Ganache node in the stack

- **Status**: accepted
- **Date**: 2026-07-28

## Context

The portfolio also demonstrates smart contract development. Reminders are mirrored on chain, which requires a contract toolchain and a network to run against, both locally and in CI.

## Options considered

### Option 1: No blockchain

Simplest stack, but drops a stated learning goal of the project.

### Option 2: Public testnet (e.g. Sepolia)

Real network conditions, but requires faucets and key management, is slow, and makes local development and CI flaky.

### Option 3: Local node (Ganache) in Docker Compose

Deterministic accounts via a fixed mnemonic, instant blocks, free. Does not exercise real-network conditions.

## Decision

Option 3. The Hardhat project in `blockchain/` holds contracts, tests and deploy scripts; Ganache runs in compose (8545); the .NET API integrates with the deployed contract.

## Consequences

- Easier: reproducible local chain; fast Hardhat tests in CI.
- Harder: a local chain hides real-network concerns (gas costs, latency, reorgs).
- Watch: the fixed mnemonic is for local development only; never reuse those keys anywhere real.
