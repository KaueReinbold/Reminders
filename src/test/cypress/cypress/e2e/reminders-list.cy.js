describe('Reminders List', () => {
  beforeEach(() => {
    // Mock API responses
    cy.mockRemindersAPI()
    
    // Visit the homepage
    cy.visit('/')
    
    // Wait for app to be ready
    cy.waitForAppReady()
  })

  it('should display the reminders list', { tags: '@list' }, () => {
    // Verify page title and main elements
    cy.title().should('contain', 'Reminders App')
    cy.get('main').should('be.visible')
    
    // Verify Create Reminder button is present
    cy.get('button').contains('Create Reminder').should('be.visible')
    
    // Verify table headers are present
    cy.get('th').should('contain', 'ID')
    cy.get('th').should('contain', 'Title')
    cy.get('th').should('contain', 'Description')
    cy.get('th').should('contain', 'Limit Date')
    cy.get('th').should('contain', 'Done')
    
    // Wait for API call and verify data is loaded
    cy.wait('@getReminders')
    
    // Verify test reminders are displayed
    cy.verifyReminderInList('1', 'Test Reminder 1', 'This is a test reminder for Cypress testing', '2024-12-31', false)
    cy.verifyReminderInList('2', 'Test Reminder 2', 'This is another test reminder for Cypress testing', '2024-11-30', true)
    cy.verifyReminderInList('3', 'Test Reminder 3', 'Third test reminder with different data', '2024-10-15', false)
    
    // Verify Edit buttons are present for each reminder
    cy.get('tbody tr').should('have.length', 3)
    cy.get('button').contains('Edit').should('have.length', 3)
  })

  it('should handle loading state', { tags: '@list' }, () => {
    // Intercept with a delay to test loading state
    cy.intercept('GET', '**/api/reminders', { 
      delay: 1000,
      fixture: 'reminders.json' 
    }).as('getRemindersDelayed')
    
    cy.visit('/')
    
    // Should show loading indicator
    cy.get('[role="progressbar"]').should('be.visible')
    
    // Wait for data to load
    cy.wait('@getRemindersDelayed')
    
    // Loading should be gone and content should be visible
    cy.get('[role="progressbar"]').should('not.exist')
    cy.get('button').contains('Create Reminder').should('be.visible')
  })

  it('should navigate to create reminder page', { tags: '@list' }, () => {
    // Click Create Reminder button
    cy.goToCreateReminder()
    
    // Verify we're on the create page
    cy.url().should('include', '/reminder/create')
    cy.get('button').contains('Create').should('be.visible')
    cy.get('button').contains('Back').should('be.visible')
  })

  it('should navigate to edit reminder page', { tags: '@list' }, () => {
    // Wait for list to load
    cy.wait('@getReminders')
    
    // Click edit button for first reminder
    cy.goToEditReminder('1')
    
    // Verify we're on the edit page
    cy.url().should('include', '/reminder/1')
    cy.get('button').contains('Edit').should('be.visible')
    cy.get('button').contains('Delete').should('be.visible')
    cy.get('button').contains('Back').should('be.visible')
  })
})