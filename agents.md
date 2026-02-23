# Reminders Project - AI Agent Guide

> **Version**: 1.0.0  
> **Last Updated**: January 2026  
> **Agent Compatibility**: GitHub Copilot, Claude, GPT-4+

## Project Overview

A full-stack learning project for managing reminders with multiple client interfaces, load-balanced APIs, and blockchain integration.

**Tech Stack**: .NET 8.0, Go (Gin), Next.js 15, PostgreSQL, Solidity, Docker Compose

**Repository**: Multi-workspace (specs + application)

## Quick Start for Agents

### Initial Setup

```bash
# Clone and setup
cd /home/kaue/repos/reminders/Reminders
cp .env.example .env

# Start all services
docker compose --profile all up --build -d

# View logs
docker compose logs -f dotnet-api go-api migrations
```

### Critical Paths

- **API (.NET)**: `src/server/api/dotnet/Reminders.Api/`
- **API (Go)**: `src/server/api/go/reminders-api/`
- **Migration Runner**: `src/server/services/dotnet/Reminders.MigrationsRunner/`
- **React Frontend**: `src/app/reactjs/reminders-app/`
- **Smart Contracts**: `blockchain/contracts/Reminders.sol`
- **Infrastructure**: `infrastructure/nginx.conf`, `docker-compose.yml`

## Architecture Patterns

### Layered Architecture (.NET API)

```
Controllers → Application Layer → Domain Layer → Data Layer
```

**Key Principles**:
- Controllers depend on `IRemindersService` (never direct repository access)
- Services depend on `IRemindersRepository` (domain contracts)
- Repositories implement EF Core `DbContext` operations
- Dependency injection configured per layer (see `DependencyInjection.cs` files)

**Namespaces**:
- `Reminders.Api.*` - Controllers and API configuration
- `Reminders.Application.*` - Business logic, services, DTOs
- `Reminders.Domain.*` - Domain models, contracts
- `Reminders.Infrastructure.*` - Data access, external services

### Database Migrations

**CRITICAL**: Migrations are handled by a **dedicated service** (`Reminders.MigrationsRunner`), NOT by the API.

**Execution Flow**:
```
postgres (healthy) → migrations (completes) → APIs (start)
```

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

**Running Migrations**:
- **Docker Compose (recommended)**: Automatic via `migrations` service
- **Manual**: `cd src/server/services/dotnet/Reminders.MigrationsRunner && dotnet run`

**Important Notes**:
- Migration runner has retry logic (5 attempts, exponential backoff)
- Health endpoint at `/healthz:8081` (dev only)
- Provider-specific migrations applied automatically
- Expected: SQL Server migration errors when using PostgreSQL (harmless)

### Load Balancing

Two backend instances behind Nginx:
- **dotnet-api**: .NET Core with full blockchain integration
- **go-api**: Lightweight Go (Gin) implementation

Both share the same PostgreSQL database. Load balanced round-robin on port 9999.

**Server Identification**: Check `X-Server` response header (`dotnet` or `go`)

## Coding Standards

### Conventional Commits

**Required Format**: `<type>[optional scope]: <description>`

**Common Types**:
- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code restructuring
- `docs`: Documentation only
- `test`: Test changes
- `chore`: Maintenance tasks
- `perf`: Performance improvement

**Examples**:
```
feat(api): add bulk reminder import
fix(migrations): handle connection timeout
docs: update agents.md with blockchain guide
refactor(repository): extract query builder
```

### .NET Code Conventions

- **Async/await**: Always use for I/O operations
- **Nullable reference types**: Enabled by default
- **Validation**: FluentValidation for DTOs
- **Mapping**: AutoMapper for entity ↔ ViewModel
- **DI Registration**: Use extension methods (`RegisterApplicationServices`)
- **Error Handling**: Global exception handler via middleware

**Example Controller**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class RemindersController : ControllerBase
{
    private readonly IRemindersService _service;

    public RemindersController(IRemindersService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reminders = await _service.GetAllAsync();
        return Ok(reminders);
    }
}
```

### React Conventions

- **Next.js App Router**: Server components by default
- **Client Components**: Mark with `'use client'` directive
- **Data Fetching**: `@tanstack/react-query` for server state
- **Styling**: Material-UI v7 components
- **Path Alias**: Use `@/app` for imports
- **TypeScript**: Strict mode enabled

**Example Client Component**:
```tsx
'use client';

import { useRouter } from 'next/navigation';
import { useRemindersContext } from '@/app/hooks';

export default function EditClient() {
  const { onUpdateReminder } = useRemindersContext();
  const router = useRouter();
  
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const status = await onUpdateReminder();
    if (status === ReminderActionStatus.Success) {
      router.push('/');
    }
  };
  
  return <form onSubmit={handleSubmit}>...</form>;
}
```

### Go API Conventions

- **Framework**: Gin web framework
- **Repository Pattern**: Direct PostgreSQL access
- **Error Handling**: Gin's built-in error handling
- **Server Header**: Always add `X-Server: go` header
- **Health Endpoint**: `GET /health` returns `"Healthy"`

## Environment Configuration

**Required Variables** (`.env`):

```bash
# Database
DATABASE_PROVIDER=Postgres  # or SqlServer
CONNECTION_STRING=Host=reminders-postgres;Database=Reminders;Username=postgres;Password=YOUR_PASSWORD

# Blockchain
BLOCKCHAIN_NODE_URL=http://reminders-blockchain:8545
BLOCKCHAIN_PRIVATE_KEY=0xc87509a1c067bbde78beb793e6fa76530b6382a4c0241e5e4a9ec0a0f44dc0d3
BLOCKCHAIN_CONTRACT_ADDRESS=0x5FbDB2315678afecb367f032d93F642f64180aa3

# API
CORS_ORIGINS=http://localhost:3000,http://localhost:5000
API_BASE_URL=http://reminders-nginx:9999
```

## Docker Compose Profiles

Control which services start using profiles:

```bash
# All services (default for development)
docker compose --profile all up -d

# API stack only (2 APIs + Postgres + Ganache + Nginx)
docker compose --profile api up -d

# Debug mode (with remote debugger)
docker compose --profile debug -f docker-compose.yml -f docker-compose.debug.yml up -d

# Production mode
docker compose --profile production -f docker-compose.yml -f docker-compose.production.yml up -d
```

**Available Profiles**:
- `all` - Full stack (APIs, frontends, database, blockchain, load balancer)
- `api` - Backend only
- `debug` - Development with debugger support
- `production` - Optimized builds

## Common Agent Tasks

### Adding a New API Endpoint

1. **Domain Model** (if needed): Update `Layers/Domain/Models/Reminder.cs`
2. **Contract**: Add method to `IRemindersRepository` in `Layers/Domain/Contracts/`
3. **Repository**: Implement in `Layers/Data/EntityFramework/RemindersRepository.cs`
4. **Service**: Add business logic to `RemindersService.cs`
5. **Controller**: Create endpoint in `Controllers/RemindersController.cs`
6. **DTO/Validation**: Add ViewModel and FluentValidation rules

**Example Flow**:
```csharp
// 1. Controller
[HttpPost("bulk")]
public async Task<IActionResult> CreateBulk([FromBody] List<ReminderViewModel> reminders)
{
    var result = await _service.CreateBulkAsync(reminders);
    return CreatedAtAction(nameof(GetAll), result);
}

// 2. Service
public async Task<List<ReminderDto>> CreateBulkAsync(List<ReminderViewModel> viewModels)
{
    var entities = _mapper.Map<List<Reminder>>(viewModels);
    await _repository.AddRangeAsync(entities);
    await _unitOfWork.CommitAsync();
    return _mapper.Map<List<ReminderDto>>(entities);
}

// 3. Repository
public async Task AddRangeAsync(IEnumerable<Reminder> reminders)
{
    await _context.Reminders.AddRangeAsync(reminders);
}
```

### Modifying Database Schema

1. **Update Entity**: Modify `Layers/Domain/Models/Reminder.cs`
2. **Create Migration**: Use `dotnet ef migrations add` command
3. **Test Locally**: Run migration runner or Docker Compose
4. **Verify**: Check `__EFMigrationsHistory` table

**Important**: Always create separate migrations for Postgres and SQL Server if supporting both.

### Adding React Components

1. Place in `src/app/reactjs/reminders-app/src/app/components/`
2. Export from `index.ts` barrel file
3. Use Material-UI components for consistency
4. Add tests in `.test.tsx` file (target 99%+ coverage)

### Blockchain Contract Changes

1. **Update Contract**: Modify `blockchain/contracts/Reminders.sol`
2. **Test**: `cd blockchain && npm test`
3. **Deploy**: `npm run deploy` (updates artifacts)
4. **Update .env**: Set new `BLOCKCHAIN_CONTRACT_ADDRESS`
5. **Restart Services**: `docker compose restart dotnet-api`

## Testing Guidelines

### .NET API Tests

```bash
# Start dependencies
docker compose up postgres ganache -d

# Run API
cd src/server/api/dotnet/Reminders.Api && dotnet run &

# Run tests
cd test/server/dotnet/Reminders.Api.Test && dotnet test
```

### React Tests

```bash
cd src/app/reactjs/reminders-app
npm test                    # Run all tests
npm test -- --coverage      # With coverage report
```

**Target**: 99%+ code coverage

### Blockchain Tests

```bash
cd blockchain
npm test                    # Hardhat tests with Chai
```

### E2E Tests

```bash
cd test/cypress
npm install
npm run cy:open            # Interactive mode
npm run cy:run             # Headless mode
```

## Debugging

### VS Code Tasks

Available via `Ctrl+Shift+P` → `Tasks: Run Task`:

- **Docker Compose**: `compose up`, `compose down`, `compose up debug`
- **Build**: `build`, `publish`
- **Entity Framework**: `ef migrations add`, `ef database update`

### Remote Debugging (.NET in Docker)

```bash
# Start in debug mode
docker compose --profile debug -f docker-compose.yml -f docker-compose.debug.yml up -d

# Attach debugger in VS Code (F5)
# Breakpoints will work in containerized API
```

### Viewing Logs

```bash
docker compose logs -f dotnet-api      # .NET API
docker compose logs -f go-api          # Go API
docker compose logs -f migrations      # Migration runner
docker compose logs -f nginx           # Load balancer
docker compose logs -f postgres        # Database
```

## Performance Considerations

### Load Testing

K6 scripts available in `infrastructure/k6/scripts/`:

```bash
cd infrastructure/k6
docker compose up -d                   # Start Grafana dashboard
k6 run scripts/load-test.js           # Run load test
```

### Database Query Optimization

- **Always use async**: EF Core operations should be async
- **Avoid N+1 queries**: Use `.Include()` for related entities
- **Projections**: Use `.Select()` to fetch only needed columns
- **Tracking**: Use `.AsNoTracking()` for read-only queries

**Example**:
```csharp
// Good
var reminders = await _context.Reminders
    .AsNoTracking()
    .Where(r => !r.IsDeleted)
    .Select(r => new ReminderDto 
    { 
        Id = r.Id, 
        Title = r.Title 
    })
    .ToListAsync();

// Bad (N+1 query)
var reminders = await _context.Reminders.ToListAsync();
foreach (var reminder in reminders)
{
    var details = await _context.Details.FirstAsync(d => d.ReminderId == reminder.Id);
}
```

## Troubleshooting

### Migration Failures

**Symptom**: APIs fail to start, migration runner exits with error

**Solutions**:
1. Check PostgreSQL is healthy: `docker compose ps postgres`
2. View migration logs: `docker compose logs migrations`
3. Verify connection string in `.env`
4. Manually run: `cd src/server/services/dotnet/Reminders.MigrationsRunner && dotnet run`

### CORS Errors

**Symptom**: Frontend can't call API (blocked by CORS policy)

**Solutions**:
1. Check `CORS_ORIGINS` in `.env` includes frontend URL
2. Verify `UseRemindersCors()` middleware is called in `Program.cs`
3. Ensure Nginx is forwarding headers correctly

### Blockchain Connection Issues

**Symptom**: API throws blockchain service errors

**Solutions**:
1. Verify Ganache is running: `docker compose ps reminders-blockchain`
2. Check contract address in `.env` matches deployed contract
3. Ensure private key has funds: `npm run count` in blockchain folder
4. Redeploy contract: `cd blockchain && npm run deploy`

### Load Balancer Issues

**Symptom**: Requests fail intermittently

**Solutions**:
1. Check both API instances are healthy: `docker compose ps dotnet-api go-api`
2. Test each API directly: `curl http://localhost:8080/health` (bypasses Nginx)
3. Review Nginx config: `infrastructure/nginx.conf`
4. Check Nginx logs: `docker compose logs nginx`

## Agent Best Practices

### Before Making Changes

1. **Read Context**: Review relevant files first (`read_file`)
2. **Search Codebase**: Use `semantic_search` or `grep_search` to understand patterns
3. **Check Tests**: Ensure existing tests pass before modifying
4. **Follow Conventions**: Match existing code style and architecture

### When Implementing Features

1. **Small Commits**: Break work into logical, reviewable commits
2. **Test First**: Write or update tests before implementation (TDD encouraged)
3. **Document**: Update relevant docs (README, agents.md, API docs)
4. **Run Locally**: Test changes in Docker Compose before committing

### Error Handling

1. **Graceful Degradation**: Handle missing services (e.g., blockchain unavailable)
2. **Meaningful Messages**: Provide actionable error messages
3. **Log Context**: Include request IDs, user context in logs
4. **Use Result Types**: Prefer `Result<T>` or `Either<L,R>` over throwing exceptions for expected failures

### Code Review Checklist

- [ ] Follows Conventional Commits format
- [ ] Tests added/updated (coverage maintained)
- [ ] No hardcoded credentials or secrets
- [ ] Async/await used for I/O operations
- [ ] Null checks for nullable types
- [ ] CORS configuration updated if new endpoints
- [ ] Migration created if schema changed
- [ ] Docker Compose services updated if new dependencies
- [ ] Documentation updated (README, agents.md, API docs)

## Resources

- **Project README**: `/home/kaue/repos/reminders/Reminders/README.md`
- **Contributing Guide**: `/home/kaue/repos/reminders/Reminders/CONTRIBUTING.md`
- **Specs Repository**: `/home/kaue/repos/reminders/reminders-specs/specs/`
- **API Documentation**: `http://localhost:8080/swagger` (when running)
- **Blockchain Explorer**: `http://localhost:8545` (Ganache)

## Contact & Support

This is a **learning project**. Code may not follow best practices everywhere. When contributing:

- **Prefer clarity over cleverness**
- **Explain trade-offs in comments**
- **Ask questions via GitHub Issues**
- **Incremental improvements are welcome**

---

**Agent Instructions**: This document is meant to provide context for AI agents working on this codebase. When in doubt, prioritize clarity and maintainability over optimization. Always test changes locally before committing.
