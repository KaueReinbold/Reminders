describe('Delete Reminder', () => {
  beforeEach(() => {
    // Mock API responses
    cy.mockRemindersAPI()
    
    // Mock specific reminder for editing/deleting
    cy.intercept('GET', '**/api/reminders/1', {
      body: {
        id: 1,
        title: 'Test Reminder to Delete',
        description: 'This reminder will be deleted by Cypress test',
        limitDateFormatted: '2024-12-31',
        isDone: false,
        isDoneFormatted: 'No'
      }
    }).as('getReminder')
    
    // Visit edit page for reminder with ID 1
    cy.visit('/reminder/1')
    
    // Wait for page to load
    cy.wait('@getReminder')
    cy.get('button').contains('Delete', { timeout: 10000 }).should('be.visible')
  })

  it('should display delete confirmation modal', { tags: '@delete' }, () => {
    // Click Delete button
    cy.get('button').contains('Delete').click()
    
    // Verify modal is displayed
    cy.get('[role="presentation"]').should('be.visible')
    cy.contains('Delete reminder').should('be.visible')
    cy.contains('Are you sure you want to delete this reminder?').should('be.visible')
    
    // Verify modal buttons
    cy.get('button[data-testid="delete-button"]').should('be.visible')
    cy.get('button[data-testid="close-button"]').should('be.visible')
  })

  it('should cancel delete operation', { tags: '@delete' }, () => {
    // Click Delete button to open modal
    cy.get('button').contains('Delete').click()
    
    // Verify modal is open
    cy.contains('Are you sure you want to delete this reminder?').should('be.visible')
    
    // Click Close button to cancel
    cy.get('button[data-testid="close-button"]').click()
    
    // Verify modal is closed
    cy.get('[role="presentation"]').should('not.exist')
    
    // Should still be on edit page
    cy.url().should('include', '/reminder/1')
    cy.get('button').contains('Edit').should('be.visible')
  })

  it('should delete reminder successfully', { tags: '@delete' }, () => {
    // Use custom command to delete reminder
    cy.deleteReminder()
    
    // Verify API call was made
    cy.wait('@deleteReminder').then((interception) => {
      expect(interception.request.url).to.include('/api/reminders/1')
    })
    
    // Should redirect to home page after successful deletion
    cy.url().should('eq', Cypress.config().baseUrl + '/')
    cy.get('button').contains('Create Reminder').should('be.visible')
  })

  it('should handle delete confirmation flow step by step', { tags: '@delete' }, () => {
    // Step 1: Click Delete button
    cy.get('button').contains('Delete').should('be.visible').click()
    
    // Step 2: Verify modal appears
    cy.get('[role="presentation"]').should('be.visible')
    cy.contains('Delete reminder').should('be.visible')
    cy.contains('Are you sure you want to delete this reminder?').should('be.visible')
    
    // Step 3: Confirm deletion
    cy.get('button[data-testid="delete-button"]').should('be.visible').click()
    
    // Step 4: Verify API call
    cy.wait('@deleteReminder')
    
    // Step 5: Verify redirect
    cy.url().should('eq', Cypress.config().baseUrl + '/')
  })

  it('should handle server errors during deletion', { tags: '@delete' }, () => {
    // Mock API to return server error
    cy.intercept('DELETE', '**/api/reminders/*', {
      statusCode: 500,
      body: { error: 'Failed to delete reminder' }
    }).as('deleteReminderError')
    
    // Attempt to delete reminder
    cy.deleteReminder()
    
    // Verify API call was made
    cy.wait('@deleteReminderError')
    
    // Should stay on edit page (not redirect on error)
    cy.url().should('include', '/reminder/1')
    
    // Modal should be closed after the error
    cy.get('[role="presentation"]').should('not.exist')
  })

  it('should handle 404 error for non-existent reminder', { tags: '@delete' }, () => {
    // Mock API to return 404 for reminder
    cy.intercept('GET', '**/api/reminders/999', {
      statusCode: 404,
      body: { error: 'Reminder not found' }
    }).as('getReminderNotFound')
    
    // Visit edit page for non-existent reminder
    cy.visit('/reminder/999')
    
    // Should handle 404 gracefully
    // This test depends on how the app handles 404 errors
    // It might redirect or show an error message
  })

  it('should allow multiple modal open/close cycles', { tags: '@delete' }, () => {
    // Open modal
    cy.get('button').contains('Delete').click()
    cy.contains('Are you sure you want to delete this reminder?').should('be.visible')
    
    // Close modal
    cy.get('button[data-testid="close-button"]').click()
    cy.get('[role="presentation"]').should('not.exist')
    
    // Open modal again
    cy.get('button').contains('Delete').click()
    cy.contains('Are you sure you want to delete this reminder?').should('be.visible')
    
    // Close modal again
    cy.get('button[data-testid="close-button"]').click()
    cy.get('[role="presentation"]').should('not.exist')
    
    // Should still be on edit page
    cy.url().should('include', '/reminder/1')
  })

  it('should verify modal accessibility', { tags: '@delete' }, () => {
    // Click Delete button
    cy.get('button').contains('Delete').click()
    
    // Verify modal has proper ARIA attributes
    cy.get('[role="presentation"]').should('have.attr', 'aria-labelledby', 'child-modal-title')
    cy.get('[role="presentation"]').should('have.attr', 'aria-describedby', 'child-modal-description')
    
    // Verify modal can be closed with Escape key
    cy.get('body').type('{esc}')
    cy.get('[role="presentation"]').should('not.exist')
  })
})