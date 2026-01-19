#!/bin/bash

# Cypress Project Validation Script
# This script validates the Cypress project setup

echo "🚀 Validating Cypress Testing Project Setup..."

# Check if we're in the correct directory
if [ ! -f "cypress.config.js" ]; then
    echo "❌ Error: Must be run from the Cypress project directory (src/test/cypress)"
    exit 1
fi

echo "✅ In correct directory"

# Check package.json exists and has required scripts
if [ ! -f "package.json" ]; then
    echo "❌ Error: package.json not found"
    exit 1
fi

echo "✅ package.json found"

# Check for required scripts
required_scripts=("cy:open" "cy:run" "test")
for script in "${required_scripts[@]}"; do
    if ! grep -q "\"$script\":" package.json; then
        echo "❌ Error: Script '$script' not found in package.json"
        exit 1
    fi
done

echo "✅ All required scripts found"

# Check Cypress configuration
if [ ! -f "cypress.config.js" ]; then
    echo "❌ Error: cypress.config.js not found"
    exit 1
fi

echo "✅ Cypress configuration found"

# Check test files
test_files=(
    "cypress/e2e/reminders-list.cy.js"
    "cypress/e2e/reminder-create.cy.js"
    "cypress/e2e/reminder-edit.cy.js"
    "cypress/e2e/reminder-delete.cy.js"
    "cypress/e2e/reminders-integration.cy.js"
)

for file in "${test_files[@]}"; do
    if [ ! -f "$file" ]; then
        echo "❌ Error: Test file '$file' not found"
        exit 1
    fi
done

echo "✅ All test files found"

# Check support files
support_files=(
    "cypress/support/commands.js"
    "cypress/support/e2e.js"
)

for file in "${support_files[@]}"; do
    if [ ! -f "$file" ]; then
        echo "❌ Error: Support file '$file' not found"
        exit 1
    fi
done

echo "✅ All support files found"

# Check fixtures
fixture_files=(
    "cypress/fixtures/reminders.json"
    "cypress/fixtures/single-reminder.json"
    "cypress/fixtures/empty-reminders.json"
)

for file in "${fixture_files[@]}"; do
    if [ ! -f "$file" ]; then
        echo "❌ Error: Fixture file '$file' not found"
        exit 1
    fi
done

echo "✅ All fixture files found"

# Check if README exists
if [ ! -f "README.md" ]; then
    echo "❌ Error: README.md not found"
    exit 1
fi

echo "✅ README.md found"

# Count test cases
test_count=$(grep -r "it('.*'" cypress/e2e/ | wc -l)
echo "✅ Found $test_count test cases"

describe_count=$(grep -r "describe('.*'" cypress/e2e/ | wc -l)
echo "✅ Found $describe_count test suites"

# Validate test structure
echo "📊 Test Coverage Summary:"
echo "  - List tests: $(grep -c "it('" cypress/e2e/reminders-list.cy.js) test cases"
echo "  - Create tests: $(grep -c "it('" cypress/e2e/reminder-create.cy.js) test cases"
echo "  - Edit tests: $(grep -c "it('" cypress/e2e/reminder-edit.cy.js) test cases"
echo "  - Delete tests: $(grep -c "it('" cypress/e2e/reminder-delete.cy.js) test cases"
echo "  - Integration tests: $(grep -c "it('" cypress/e2e/reminders-integration.cy.js) test cases"

echo ""
echo "🎉 Cypress Testing Project Setup Validation Complete!"
echo "📝 Total Lines of Test Code: $(find cypress/e2e -name "*.cy.js" | xargs wc -l | tail -1 | awk '{print $1}')"
echo ""
echo "Next steps:"
echo "1. Install dependencies: npm install"
echo "2. Start React app: cd ../../../app/reactjs/reminders-app && npm start"
echo "3. Run tests: npm run cy:run (headless) or npm run cy:open (interactive)"
echo ""
echo "For detailed instructions, see README.md"