# Reminders Go API

This service is a lightweight Go implementation of the Reminders API used for local development and integration tests.

## Overview

- Framework: Gin
- Database: PostgreSQL
- Healthcheck: `GET /health`

## Endpoints

- `GET /health`
  - Returns 200 and `Healthy` when the service is running.

- `GET /api/reminders`
  - Returns JSON array of reminders.

- `GET /api/reminders/count`
  - Returns `{ "count": <number> }`.

- `GET /api/reminders/:id`
  - Returns the reminder by `id` (uuid).

- `POST /api/reminders`
  - Creates a new reminder.
  - Expected JSON body:
    ```json
    {
      "title": "string",
      "description": "string",
      "limitDate": "2026-01-01T00:00:00Z",
      "isDone": false
    }
    ```

- `PUT /api/reminders/:id`
  - Updates an existing reminder. Same body shape as POST.

- `DELETE /api/reminders/:id`
  - Deletes the reminder.

Responses follow standard HTTP status codes: 200 for success, 201 for created, 400 for invalid input, 404 for not found, 500 for server errors.

## Database schema (used by repository)

The Go repository expects a `Reminders` table with columns similar to:

```
Id uuid PRIMARY KEY,
Title text,
Description text,
LimitDate timestamptz,
IsDone boolean,
IsDeleted boolean DEFAULT false
```

When using Docker Compose for local dev, ensure the Postgres service creates the `Reminders` database (e.g. `POSTGRES_DB=Reminders`).

## Environment variables

- `PostgresDefaultConnection` — PostgreSQL connection string, for example:

```
postgresql://root:password@reminders-postgres/Reminders?sslmode=disable
```

## Build & Run (local)

1. From the Go module directory:

```bash
cd src/server/api/go/reminders-api
go mod download
go build -o reminders-api ./cmd/app
./reminders-api
```

2. Docker (multi-stage Dockerfile present):

```bash
docker build -t reminders-go-api:local .
docker run --env PostgresDefaultConnection="$CONN" -p 8080:8080 reminders-go-api:local
```

The service listens on port `8080` inside the container. In the project's Docker Compose stack this is published on host port `5001` (see `docker-compose.override.yml`).

The Docker image contains a `wait-for-dotnet.sh` helper that can be used in Compose to delay start until the .NET API is healthy. See the project `docker-compose` for usage.

## Examples

The examples below assume the service is reachable directly on port `8080` (a local `docker run` or `go run`). When running via the project's Docker Compose stack, use port `5001` instead.

Create reminder:

```bash
curl -X POST http://localhost:8080/api/reminders \
  -H 'Content-Type: application/json' \
  -d '{"title":"Buy milk","description":"2 liters","limitDate":"2030-12-12T00:00:00Z","isDone":false}'
```

List reminders:

```bash
curl http://localhost:8080/api/reminders
```

Get by id:

```bash
curl http://localhost:8080/api/reminders/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

## Notes

- The service adds an `X-Server: go` response header to help distinguish responses when load-balanced with the .NET API.
- For production or CI, prefer running the service inside the project's Docker Compose stack which configures networking and DB initialization.
