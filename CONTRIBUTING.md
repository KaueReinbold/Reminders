# Contributing to Reminders

Thank you for your interest in contributing to the Reminders project! This document provides guidelines and information for contributors.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Change Workflow](#change-workflow)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Database Migrations](#database-migrations)
- [Making Changes](#making-changes)
- [Testing](#testing)
- [Pull Request Process](#pull-request-process)

## Code of Conduct

This is a learning project, and we welcome contributions from developers of all skill levels. Please be respectful, constructive, and supportive of others.

## Change Workflow

Every change follows the same path:

1. **Discussion (RFC)**: questions and ideas that need debate start as a [GitHub Discussion](https://github.com/jumperck/Reminders/discussions).
2. **PRD**: feature-sized work gets a one-page PRD in [docs/product/](docs/product/README.md) capturing problem, users, success criteria, and scope before implementation.
3. **ADR**: decisions that shape architecture, workflow, or tooling are recorded in [docs/adr/](docs/adr/README.md) before implementation.
4. **Issue**: every piece of work maps to a GitHub Issue with explicit acceptance criteria. Check open issues before proposing new work.
5. **Branch**: short-lived branch off `main`, named `<type>/<short-description>` (e.g. `feat/redis-cache`).
6. **PR**: small and focused, follows the PR template, references the issue (`Closes #123`).
7. **Review**: maintainer review is required (CODEOWNERS); CI must be green.
8. **Merge**: squash merge onto `main`, which must always stay releasable.

Small fixes can skip steps 1-3 and start at the issue. See [ADR-0001](docs/adr/0001-development-workflow.md) and [ADR-0006](docs/adr/0006-product-docs-and-prd-workflow.md) for the reasoning behind this workflow.

## Getting Started

1. Fork the repository on GitHub
2. Clone your fork locally
3. Create a new branch for your feature or bugfix
4. Make your changes
5. Test your changes
6. Submit a pull request

## Development Setup

### Prerequisites

- Docker & Docker Compose
- .NET SDK 8.0
- Node.js 18+
- Git

### Initial Setup

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/Reminders.git
cd Reminders

# Set up environment variables
cp .env.example .env
# Edit .env and configure your settings

# Start infrastructure services
docker compose --profile api up postgres ganache -d

# Build the API
cd src/server/api/dotnet/Reminders.Api
dotnet build

# Install React dependencies
cd ../../../../app/reactjs/reminders-app
npm install
```

## Project Structure

```text
Reminders/
├── blockchain/                    # Hardhat smart contracts
│   ├── contracts/                # Solidity contracts
│   ├── scripts/                  # Deployment scripts
│   └── test/                     # Contract tests
├── docs/                          # GitHub Pages documentation
├── infrastructure/                # Nginx configs, k6 tests
├── src/
│   ├── app/
│   │   ├── dotnet/                # ASP.NET MVC application
│   │   └── reactjs/               # Next.js React application
│   ├── server/
│   │   ├── api/
│   │   │   ├── dotnet/            # ASP.NET Core Web API
│   │   │   ├── go/                # Go (Gin) API
│   │   │   └── cpp/               # C++ API
│   │   └── services/dotnet/       # Migration runner service
│   └── test/
│       ├── cypress/               # E2E tests
│       └── server/dotnet/         # API unit tests
```

## Database Migrations

### Important: Dual Database Support

The project supports both **PostgreSQL** (default) and **SQL Server**. Migrations are stored separately:

- PostgreSQL: `src/server/api/dotnet/Reminders.Api/Layers/Data/EntityFramework/Postgres/Migrations/`
- SQL Server: `src/server/api/dotnet/Reminders.Api/Layers/Data/EntityFramework/SqlServer/Migrations/`

### Expected Behavior

Migrations are applied by a **dedicated migration runner service** (`src/server/services/dotnet/Reminders.MigrationsRunner/`), not by the API itself. Docker Compose starts `postgres`, waits for it to report healthy, then runs `migrations` to completion before either API instance starts. When PostgreSQL is the active provider (default), the runner also attempts the SQL Server migration set and logs an expected failure for it - **this is expected and harmless**. Only the PostgreSQL migrations are actually applied.

### How the Runner Works

- **Technology**: .NET 8.0 console app with an embedded HTTP health endpoint (`http://localhost:8081/healthz`, development mode only). Runs once per deployment and exits, it is not a long-running service.
- **Order**: `postgres` starts and reports healthy, then `migrations` runs with retry logic (exponential backoff, 5 attempts, 2s base delay), and once it exits with code 0 both `dotnet-api` and `go-api` start (each depends on `migrations` with `condition: service_completed_successfully`).
- **Health signal**: the runner exposes HTTP 500 while running and HTTP 200 once migrations succeed.

### Running Migrations Manually

**Local development (without Docker):**

```bash
cd src/server/services/dotnet/Reminders.MigrationsRunner

export ConnectionStrings__DefaultConnection="Host=localhost;Database=Reminders;Username=postgres;Password=yourpassword"
export DatabaseProvider="Postgres"

dotnet run

# check health endpoint in another terminal
curl http://localhost:8081/healthz
```

**Via Docker Compose (recommended):**

```bash
docker compose --profile all up -d

# check migration runner logs
docker compose logs migrations

# verify it completed successfully - should show "Exited (0)"
docker compose ps migrations
```

### Runner Configuration

Settings live in `appsettings.json`, or override via environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Database=Reminders;..."
  },
  "DatabaseProvider": "Postgres",
  "MigrationRunner": {
    "MaxRetryAttempts": 5,
    "RetryBaseDelaySeconds": 2
  }
}
```

```bash
ConnectionStrings__DefaultConnection="..."
DatabaseProvider="Postgres"
MigrationRunner__MaxRetryAttempts=5
MigrationRunner__RetryBaseDelaySeconds=2
```

### Creating New Migrations

Always specify the context and output directory:

**For PostgreSQL:**

```bash
dotnet ef migrations add MigrationName \
  --project src/server/api/dotnet/Reminders.Api \
  --context RemindersContext \
  --output-dir Layers/Data/EntityFramework/Postgres/Migrations
```

**For SQL Server:**

```bash
dotnet ef migrations add MigrationName \
  --project src/server/api/dotnet/Reminders.Api \
  --context RemindersContext \
  --output-dir Layers/Data/EntityFramework/SqlServer/Migrations
```

### Troubleshooting Migrations

- **Runner fails to start**: check `docker compose logs migrations`. Usual causes are the database not being ready yet, an invalid connection string, or missing environment variables.
- **Migrations fail to apply**: check the logs for the detailed error, then test the connection directly with `docker compose exec postgres psql -U root -d Reminders -c "\dt"`. As a last resort, `docker compose down -v && docker compose --profile all up -d` resets the database (this deletes all data).
- **API won't start after a migration failure**: the runner must exit with code 0 for the APIs to start. Check `docker compose ps migrations`; if the exit code is non-zero, fix the underlying issue and run `docker compose up migrations -d --force-recreate`.

## Making Changes

### Branching Strategy

Trunk-based development (see [ADR-0001](docs/adr/0001-development-workflow.md)):

- `main` is the trunk and must always be releasable
- Work in short-lived branches off `main`, named `<type>/<short-description>` (e.g. `feat/redis-cache`, `fix/async-service-chain`)
- Rebase on `main` before merging; squash merge keeps trunk history linear

### Commit Messages

All commits follow [Conventional Commits](https://www.conventionalcommits.org/): `<type>(<scope>): <description>`, imperative mood, lowercase, no trailing period. See [CLAUDE.md](CLAUDE.md#conventional-commits) for the full list of types and scopes.

```text
feat(api): add redis caching for reminder queries
fix(react): resolve reminder edit route crash
docs: update api documentation
test(api): add unit tests for reminder service
```

### Code Style

- **.NET**: Follow Microsoft's C# coding conventions
- **TypeScript/React**: Follow the existing ESLint configuration
- **Solidity**: Follow Solidity style guide
- **UI**: changes follow the [design system](docs/design/reminders-redesign/design-system.md)

## Testing

### Run All Tests

```bash
# .NET API Integration Tests (requires running API)
# Start API first:
docker compose --profile api up postgres ganache -d
cd src/server/api/dotnet/Reminders.Api
dotnet run &

# Then run tests:
cd src/test/server/dotnet/Reminders.Api.Test
dotnet test

# React Unit Tests (all passing ✅)
cd src/app/reactjs/reminders-app
npm test

# Cypress E2E Tests
cd src/test/cypress
npm run cy:run

# Blockchain Tests (all passing ✅)
cd blockchain
npm test
```

### Test Status

| Test Suite | Status | Notes |
| ------------ | -------- | ------- |
| **React/Jest** | ✅ Passing | 10 tests, 99.6% coverage |
| **Blockchain/Hardhat** | ✅ Passing | 10 tests |
| **Cypress E2E** | ✅ Passing | Runs against deployed app |
| **.NET Integration** | ⚠️ Manual | Requires API running on port 5000 |
| **.NET Selenium** | ⚠️ Manual | Requires browser drivers installed |

### Before Submitting

Ensure all tests pass:

- Unit tests
- Integration tests
- E2E tests
- Linting checks

## Pull Request Process

1. **Update Documentation**: If you've changed APIs or added features, update the relevant documentation

2. **Add Tests**: Include appropriate test coverage for your changes

3. **Update CHANGELOG**: Add a brief description of your changes (if applicable)

4. **Ensure CI Passes**: All GitHub Actions workflows must pass

5. **Description**: Provide a clear description of:
   - What changes you made
   - Why you made them
   - Any breaking changes
   - Screenshots (for UI changes)

6. **Link Issues**: Reference any related issues using `Fixes #123` or `Closes #456`

### PR Title Format

PR titles follow the same Conventional Commits format as commits and are validated by the PR Title Check workflow:

```text
<type>(<scope>): <description>

Example: feat(api): add email notification for reminders
```

## Development Tips

### Running Individual Services

```bash
# Just the database
docker compose --profile api up postgres -d

# Just the blockchain
docker compose up ganache -d

# API locally (with hot reload)
cd src/server/api/dotnet/Reminders.Api
dotnet watch run

# React app (with hot reload)
cd src/app/reactjs/reminders-app
npm run dev
```

### Debugging

- **.NET API**: Use VS Code with C# extension or Visual Studio
- **React**: Use browser DevTools and React DevTools
- **Blockchain**: Use Hardhat console and Ganache logs

### Common Issues

#### Port Already in Use

```bash
# Find and kill process using a port (example: 5000)
lsof -ti:5000 | xargs kill -9

# Or stop all containers and try again
docker compose down
docker compose --profile all up -d
```

#### Docker Build Fails or Takes Too Long

The first build can take 10+ minutes since Docker has to download base images (.NET, Node.js, PostgreSQL, Nginx, Ganache) and install all dependencies. Subsequent builds are much faster thanks to layer caching. If a build is actually failing:

```bash
# Clean Docker cache
docker system prune -a
docker compose down -v
```

#### Database Connection Issues

- Ensure PostgreSQL container is running and healthy: `docker compose ps` / `docker logs reminders-postgres`
- Check `.env` file has correct credentials
- Verify port 5432 is not blocked, and give PostgreSQL a few seconds to finish initializing

#### React App Shows API Connection Error

- Ensure the API is reachable: `curl http://localhost:9999/health`
- Check CORS configuration in your `.env` file
- Verify Nginx is running: `docker compose ps reminders-nginx`

## Questions or Need Help?

- Open an issue for bugs or feature requests
- Start a discussion for questions
- Check existing issues for solutions

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

---

Thank you for contributing to Reminders! 🎉
