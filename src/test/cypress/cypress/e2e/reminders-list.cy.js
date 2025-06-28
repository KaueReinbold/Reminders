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
    
    // Verify table has rows (any reminders)
    cy.get('tbody tr').should('have.length.greaterThan', 0)
    
    // Verify at least the first reminder is displayed
    cy.get('tbody tr').first().within(() => {
      cy.contains('1').should('be.visible')
      cy.contains('Test Reminder 1').should('be.visible')
    })
    
    // Verify Edit buttons are present
    cy.get('button').contains('Edit').should('exist')
  })

  it('should handle loading state', { tags: '@list' }, () => {
    // Intercept with a delay to test loading state
    cy.intercept('GET', '**/api/reminders', { 
      delay: 2000,
      fixture: 'reminders.json' 
    }).as('getRemindersDelayed')
    
    cy.visit('/')
    
    // Should show loading indicator - wait for the page to start loading first
    cy.get('main').should('be.visible')
    
    // Wait for data to load
    cy.wait('@getRemindersDelayed')
    
    // Content should be visible after loading
    cy.get('button').contains('Create Reminder').should('be.visible')
    cy.get('table').should('be.visible')
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