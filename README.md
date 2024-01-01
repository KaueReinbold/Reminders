# Reminders

A simple and intuitive web application for managing your daily reminders.

## Features

- **User-friendly Interface**: Easily create, edit, and delete reminders with a clean and intuitive user interface.
- **Persistent Storage**: Reminders are stored securely, ensuring you never lose your important tasks.
- **Customization**: Customize your reminders with different colors, tags, and priority levels.
- **Reminders Notification**: Receive timely reminders to stay on top of your tasks.

## Project Status

[![Coverage Status](https://coveralls.io/repos/github/KaueReinbold/Reminders/badge.svg?branch=main)](https://coveralls.io/github/KaueReinbold/Reminders?branch=main)

### Build Status

  [![dotnet - build - pull request](https://github.com/KaueReinbold/Reminders/actions/workflows/dotnet-pull-request.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/dotnet-pull-request.yml)

  [![docker - build - pull request](https://github.com/KaueReinbold/Reminders/actions/workflows/docker-pull-request.yml/badge.svg)](https://github.com/KaueReinbold/Reminders/actions/workflows/docker-pull-request.yml)

### Docker Hub

Reminders MVC:

  [![Docker Hub - Reminders MVC)](https://img.shields.io/docker/pulls/kauereinbold/reminders-mvc.svg)](https://hub.docker.com/r/kauereinbold/reminders-mvc)

Reminders API:

  [![Docker Hub - Reminders API)](https://img.shields.io/docker/pulls/kauereinbold/reminders-api.svg)](https://hub.docker.com/r/kauereinbold/reminders-api)

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

## Profiles

Profiles are used to define different configurations for services based on specific roles. Here's a breakdown of how profiles are used:

- **Profiles Defined:**
  - For the `api` service: `profiles: - api - all`
  - For the `mvc` service: `profiles: - mvc - all`
  - For the `mssql` service: `profiles: - all - mvc - api`
  - For the `postgres` service: `profiles: - all`

- **Usage:**
  
To run the application with specific profiles, you can use the following Docker Compose command:
    
```bash
docker-compose --profile <profile_name_1> --profile <profile_name_2> up -d
```

Replace <profile_name_1>, <profile_name_2>, etc., with the desired profiles you want to apply. For example:

- To run the application with the API profile:

  ```bash
  docker-compose --profile api up -d
  ```

- To run the application with the MVC profile:
  
  ```bash
  docker-compose --profile mvc up -d
  ```

  - To run the application with multiple profiles:

  ```bash
  docker-compose --profile api --profile mvc up -d
  ```

## Support

If you encounter any issues or have suggestions, we encourage you to open an issue on the [GitHub Issues](https://github.com/KaueReinbold/Reminders/issues) page.
