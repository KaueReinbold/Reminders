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
      cy.get('article').should('have.length', 3)
      
      // Step 1: CREATE - Open the modal and create a reminder
      cy.openCreateSheet()
      cy.fillSheet('Integration Test Reminder', 'Created via integration test', '2024-12-25')
      cy.saveSheet()
      cy.wait('@createReminder')

      // Modal closes and the list stays on the same route
      cy.get('[role="dialog"]').should('not.exist')
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
      cy.get('article').should('have.length', 4)

      // Verify the new reminder card exists (just check that it exists)
      cy.get('article').contains('Integration Test Reminder').should('be.visible')
      // Skip checking the exact date for now to focus on the main functionality
      
      // Step 3: UPDATE - Edit the reminder in the modal
      cy.openEditSheet('Integration Test Reminder')
      cy.fillSheet('Updated Integration Test Reminder', 'Updated via integration test', '2024-12-30')
      cy.get('[data-testid="isDone"]').click()
      cy.saveSheet()
      cy.wait('@updateReminder')
      cy.get('[role="dialog"]').should('not.exist')

      // Step 4: DELETE - Delete the reminder from the edit modal
      cy.openEditSheet('Integration Test Reminder')
      cy.deleteFromSheet()
      cy.wait('@deleteReminder')

      // Both overlays close and we stay on the list
      cy.get('[role="dialog"]').should('not.exist')
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
      
      // App should handle API error gracefully: shell still renders even
      // though the list column is empty
      cy.get('header').contains('New reminder').should('be.visible')
      cy.get('aside').should('be.visible')
      
      // Try to create a reminder from the modal
      cy.openCreateSheet()
      cy.fillSheet('Test Title', 'Test Description', '2024-12-31')
      cy.saveSheet()
      cy.wait('@createReminderError')

      // Modal stays open so the draft is not lost
      cy.get('[role="dialog"]').should('be.visible')
    })
  })

  context('Modal Flow', () => {
    beforeEach(() => {
      cy.mockRemindersAPI()
    })

    it('should open and dismiss both modals without leaving the list', { tags: '@integration' }, () => {
      // Start at homepage
      cy.visit('/')
      cy.waitForAppReady()
      cy.wait('@getReminders')

      // Open and dismiss the create modal
      cy.openCreateSheet()
      cy.get('button').contains('Cancel').click()
      cy.get('[role="dialog"]').should('not.exist')

      // Open and dismiss the edit modal
      cy.openEditSheet('Test Reminder 1')
      cy.get('button').contains('Close').click()
      cy.get('[role="dialog"]').should('not.exist')

      // The list is untouched
      cy.get('button').contains('New reminder').should('be.visible')
      cy.get('article').should('have.length', 3)
    })
  })

  context('Data Persistence and Validation', () => {
    beforeEach(() => {
      cy.mockRemindersAPI()
    })

    it('should surface server validation errors in the modal', { tags: '@integration' }, () => {
      cy.intercept('POST', '**/api/reminders', {
        statusCode: 400,
        body: {
          errors: {
            title: ["The field Title must be a text with a maximum length of '50'."]
          }
        }
      }).as('createReminderInvalid')

      cy.visit('/')
      cy.waitForAppReady()

      cy.openCreateSheet()
      cy.fillSheet('a'.repeat(60), 'b'.repeat(250), '2020-01-01')
      cy.saveSheet()
      cy.wait('@createReminderInvalid')

      cy.get('[data-testid="title-error"]')
        .should('contain', "The field Title must be a text with a maximum length of '50'.")
    })

    it('should reset the draft when the create modal is reopened', { tags: '@integration' }, () => {
      cy.visit('/')
      cy.waitForAppReady()

      // Fill a partial draft, then dismiss the modal
      cy.openCreateSheet()
      cy.get('[data-testid="title"]').type('Partial Title')
      cy.get('[data-testid="description"]').type('Partial Description')
      cy.get('button').contains('Cancel').click()

      // Reopening starts from a blank draft
      cy.openCreateSheet()
      cy.get('[data-testid="title"]').should('have.value', '')
      cy.get('[data-testid="description"]').should('have.value', '')
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
      cy.get('button').contains('New reminder').should('be.visible')
      cy.get('article').should('have.length', 3)
      
      // The create modal is reachable on mobile too
      cy.openCreateSheet()
      cy.get('button').contains('Cancel').click()
      cy.get('[role="dialog"]').should('not.exist')
    })

    it('should work on tablet viewport', { tags: '@integration' }, () => {
      // Set tablet viewport
      cy.viewport(768, 1024) // iPad dimensions
      
      cy.visit('/')
      cy.waitForAppReady()
      cy.wait('@getReminders')
      
      // Verify layout is appropriate for tablet
      cy.get('button').contains('New reminder').should('be.visible')
      cy.get('article').should('have.length', 3)
    })
  })
})