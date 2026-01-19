# Reminders Blockchain

This directory contains the blockchain smart contracts for the Reminders application, built with Hardhat and Solidity.

## Overview

The blockchain component provides a decentralized storage layer for reminders using Ethereum smart contracts. This ensures data persistence and immutability through blockchain technology.

## Prerequisites

- Node.js 18 or higher
- npm or yarn
- Ganache (provided via Docker Compose in the main project)

## Installation

```bash
# Install dependencies
npm install
```

## Smart Contracts

### Reminders.sol

The main smart contract that manages reminders on the blockchain. It provides functions to:

- Create new reminders
- Retrieve reminders by ID
- Get the total count of reminders
- Update reminder status

### Lock.sol

A sample contract from Hardhat template (can be removed if not needed).

## Available Scripts

```bash
# Run tests
npm test

# Deploy contracts to local network
npm run deploy

# Create a new reminder
npm run create

# Get a reminder by ID
npm run get

# Get total reminder count
npm run count

# Monitor blockchain events
npm run monitor
```

## Deployment

### Local Development (Ganache)

The main project's Docker Compose setup includes Ganache. To deploy contracts:

```bash
# 1. Ensure Ganache is running (from main project root)
docker compose up ganache -d

# 2. Deploy the contract
npm run deploy

# 3. Copy the deployed contract address to your .env file
# Update BLOCKCHAIN_CONTRACT_ADDRESS with the output address
```

### Configuration

The Hardhat configuration (hardhat.config.ts) includes network settings for:

- **local**: Connects to Ganache at `http://localhost:8545`
- Default accounts use a deterministic mnemonic for testing

## Testing

```bash
# Run all tests
npm test

# Run specific test file
npx hardhat test test/Reminders.ts
```

## Project Structure

```text
blockchain/
├── contracts/          # Solidity smart contracts
│   ├── Reminders.sol  # Main reminders contract
│   └── Lock.sol       # Sample contract
├── scripts/           # Deployment and interaction scripts
│   ├── deploy.js     # Deploy contracts
│   ├── create.js     # Create reminder
│   ├── get.js        # Get reminder
│   ├── count.js      # Get count
│   └── monitor.js    # Monitor events
├── test/             # Contract tests
│   ├── Reminders.ts  # Reminders contract tests
│   └── Lock.ts       # Lock contract tests
├── artifacts/        # Compiled contracts (generated)
└── cache/           # Hardhat cache (generated)
```

## Integration with Main Application

The .NET API integrates with these smart contracts to:

1. Store reminders on the blockchain
2. Retrieve reminders from the blockchain
3. Ensure data immutability

Configuration is done via environment variables:

- `BLOCKCHAIN_NODE_URL` - Ganache endpoint
- `BLOCKCHAIN_PRIVATE_KEY` - Account private key
- `BLOCKCHAIN_CONTRACT_ADDRESS` - Deployed contract address

## Common Commands

```bash
# Compile contracts
npx hardhat compile

# Start local node
npx hardhat node

# Run deploy script
npx hardhat run scripts/deploy.js --network local

# Interact with contract
npx hardhat run scripts/create.js --network local
npx hardhat run scripts/get.js --network local

# Clean build artifacts
npx hardhat clean
```

## Notes

- The mnemonic and private keys in this project are for **development only**
- Never use these keys in production or with real funds
- Contract address changes with each deployment
- Update `.env` with the new contract address after deployment
