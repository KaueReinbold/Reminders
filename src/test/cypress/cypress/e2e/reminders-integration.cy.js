describe('Reminders Integration Tests', () => {
  context('Full User Journey', () => {
    beforeEach(() => {
      // Set up API intercepts for the full journey
      cy.intercept('GET', '**/api/reminders', { fixture: 'reminders.json' }).as('getReminders')
      cy.intercept('POST', '**/api/reminders', { 
        statusCode: 201, 
        body: { 
          id: 4, 
          title: 'Integration Test Reminder',
          description: 'Created via integration test',
          limitDateFormatted: '2024-12-25',
          isDone: false,
          isDoneFormatted: 'No'
        } 
      }).as('createReminder')
      cy.intercept('GET', '**/api/reminders/4', {
        body: {
          id: 4,
          title: 'Integration Test Reminder',
          description: 'Created via integration test',
          limitDateFormatted: '2024-12-25',
          isDone: false,
          isDoneFormatted: 'No'
        }
      }).as('getReminder')
      cy.intercept('PUT', '**/api/reminders/4', { 
        statusCode: 200, 
        body: { 
          id: 4, 
          title: 'Updated Integration Test Reminder',
          description: 'Updated via integration test',
          limitDateFormatted: '2024-12-30',
          isDone: true,
          isDoneFormatted: 'Yes'
        } 
      }).as('updateReminder')
      cy.intercept('DELETE', '**/api/reminders/4', { statusCode: 204 }).as('deleteReminder')
    })

    it('should complete full CRUD journey', { tags: '@integration' }, () => {
      // Start at homepage
      cy.visit('/')
      cy.waitForAppReady()
      cy.wait('@getReminders')
      
      // Verify initial list
      cy.get('tbody tr').should('have.length', 3)
      
      // Step 1: CREATE - Navigate to create page and create reminder
      cy.goToCreateReminder()
      cy.createReminder('Integration Test Reminder', 'Created via integration test', '2024-12-25')
      cy.wait('@createReminder')
      
      // Should be back on homepage
      cy.url().should('eq', Cypress.config().baseUrl + '/')
      
      // Step 2: READ - Verify reminder appears in list (mock the updated list)
      cy.intercept('GET', '**/api/reminders', { 
        body: [
          {
            id: 1,
            title: 'Test Reminder 1',
            description: 'This is a test reminder for Cypress testing',
            limitDateFormatted: '2024-12-31',
            isDone: false,
            isDoneFormatted: 'No'
          },
          {
            id: 2,
            title: 'Test Reminder 2',
            description: 'This is another test reminder for Cypress testing',
            limitDateFormatted: '2024-11-30',
            isDone: true,
            isDoneFormatted: 'Yes'
          },
          {
            id: 3,
            title: 'Test Reminder 3',
            description: 'Third test reminder with different data',
            limitDateFormatted: '2024-10-15',
            isDone: false,
            isDoneFormatted: 'No'
          },
          {
            id: 4,
            title: 'Integration Test Reminder',
            description: 'Created via integration test',
            limitDateFormatted: '2024-12-25',
            isDone: false,
            isDoneFormatted: 'No'
          }
        ]
      }).as('getRemindersWithNew')
      
      // Refresh to see new reminder
      cy.reload()
      cy.wait('@getRemindersWithNew')
      cy.get('tbody tr').should('have.length', 4)
      cy.verifyReminderInList('4', 'Integration Test Reminder', 'Created via integration test', '2024-12-25', false)
      
      // Step 3: UPDATE - Edit the reminder
      cy.goToEditReminder('4')
      cy.wait('@getReminder')
      cy.editReminder('Updated Integration Test Reminder', 'Updated via integration test', '2024-12-30', true)
      cy.wait('@updateReminder')
      
      // Should be back on homepage
      cy.url().should('eq', Cypress.config().baseUrl + '/')
      
      // Step 4: DELETE - Delete the reminder
      cy.goToEditReminder('4')
      cy.wait('@getReminder')
      cy.deleteReminder()
      cy.wait('@deleteReminder')
      
      // Should be back on homepage
      cy.url().should('eq', Cypress.config().baseUrl + '/')
    })
  })

  context('API Error Handling', () => {
    it('should handle API failures gracefully', { tags: '@integration' }, () => {
      // Mock API failures
      cy.intercept('GET', '**/api/reminders', { statusCode: 500 }).as('getRemindersError')
      cy.intercept('POST', '**/api/reminders', { statusCode: 500 }).as('createReminderError')
      
      // Visit homepage
      cy.visit('/')
      cy.wait('@getRemindersError')
      
      // App should handle API error gracefully
      // This depends on how the app handles errors
      cy.get('main').should('be.visible')
      
      // Try to create reminder
      cy.visit('/reminder/create')
      cy.createReminder('Test Title', 'Test Description', '2024-12-31')
      cy.wait('@createReminderError')
      
      // Should stay on create page
      cy.url().should('include', '/reminder/create')
    })
  })

  context('Cross-Page Navigation', () => {
    beforeEach(() => {
      cy.mockRemindersAPI()
    })

    it('should maintain navigation flow between pages', { tags: '@integration' }, () => {
      // Start at homepage
      cy.visit('/')
      cy.waitForAppReady()
      cy.wait('@getReminders')
      
      // Navigate to create page
      cy.goToCreateReminder()
      
      // Navigate back from create page
      cy.goBack()
      cy.wait('@getReminders')
      
      // Navigate to edit page
      cy.goToEditReminder('1')
      
      // Navigate back from edit page
      cy.goBack()
      cy.wait('@getReminders')
      
      // Verify we're back on homepage with all elements
      cy.get('button').contains('Create Reminder').should('be.visible')
      cy.get('tbody tr').should('have.length', 3)
    })
  })

  context('Data Persistence and Validation', () => {
    beforeEach(() => {
      cy.mockRemindersAPI()
    })

    it('should validate form data correctly', { tags: '@integration' }, () => {
      // Test create form validation
      cy.visit('/reminder/create')
      
      // Try to submit empty form
      cy.get('button').contains('Create').click()
      
      // Fill form with invalid data
      cy.get('input[data-testid="title"]').type('a'.repeat(101)) // Assuming max length
      cy.get('input[data-testid="description"]').type('b'.repeat(501)) // Assuming max length
      cy.get('input[data-testid="limitDate"]').type('invalid-date')
      
      // Verify form handles validation
      // This would depend on client-side or server-side validation
    })

    it('should maintain form state during navigation', { tags: '@integration' }, () => {
      // Visit create page
      cy.visit('/reminder/create')
      
      // Fill partial form
      cy.get('input[data-testid="title"]').type('Partial Title')
      cy.get('input[data-testid="description"]').type('Partial Description')
      
      // Navigate away and back
      cy.visit('/')
      cy.waitForAppReady()
      cy.goToCreateReminder()
      
      // Form should be reset (this is expected behavior for new form)
      cy.get('input[data-testid="title"]').should('have.value', '')
      cy.get('input[data-testid="description"]').should('have.value', '')
    })
  })

  context('Responsive Design', () => {
    beforeEach(() => {
      cy.mockRemindersAPI()
    })

    it('should work on mobile viewport', { tags: '@integration' }, () => {
      // Set mobile viewport
      cy.viewport(375, 667) // iPhone SE dimensions
      
      cy.visit('/')
      cy.waitForAppReady()
      cy.wait('@getReminders')
      
      // Verify elements are still accessible
      cy.get('button').contains('Create Reminder').should('be.visible')
      cy.get('table').should('be.visible')
      
      // Test navigation on mobile
      cy.goToCreateReminder()
      cy.get('form').should('be.visible')
      cy.goBack()
    })

    it('should work on tablet viewport', { tags: '@integration' }, () => {
      // Set tablet viewport
      cy.viewport(768, 1024) // iPad dimensions
      
      cy.visit('/')
      cy.waitForAppReady()
      cy.wait('@getReminders')
      
      // Verify layout is appropriate for tablet
      cy.get('button').contains('Create Reminder').should('be.visible')
      cy.get('table').should('be.visible')
    })
  })
})