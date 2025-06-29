# Reminders Cypress Testing

This directory contains Cypress end-to-end tests for the Reminders application, ensuring the reliability and functionality of critical components.

## ⚠️ CI Environment Notice

**Important**: Due to network restrictions in the CI environment, Cypress binary cannot be downloaded from `download.cypress.io`. The tests are configured to run using the official Cypress GitHub Action (`cypress-io/github-action@v6`) which handles the Cypress installation in CI environments. For local development, you can install and run Cypress normally.

## Overview

The Cypress test suite covers four main areas of functionality:
- **List**: Viewing and navigating reminders
- **Creation**: Creating new reminders
- **Editing**: Modifying existing reminders
- **Deletion**: Removing reminders with confirmation

## Project Structure

```
src/test/cypress/
├── cypress/
│   ├── e2e/                    # Test files
│   │   ├── reminders-list.cy.js      # List functionality tests
│   │   ├── reminder-create.cy.js     # Create functionality tests
│   │   ├── reminder-edit.cy.js       # Edit functionality tests
│   │   ├── reminder-delete.cy.js     # Delete functionality tests
│   │   └── reminders-integration.cy.js # Integration tests
│   ├── fixtures/               # Test data
│   │   ├── reminders.json            # Sample reminders data
│   │   ├── single-reminder.json     # Single reminder data
│   │   └── empty-reminders.json     # Empty list scenario
│   └── support/                # Support files
│       ├── commands.js               # Custom commands
│       └── e2e.js                   # Support configuration
├── cypress.config.js           # Cypress configuration
├── package.json               # Dependencies and scripts
└── README.md                  # This file
```

## Prerequisites

Before running Cypress tests, ensure the following:

1. **React Application**: The ReactJS Reminders app must be deployed to GitHub Pages at `https://kaueereinbold.github.io/Reminders` (for production tests) or running locally at `http://localhost:3000` (for development)
2. **API Backend**: The API should be available at `http://localhost:5000` (configured in cypress.config.js)
3. **Node.js**: Node.js version 16 or higher

## Installation

1. Navigate to the Cypress test directory:
   ```bash
   cd src/test/cypress
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

## Running Tests

### Interactive Mode (Cypress Test Runner)

Open Cypress Test Runner for interactive testing and debugging:

```bash
npm run cy:open
```

This will open the Cypress GUI where you can:
- Select and run individual test files
- See tests run in real-time
- Debug test failures
- Take screenshots and record videos

### Headless Mode (CI/Command Line)

Run all tests in headless mode:

```bash
npm run cy:run
```

Run tests in specific browsers:

```bash
# Chrome
npm run cy:run:chrome

# Firefox  
npm run cy:run:firefox
```

### Running Specific Test Categories

Use tags to run specific test categories:

```bash
# Run only list tests
npx cypress run --env grep="@list"

# Run only create tests
npx cypress run --env grep="@create"

# Run only edit tests
npx cypress run --env grep="@edit"

# Run only delete tests
npx cypress run --env grep="@delete"

# Run only integration tests
npx cypress run --env grep="@integration"
```

## Test Categories

### List Tests (`reminders-list.cy.js`)
- Display reminders list
- Handle loading states
- Navigate to create/edit pages
- Verify table structure and data

### Create Tests (`reminder-create.cy.js`)
- Display create form
- Create new reminders successfully
- Handle form validation errors
- Navigate back to home page
- Handle server errors

### Edit Tests (`reminder-edit.cy.js`)
- Display edit form with existing data
- Update reminders successfully
- Toggle done status
- Handle form validation errors
- Navigate back to home page

### Delete Tests (`reminder-delete.cy.js`)
- Display delete confirmation modal
- Cancel delete operation
- Delete reminders successfully
- Handle server errors
- Test modal accessibility

### Integration Tests (`reminders-integration.cy.js`)
- Complete CRUD journey
- API error handling
- Cross-page navigation
- Data persistence and validation
- Responsive design testing

## Custom Commands

The test suite includes custom Cypress commands for common operations:

- `cy.createReminder(title, description, limitDate)` - Create a new reminder
- `cy.editReminder(title, description, limitDate, isDone)` - Edit an existing reminder
- `cy.deleteReminder()` - Delete a reminder with confirmation
- `cy.goToCreateReminder()` - Navigate to create page
- `cy.goToEditReminder(id)` - Navigate to edit page for specific reminder
- `cy.goBack()` - Navigate back to home page
- `cy.mockRemindersAPI()` - Set up API mocks
- `cy.waitForAppReady()` - Wait for app to be fully loaded
- `cy.verifyReminderInList(id, title, description, limitDate, isDone)` - Verify reminder in list

## Configuration

Key configuration options in `cypress.config.js`:

- `baseUrl`: React app URL (default: `https://kaueereinbold.github.io/Reminders` for production, can be overridden with `CYPRESS_baseUrl` environment variable)
- `env.apiUrl`: API backend URL (default: `http://localhost:5000`)
- `viewportWidth/Height`: Test viewport dimensions
- `video`: Enable/disable video recording
- `screenshotOnRunFailure`: Enable/disable failure screenshots

### Running Tests Against Different Environments

**Against deployed GitHub Pages (default):**
```bash
npm run cy:run
```

**Against local development server:**
```bash
CYPRESS_baseUrl=http://localhost:3000 npm run cy:run
```

## API Mocking

Tests use Cypress intercepts to mock API responses, ensuring:
- Tests run independently of backend state
- Consistent test data
- Testing of error scenarios
- Fast test execution

## Test Data

Test fixtures in `cypress/fixtures/` provide:
- Sample reminder data for list tests
- Single reminder data for edit/delete tests
- Empty list scenarios
- Error response examples

## Continuous Integration

Cypress tests run automatically on pull requests and pushes to main against the deployed GitHub Pages application:

1. **GitHub Pages Deployment**: The React app is automatically deployed to GitHub Pages when changes are pushed to main
2. **Automatic Testing**: Cypress tests run against the deployed application at `https://kaueereinbold.github.io/Reminders`
3. **Test Results**: Screenshots and videos are collected as artifacts on test failures

### Local Development Testing

For local development testing against a development server:

1. **Install dependencies**:
   ```bash
   cd src/test/cypress && npm ci
   ```

2. **Start application** (ensure React app is running at localhost:3000)

3. **Run tests against local server**:
   ```bash
   CYPRESS_baseUrl=http://localhost:3000 npm run cy:run
   ```

4. **Collect artifacts**:
   - Screenshots: `cypress/screenshots/`
   - Videos: `cypress/videos/`
   - Coverage reports (if configured)

## Environment Variables

Configure tests using environment variables:

- `CYPRESS_baseUrl`: Override base URL
- `CYPRESS_apiUrl`: Override API URL
- `CYPRESS_viewportWidth`: Override viewport width
- `CYPRESS_viewportHeight`: Override viewport height

Example:
```bash
# Test against local development server
CYPRESS_baseUrl=http://localhost:3001 npm run cy:run

# Test against staging environment
CYPRESS_baseUrl=https://staging.example.com npm run cy:run

# Override API URL
CYPRESS_apiUrl=https://api.staging.example.com npm run cy:run
```

## Debugging

### Test Failures
1. Check screenshots in `cypress/screenshots/`
2. Review videos in `cypress/videos/`
3. Use `cy.debug()` or `cy.pause()` in tests
4. Run in interactive mode for step-by-step debugging

### Common Issues
1. **App not running**: Ensure React app is running at configured baseUrl
2. **API not available**: Ensure API backend is running and accessible
3. **Element not found**: Check selectors and wait conditions
4. **Timing issues**: Add appropriate waits or increase timeouts

## Extending Tests

### Adding New Tests
1. Create new test file in `cypress/e2e/`
2. Use existing custom commands
3. Follow naming convention: `feature-name.cy.js`
4. Add appropriate tags for test categorization

### Adding Custom Commands
1. Add command to `cypress/support/commands.js`
2. Follow existing patterns
3. Include error handling
4. Document usage in this README

### Adding Test Data
1. Create fixture file in `cypress/fixtures/`
2. Use JSON format
3. Reference in tests using `cy.fixture()`

## Best Practices

1. **Use data-testid attributes** for reliable element selection
2. **Mock API responses** for consistent, fast tests
3. **Use custom commands** for repeated operations
4. **Add meaningful assertions** to verify expected behavior
5. **Keep tests independent** - each test should be able to run alone
6. **Use descriptive test names** that explain what is being tested
7. **Group related tests** using `describe` blocks
8. **Clean up test data** when needed
9. **Handle async operations** properly with waits
10. **Test both happy path and error scenarios**

## Troubleshooting

### Common Commands

```bash
# Clear Cypress cache
npx cypress cache clear

# Verify Cypress installation
npx cypress verify

# Run tests with debug output
DEBUG=cypress:* npm run cy:run

# Run single test file
npx cypress run --spec "cypress/e2e/reminders-list.cy.js"

# Run tests with specific browser
npx cypress run --browser chrome --headed
```

For additional help, refer to:
- [Cypress Documentation](https://docs.cypress.io/)
- [Cypress Best Practices](https://docs.cypress.io/guides/references/best-practices)
- [Cypress API Reference](https://docs.cypress.io/api/table-of-contents)