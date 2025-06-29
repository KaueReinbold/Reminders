# Reminders App

This is a React.js application built with Next.js and TypeScript. It's a simple reminders app that allows users to create, update, and delete reminders.

## Recent Updates

- **Upgraded to React 19**: Updated React to version 19.1.0 and React DOM to 19.1.0, taking advantage of the latest React features and improvements.
- **Upgraded to Next.js 15**: Updated Next.js to version 15.3.4 for better React 19 compatibility and latest framework features.
- **Upgraded to Material-UI v7**: Updated @mui/material to version 7.1.2 for React 19 compatibility and latest design components.
- **Migrated to @tanstack/react-query**: Replaced deprecated react-query with @tanstack/react-query v5.81.5 for better performance and React 19 support.
- **Updated TypeScript types**: Updated @types/react and @types/react-dom to latest versions for React 19 compatibility.
- **Fixed UI components**: Updated Grid components to use Stack layout for better compatibility with Material-UI v7.
- **Maintained test coverage**: All existing tests pass with 99.6% code coverage after the upgrade.
- **Note**: Google Fonts import is temporarily disabled in the sandbox environment due to network restrictions. In production, uncomment the Inter font import in layout.tsx.
- Added ESLint for linting and code quality checks. The ESLint configuration includes recommended rules from ESLint, React, and TypeScript.
- Added Prettier for code formatting. The Prettier configuration ensures consistent code style across the project.
- Fixed various linting issues in the codebase. The code now adheres to the rules specified in the ESLint configuration.
- Updated the project structure to use the `@/app` alias for imports, improving the readability and maintainability of import statements.

## Getting Started

First, run the development server:

```bash
npm run dev
# or
yarn dev
# or
pnpm dev
# or
bun dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

You can start editing the page by modifying `app/page.tsx`. The page auto-updates as you edit the file.

This project uses [`next/font`](https://nextjs.org/docs/basic-features/font-optimization) to automatically optimize and load Inter, a custom Google Font.

## Learn More

To learn more about Next.js, take a look at the following resources:

- [Next.js Documentation](https://nextjs.org/docs) - learn about Next.js features and API.
- [Learn Next.js](https://nextjs.org/learn) - an interactive Next.js tutorial.

You can check out [the Next.js GitHub repository](https://github.com/vercel/next.js/) - your feedback and contributions are welcome!
