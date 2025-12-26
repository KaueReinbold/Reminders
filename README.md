# Reminders

A simple and intuitive web application for managing your daily reminders.

🚀 **Live Demo**: [https://kauereinbold.github.io/Reminders](https://kauereinbold.github.io/Reminders)

## Features

- **User-friendly Interface**: Easily create, edit, and delete reminders with a clean and intuitive user interface.
- **Persistent Storage**: Reminders are stored securely, ensuring you never lose your important tasks.
- **Customization**: Customize your reminders with different colors, tags, and priority levels.
- **Reminders Notification**: Receive timely reminders to stay on top of your tasks.

## Learning Project

Please note that as a learning project, the code here may not follow best practices at all times as it's a process of learning and improving. Feedback and suggestions are always welcome!

## Prerequisites

Before you begin, ensure you have the following installed on your system:

### Required

- **Docker** (v20.10 or higher) - [Install Docker](https://docs.docker.com/get-docker/)
- **Docker Compose** (v2.0 or higher) - Usually included with Docker Desktop
- **Git** - For cloning the repository

### Optional (for local development without Docker)

- **.NET SDK 8.0** - [Download .NET](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** and **npm** - [Download Node.js](https://nodejs.org/)
- **PostgreSQL 13+** - For local database development

### Port Requirements

Make sure the following ports are available on your system:

- **5000, 5001** - API instances
- **5050** - MVC application
- **3000** - React application
- **9999** - Nginx load balancer
- **5432** - PostgreSQL database
- **8545** - Ganache blockchain node

## Quick Start

Get the application running in 3 simple steps:

```bash
# 1. Clone the repository
git clone https://github.com/KaueReinbold/Reminders.git
cd Reminders

# 2. Set up environment variables
cp .env.example .env
# Edit .env and replace YOUR_PASSWORD_HERE with a secure password

# 3. Start all services with Docker Compose
docker compose --profile all up -d

# Wait a few minutes for all services to build and start...
```

### Access the Applications

Once all containers are running:

- **React App**: [http://localhost:3000](http://localhost:3000)
- **MVC App**: [http://localhost:5050](http://localhost:5050)
- **API (via Load Balancer)**: [http://localhost:9999](http://localhost:9999)
- **API Instance 1**: [http://localhost:5000](http://localhost:5000)
- **API Instance 2**: [http://localhost:5001](http://localhost:5001)

### Verify Everything is Running

```bash
# Check container status
docker compose ps

# View logs
docker compose logs -f

# Stop all services
docker compose --profile all down
```

## Project Status

[![Coverage Status](https://coveralls.io/repos/github/KaueReinbold/Reminders/badge.svg?branch=main)](https://coveralls.io/github/KaueReinbold/Reminders?branch=main)

### Build Status

  [![dotnet - build - pull request](https://github.com/KaueReinbold/Reminders/actions/workflows/dotnet-pull-request.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/dotnet-pull-request.yml)

  [![docker - build - pull request](https://github.com/KaueReinbold/Reminders/actions/workflows/docker-pull-request.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/docker-pull-request.yml)

  [![React - build and test](https://github.com/KaueReinbold/Reminders/actions/workflows/react-pull-request.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/react-pull-request.yml)

  [![Cypress E2E Tests](https://github.com/KaueReinbold/Reminders/actions/workflows/cypress-e2e.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/cypress-e2e.yml)

  [![Deploy to GitHub Pages](https://github.com/KaueReinbold/Reminders/actions/workflows/deploy-pages.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/deploy-pages.yml)

### Docker Hub

Reminders MVC:

  [![Docker Hub - Reminders MVC)](https://img.shields.io/docker/pulls/kauereinbold/reminders-mvc.svg)](https://hub.docker.com/r/kauereinbold/reminders-mvc)

Reminders API:

  [![Docker Hub - Reminders API)](https://img.shields.io/docker/pulls/kauereinbold/reminders-api.svg)](https://hub.docker.com/r/kauereinbold/reminders-api)

## Testing

The Reminders application includes comprehensive testing to ensure reliability and functionality:

### Unit Testing with Jest

The ReactJS application includes unit tests using Jest and React Testing Library:

- **Location**: `src/app/reactjs/reminders-app/src/`
- **Coverage**: Components, hooks, API layers, and pages
- **Status**: ✅ All tests passing (99.6% coverage)
- **Run tests**: `npm test` (in the React app directory)
- **Coverage report**: `npm test -- --coverage`

### Blockchain Testing with Hardhat

Smart contract tests using Hardhat and Chai:

- **Location**: `blockchain/test/`
- **Coverage**: Lock and Reminders contracts
- **Status**: ✅ All tests passing (10 tests)
- **Run tests**: `npm test` (in the blockchain directory)

### Quick Test All

Run the automated test suite:

```bash
# Run all available tests
./run-tests.sh
```

This will run:

- ✅ React/Jest unit tests (61 tests)
- ✅ Blockchain/Hardhat tests (10 tests)
- ⚠️ .NET integration tests (if API is running)

### End-to-End Testing with Cypress

Comprehensive E2E tests validate critical user interactions against the deployed application:

- **Location**: `src/test/cypress/`
- **Testing Environment**: Tests run against the deployed GitHub Pages application at `https://kaueereinbold.github.io/Reminders`
- **Coverage**: List, Create, Edit, Delete operations
- **Test categories**:
  - List functionality (viewing reminders)
  - Creation workflow (create new reminder)
  - Editing workflow (modify existing reminder)  
  - Deletion workflow (remove reminder with confirmation)
  - Integration tests (full user journeys)
- **Run tests**:
  - Interactive: `npm run cy:open` (in Cypress directory)
  - Headless: `npm run cy:run` (in Cypress directory)
  - Against local dev: `CYPRESS_baseUrl=http://localhost:3000 npm run cy:run`

For detailed testing setup and usage instructions, see:

- [Cypress Testing README](src/test/cypress/README.md)

### .NET Integration Tests

**Note**: Integration tests require the API to be running on `http://localhost:5000`.

**To run integration tests:**

```bash
# 1. Start the API and database
docker compose up postgres ganache -d
cd src/server/api/dotnet/Reminders.Api
dotnet run

# 2. In a new terminal, run the tests
cd src/test/server/dotnet/Reminders.Api.Test
dotnet test
```

**Selenium Tests**: The MVC Selenium tests require Chrome/Firefox drivers and browsers installed. These are primarily for local development and may be skipped in CI environments.

### Continuous Integration

All tests run automatically on pull requests:

- Jest unit tests via GitHub Actions
- Cypress E2E tests via GitHub Actions
- Coverage reporting and artifact collection

## Docker Compose Configuration

This section describes the Docker Compose configuration for deploying the Reminders application.

## Usage

To deploy the Reminders application using Docker Compose, follow these steps:

1. Clone the repository: `git clone https://github.com/KaueReinbold/Reminders.git`
2. Navigate to the project directory: `cd Reminders`
3. Run Docker Compose: `docker compose up -d`

The application can be accessed using the following URLs:

- **API 1:** [http://localhost:5000](http://localhost:5000)
- **API 2:** [http://localhost:5001](http://localhost:5001)
- **MVC:** [http://localhost:5050](http://localhost:5050)
- **Nginx Load Balancer:** [http://localhost:9999](http://localhost:9999)

Please note that the exact URLs may vary based on your specific configuration and environment. Adjust them accordingly.

## Troubleshooting

### Common Issues

#### Database Migration Warning Message

**Symptom**: When starting the API, you see an error about failed SQL Server migration.

**Cause**: The project supports both PostgreSQL and SQL Server. When using PostgreSQL (default), the SQL Server migration will fail.

**Solution**: This is **expected behavior** and harmless. The PostgreSQL migration succeeds, and you can safely ignore the SQL Server migration error. The application will run normally.

#### Ports Already in Use

**Symptom**: Docker Compose fails with "port is already allocated" error.

**Solution**:

```bash
# Check what's using the port (example for port 5432)
lsof -ti:5432 | xargs kill -9

# Or stop all containers and try again
docker compose down
docker compose --profile all up -d
```

#### Docker Build Takes Too Long

**Symptom**: Initial build takes 10+ minutes.

**Solution**: This is normal for the first build. Docker needs to:

- Download base images (.NET, Node.js, PostgreSQL, Nginx, Ganache)
- Install all dependencies
- Build all services

Subsequent builds will be much faster thanks to Docker's layer caching.

#### Cannot Connect to Database

**Symptom**: API fails to start with database connection errors.

**Solution**:

1. Ensure PostgreSQL is running: `docker compose ps`
2. Check your `.env` file has correct password
3. Verify the database container is healthy: `docker logs reminders-postgres`
4. Wait a few seconds for PostgreSQL to fully initialize

#### React App Shows API Connection Error

**Symptom**: Frontend can't connect to the API.

**Solution**:

1. Ensure API is running: `curl http://localhost:9999/health`
2. Check CORS configuration in your `.env` file
3. Verify Nginx is running: `docker compose ps reminders-nginx`

### Getting Help

- Check [CONTRIBUTING.md](CONTRIBUTING.md) for development guidelines
- Check the [Issues](https://github.com/KaueReinbold/Reminders/issues) page for known problems
- Open a new issue if you encounter a bug

## Contributing

We welcome contributions! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on:

- Setting up your development environment
- Project structure and architecture
- Database migration guidelines
- Testing requirements
- Pull request process

## Support

If you encounter any issues or have suggestions, we encourage you to open an issue on the [GitHub Issues](https://github.com/KaueReinbold/Reminders/issues) page.
