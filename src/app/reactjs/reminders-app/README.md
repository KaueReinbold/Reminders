# Reminders App (React / Next.js)

The React frontend for Reminders, built with Next.js App Router, TypeScript, and Material-UI.

## Stack

- **Next.js 15** (App Router) + **React 19**
- **TypeScript**, strict mode enabled
- **Material-UI v7** for components
- **@tanstack/react-query** for API data fetching
- **Jest** + **React Testing Library** for unit tests

## Features

- List, create, edit, and delete reminders
- Form validation on create/edit
- Toggle reminder "done" status
- Responsive layout via Material-UI

## Prerequisites

- Node.js 18+
- npm (or yarn/pnpm/bun)
- A running Reminders API reachable at the URL configured below (see the repository root [README](../../../../README.md) for starting the full stack via Docker Compose)

## Environment Variables

Copy `.env.example` to `.env` and set:

```bash
# Base URL the app calls for API requests
NEXT_PUBLIC_API_BASE_URL=http://localhost:9999
```

- For local development against the full Docker Compose stack, use the Nginx load balancer URL (`http://localhost:9999`).
- For a GitHub Pages / production deployment, point this at your deployed API's public URL.

## Demo Mode (mock API)

The app can run with no backend: `src/app/api/mock.ts` serves seeded reminders
from memory and implements the full CRUD contract, including the server-side
validation messages. A "Demo data" badge appears in the header while it is on,
and reloading the page restores the seed data.

`NEXT_PUBLIC_MOCK_API` switches it:

```bash
# Try the demo locally
NEXT_PUBLIC_MOCK_API=true npm run dev

# Or a static demo build, the same one GitHub Pages serves
NODE_ENV=production GITHUB_PAGES=true npm run build && npm run export
```

The GitHub Pages workflow builds with `GITHUB_PAGES=true`, which turns the flag
on automatically (see `next.config.js`), so the published demo at
[kauereinbold.github.io/Reminders](https://kauereinbold.github.io/Reminders) is
usable without a server. Every other build leaves the flag off and calls the API
at `NEXT_PUBLIC_API_BASE_URL` as usual. See
[ADR-0011](../../../../docs/adr/0011-react-mock-api-for-pages-demo.md).

## Local Development

```bash
# Install dependencies
npm install

# Set up environment
cp .env.example .env

# Start the dev server
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser. The page auto-updates as you edit files under `src/app/`.

## Running with Docker

The app is built and served via the project's root `docker-compose.yml`/`docker-compose.override.yml`:

```bash
# From the repository root
docker compose --profile all up react -d
```

`NEXT_PUBLIC_API_BASE_URL` is baked in at build time as a Docker build argument (see `Dockerfile`); set it via the root `.env` file before building.

## Testing

```bash
# Run unit tests (Jest + React Testing Library)
npm test

# Run tests with coverage
npm test -- --coverage
```

Tests live alongside the code they cover, e.g. `src/app/components/ReminderForm/index.test.tsx`.

## Build

```bash
npm run build
npm start
```

## Linting

```bash
npm run lint
```

## Project Structure

```text
src/app/
├── api/            # API client, in-browser mock, hooks, and types
├── components/     # Reusable UI components (AlertError, ReminderForm, ReminderDeleteModal)
├── constants/       # Shared constants
├── hooks/          # Custom React hooks (context, query client)
├── reminder/       # Route segments: list, create, edit (App Router)
├── services/       # Validation and other client-side services
└── util/           # Utility helpers
```
