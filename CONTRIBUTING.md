# Contributing to Reminders

Thank you for your interest in contributing to the Reminders project! This document provides guidelines and information for contributors.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Database Migrations](#database-migrations)
- [Making Changes](#making-changes)
- [Testing](#testing)
- [Pull Request Process](#pull-request-process)

## Code of Conduct

This is a learning project, and we welcome contributions from developers of all skill levels. Please be respectful, constructive, and supportive of others.

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

## Making Changes

### Branching Strategy

- `main` - Production-ready code
- `develop` - Development branch (if used)
- `feature/your-feature-name` - New features
- `bugfix/issue-description` - Bug fixes
- `hotfix/critical-fix` - Urgent production fixes

### Commit Messages

Use clear, descriptive commit messages:

```text
feat: Add reminder notification feature
fix: Resolve database connection timeout
docs: Update API documentation
test: Add unit tests for reminder service
refactor: Improve error handling
```

### Code Style

- **.NET**: Follow Microsoft's C# coding conventions
- **TypeScript/React**: Follow the existing ESLint configuration
- **Solidity**: Follow Solidity style guide

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

```text
[Type] Brief description

Types: feat, fix, docs, test, refactor, chore
Example: [feat] Add email notification for reminders
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
# Find and kill process using port 5000
lsof -ti:5000 | xargs kill -9
```

#### Docker Build Fails

```bash
# Clean Docker cache
docker system prune -a
docker compose down -v
```

#### Database Connection Issues

- Ensure PostgreSQL container is running: `docker compose ps`
- Check `.env` file has correct credentials
- Verify port 5432 is not blocked

## Questions or Need Help?

- Open an issue for bugs or feature requests
- Start a discussion for questions
- Check existing issues for solutions

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

---

Thank you for contributing to Reminders! 🎉
