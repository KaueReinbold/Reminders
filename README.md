# Reminders

A simple and intuitive web application for managing your daily reminders.

## Features

- **User-friendly Interface**: Easily create, edit, and delete reminders with a clean and intuitive user interface.
- **Persistent Storage**: Reminders are stored securely, ensuring you never lose your important tasks.
- **Customization**: Customize your reminders with different colors, tags, and priority levels.
- **Reminders Notification**: Receive timely reminders to stay on top of your tasks.

# Learning Project

Please note that as a learning project, the code here may not follow best practices at all times as it's a process of learning and improving. Feedback and suggestions are always welcome!

## Project Status

[![Coverage Status](https://coveralls.io/repos/github/KaueReinbold/Reminders/badge.svg?branch=main)](https://coveralls.io/github/KaueReinbold/Reminders?branch=main)

### Build Status

  [![dotnet - build - pull request](https://github.com/KaueReinbold/Reminders/actions/workflows/dotnet-pull-request.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/dotnet-pull-request.yml)

  [![docker - build - pull request](https://github.com/KaueReinbold/Reminders/actions/workflows/docker-pull-request.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/docker-pull-request.yml)

  [![React - build and test](https://github.com/KaueReinbold/Reminders/actions/workflows/react-pull-request.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/react-pull-request.yml)

  [![Cypress E2E Tests](https://github.com/KaueReinbold/Reminders/actions/workflows/cypress-e2e.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/cypress-e2e.yml)

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
- **Run tests**: `npm test` (in the React app directory)
- **Coverage report**: `npm test -- --coverage`

### End-to-End Testing with Cypress

Comprehensive E2E tests validate critical user interactions:

- **Location**: `src/test/cypress/`
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

For detailed testing setup and usage instructions, see:
- [Cypress Testing README](src/test/cypress/README.md)

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
3. Run Docker Compose: `docker-compose up -d`

The application can be accessed using the following URLs:

- **API 1:** [http://localhost:5000](http://localhost:5000)
- **API 2:** [http://localhost:5003](http://localhost:5003)
- **MVC:** [http://localhost:5001](http://localhost:5001)
- **Nginx Load Balancer:** [http://localhost:9999](http://localhost:9999)

Please note that the exact URLs may vary based on your specific configuration and environment. Adjust them accordingly.

## Support

If you encounter any issues or have suggestions, we encourage you to open an issue on the [GitHub Issues](https://github.com/KaueReinbold/Reminders/issues) page.
