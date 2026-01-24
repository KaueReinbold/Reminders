# Reminders Project - AI Agent Instructions

## Architecture Overview

This is a **full-stack learning project** for managing reminders with multiple client interfaces (React, ASP.NET MVC), a load-balanced .NET API, and blockchain integration.

### Tech Stack
- **Backend**: ASP.NET Core 8.0 Web API (2 instances behind Nginx load balancer)
- **Frontend**: Next.js 15 (React 19) + ASP.NET MVC
- **Database**: PostgreSQL (primary) or SQL Server (legacy support)
- **Blockchain**: Hardhat + Solidity smart contracts on Ganache
- **Infrastructure**: Docker Compose with profiles, Nginx, K6 load testing

### Project Structure

```
src/
├── server/
│   ├── services/dotnet/Reminders.MigrationsRunner/ # Database migration runner (console app)
│   ├── api/dotnet/Reminders.Api/          # API with layered architecture
│   │   ├── Controllers/                          # API endpoints
│   │   ├── Layers/
│   │   │   ├── Application/                      # Business logic & services
│   │   │   ├── Domain/                           # Domain models & contracts
│   │   │   ├── Data/EntityFramework/             # Data access layer
│   │   │   │   ├── Postgres/Migrations/          # PostgreSQL migrations
│   │   │   │   └── SqlServer/Migrations/         # SQL Server migrations (legacy)
│   │   │   └── CrossCutting/                     # Shared infrastructure
│   │   └── Extensions/                           # Middleware & DI extensions
├── app/
│   ├── reactjs/reminders-app/                # Next.js App Router app
│   └── dotnet/Reminders.Mvc/                 # ASP.NET MVC app
blockchain/                                    # Hardhat smart contracts
test/cypress/                                  # E2E tests
```

## Critical Workflows

### Docker Compose Profiles
The project uses **profiles** to control which services start:

```bash
# Start all services (API x2, MVC, React, Postgres, Nginx, Ganache)
docker compose --profile all up -d

# Start only API stack (API x2, Postgres, Ganache, Nginx)
docker compose --profile api up -d

# Debug mode (with remote debugger)
docker compose --profile debug -f docker-compose.yml -f docker-compose.debug.yml up -d

# Production mode
docker compose --profile production -f docker-compose.yml -f docker-compose.production.yml up -d
```

**Important**: Each service requires its profile to start. Tasks in `.vscode/tasks.json` use these profiles.

### Database Migrations

**Migration Runner Service**: Database schema updates are managed by a dedicated console application (`src/server/services/dotnet/Reminders.MigrationsRunner/`) that runs before API instances start.

**Architecture**:
- **Execution Model**: Runs once per deployment, exits after completion (not long-running)
- **Orchestration**: Docker Compose ensures `migrations` service completes successfully before starting `dotnet-api` and `go-api`
- **Health Endpoint**: Exposes `/healthz` on port 8081 (development only)
  - HTTP 500: Migrations pending, running, or failed
  - HTTP 200: Migrations completed successfully
- **Retry Logic**: Exponential backoff with jitter (5 attempts, 2s base delay)
- **Provider-Specific**: Automatically detects and applies only relevant migrations (Postgres vs SQL Server)

**Dual Database Support**: Both PostgreSQL and SQL Server migrations exist. Default is PostgreSQL.

**Expected Behavior**: When using PostgreSQL, SQL Server migration errors in logs are harmless and expected.

**Creating Migrations**:

```bash
# PostgreSQL (default)
dotnet ef migrations add MigrationName \
  --project src/server/api/dotnet/Reminders.Api \
  --context RemindersContext \
  --output-dir Layers/Data/EntityFramework/Postgres/Migrations

# SQL Server (legacy)
dotnet ef migrations add MigrationName \
  --project src/server/api/dotnet/Reminders.Api \
  --context RemindersContext \
  --output-dir Layers/Data/EntityFramework/SqlServer/Migrations
```

**Applying Migrations**:
- **Docker Compose (recommended)**: Migrations run automatically via the `migrations` service before APIs start
- **Manual Execution**: `cd src/server/services/dotnet/Reminders.MigrationsRunner && dotnet run`
- **Note**: The API no longer runs migrations on startup — this is now handled by the dedicated migration runner service

**Migration Service Dependencies**:
```
postgres (healthy) → migrations (completed successfully) → APIs (start)
```

**Configuration**: Set via `appsettings.json` or environment variables:
- `ConnectionStrings__DefaultConnection`: Database connection string
- `DatabaseProvider`: "Postgres" or "SqlServer"
- `MigrationRunner__MaxRetryAttempts`: Number of retry attempts (default: 5)
- `MigrationRunner__RetryBaseDelaySeconds`: Base delay for exponential backoff (default: 2)

### Building & Testing

```bash
# Build .NET solution
dotnet build ./src

# Run React tests (99.6% coverage)
cd src/app/reactjs/reminders-app && npm test

# Run blockchain tests (Hardhat)
cd blockchain && npm test

# Run Cypress E2E tests
cd test/cypress && npm run cy:run

# Run .NET API tests (requires running API)
docker compose up postgres ganache -d
cd src/server/api/dotnet/Reminders.Api && dotnet run &
cd test/server/dotnet/Reminders.Api.Test && dotnet test
```

### Blockchain Development

Smart contracts are in [blockchain/contracts/Reminders.sol](../blockchain/contracts/Reminders.sol). The API integrates via `IRemindersBlockchainService`.

```bash
cd blockchain
npm install
npm test                  # Run Hardhat tests
npm run deploy           # Deploy to Ganache
npm run create           # Create reminder on blockchain
npm run get              # Get reminder by ID
```

**Configuration**: Set blockchain settings in `.env` (private key, contract address, node URL).

## Project-Specific Conventions

### Layered Architecture (API)

The API follows a **clean architecture** with explicit layers:

- **Controllers** → `IRemindersService` (Application layer)
- **Application/Services** → `IRemindersRepository` (Domain contracts)
- **Data/Repositories** → Entity Framework DbContext

**Namespaces**:
- Domain: `Reminders.Domain.*`
- Application: `Reminders.Application.*`
- Infrastructure: `Reminders.Infrastructure.*`
- API: `Reminders.Api.*`

### Dependency Injection Patterns

Each layer registers its services via DI configuration files:
- [Data/EntityFramework/DependencyInjection.cs](../src/server/api/dotnet/Reminders.Api/Layers/Data/EntityFramework/DependencyInjection.cs)
- [Application/Extensions/ApplicationConfiguration.cs](../src/server/api/dotnet/Reminders.Api/Layers/Application/Extensions/ApplicationConfiguration.cs)

Called from [Program.cs](../src/server/api/dotnet/Reminders.Api/Program.cs):
```csharp
builder.Services.RegisterApplicationServices(
    connectionString,
    SupportedDatabases.Postgres
);
```

### React App Structure

Uses **Next.js App Router** with:
- Server components by default (`async function Page()`)
- Client components marked with `'use client'` ([edit-client.tsx](../src/app/reactjs/reminders-app/src/app/reminder/[id]/edit-client.tsx))
- `@tanstack/react-query` for data fetching
- Material-UI v7 components
- `@/app` path alias for imports

### Environment Variables

**Required setup**: Copy `.env.example` to `.env` and configure:

```bash
DATABASE_PROVIDER=Postgres
CONNECTION_STRING=Host=reminders-postgres;Database=Reminders;Username=postgres;Password=YOUR_PASSWORD_HERE
BLOCKCHAIN_NODE_URL=http://reminders-blockchain:8545
BLOCKCHAIN_PRIVATE_KEY=0xc87509a1c067bbde78beb793e6fa76530b6382a4c0241e5e4a9ec0a0f44dc0d3
CORS_ORIGINS=http://localhost:3000
API_BASE_URL=http://reminders-nginx:9999
```

## Integration Points

### Load Balancing
Two API instances (`api`, `api-2`) run behind Nginx ([infrastructure/nginx.conf](../infrastructure/nginx.conf)). Requests to port 9999 are load-balanced.

### Blockchain Integration
- API calls smart contracts via `IRemindersBlockchainService` ([Layers/Application/Contracts/IBlockchainReminderService.cs](../src/server/api/dotnet/Reminders.Api/Layers/Application/Contracts/IBlockchainReminderService.cs))
- Ganache runs on port 8545 with deterministic mnemonic
- Contract artifacts in `blockchain/artifacts/`

### Cross-Origin Requests
CORS configured via `AddRemindersCors()` extension in [Extensions/CorsExtensions.cs](../src/server/api/dotnet/Reminders.Api/Extensions/CorsExtensions.cs). Origins set in `.env`.

## Debugging

### Remote Debugging .NET in Docker
Use `debug` profile which mounts `~/.vsdbg` and builds with `Debug` configuration:

```bash
docker compose --profile debug -f docker-compose.yml -f docker-compose.debug.yml up -d
```

### Viewing Logs
```bash
docker compose logs -f api          # API instance 1
docker compose logs -f api-2        # API instance 2
docker compose logs -f react        # React app
docker compose logs -f postgres     # Database
```

## Common Tasks

- **Add new endpoint**: Create method in `RemindersController` → implement in `RemindersService` → add repository method if needed
- **Modify entity**: Update [Domain/Models/Reminder.cs](../src/server/api/dotnet/Reminders.Api/Layers/Domain/Models/Reminder.cs) → create migration → update repository/service
- **Add React page**: Create in `src/app/reactjs/reminders-app/src/app/` following App Router conventions
- **Deploy contract changes**: Update [blockchain/contracts/Reminders.sol](../blockchain/contracts/Reminders.sol) → `npm run deploy` → update `BLOCKCHAIN_CONTRACT_ADDRESS` in `.env`

## Testing Standards

- React: Jest + React Testing Library (target 99%+ coverage)
- Blockchain: Hardhat + Chai (test all contract functions)
- E2E: Cypress tests in [src/test/cypress/cypress/e2e/](../src/test/cypress/cypress/e2e/)
- .NET: xUnit integration tests (require running API)

## Learning Project Notice

This is explicitly a **learning project**. Code may not follow best practices everywhere. When contributing, prefer clear explanations and incremental improvements over perfect solutions.
