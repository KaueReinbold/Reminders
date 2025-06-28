describe('Edit Reminder', () => {
  beforeEach(() => {
    // Mock API responses
    cy.mockRemindersAPI()
    
    // Mock specific reminder for editing
    cy.intercept('GET', '**/api/reminders/1', {
      body: {
        id: 1,
        title: 'Test Reminder 1',
        description: 'This is a test reminder for Cypress testing',
        limitDateFormatted: '2024-12-31',
        isDone: false,
        isDoneFormatted: 'No'
      }
    }).as('getReminder')
    
    // Visit edit page for reminder with ID 1
    cy.visit('/reminder/1')
    
    // Wait for page to load
    cy.wait('@getReminder')
    cy.get('button').contains('Edit', { timeout: 10000 }).should('be.visible')
  })

  it('should display the edit reminder form with existing data', { tags: '@edit' }, () => {
    // Verify page elements
    cy.get('form').should('be.visible')
    
    // Verify form fields are present and populated
    cy.get('input[data-testid="reminderId"]').should('be.visible').and('be.disabled').and('have.value', '1')
    cy.get('input[data-testid="title"]').should('be.visible').and('have.value', 'Test Reminder 1')
    cy.get('input[data-testid="description"]').should('be.visible').and('have.value', 'This is a test reminder for Cypress testing')
    cy.get('input[data-testid="limitDate"]').should('be.visible').and('have.value', '2024-12-31')
    cy.get('input[data-testid="isDone"]').should('be.visible').and('not.be.checked')
    
    // Verify buttons
    cy.get('button').contains('Edit').should('be.visible')
    cy.get('button').contains('Delete').should('be.visible')
    cy.get('button').contains('Back').should('be.visible')
  })

  it('should update reminder successfully', { tags: '@edit' }, () => {
    // Update the form fields
    const newTitle = 'Updated Cypress Reminder'
    const newDescription = 'This reminder was updated by Cypress test'
    const newLimitDate = '2024-11-30'
    
    cy.editReminder(newTitle, newDescription, newLimitDate, true)
    
    // Verify API call was made
    cy.wait('@updateReminder').then((interception) => {
      expect(interception.request.body).to.include({
        title: newTitle,
        description: newDescription,
        limitDate: newLimitDate,
        isDone: true
      })
    })
    
    // Should redirect to home page after successful update
    cy.url().should('eq', Cypress.config().baseUrl + '/')
  })

  it('should handle form validation errors during update', { tags: '@edit' }, () => {
    // Mock API to return validation errors
    cy.intercept('PUT', '**/api/reminders/*', {
      statusCode: 400,
      body: {
        errors: {
          Title: ['Title cannot be empty'],
          Description: ['Description cannot be empty']
        }
      }
    }).as('updateReminderError')
    
    // Clear required fields and try to submit
    cy.get('input[data-testid="title"]').clear()
    cy.get('input[data-testid="description"]').clear()
    cy.get('button').contains('Edit').click()
    
    // Verify error messages are displayed
    cy.contains('Title cannot be empty').should('be.visible')
    cy.contains('Description cannot be empty').should('be.visible')
    
    // Should stay on edit page
    cy.url().should('include', '/reminder/1')
  })

  it('should toggle done status', { tags: '@edit' }, () => {
    // Initially should be unchecked
    cy.get('input[data-testid="isDone"]').should('not.be.checked')
    
    // Check the done checkbox
    cy.get('input[data-testid="isDone"]').check().should('be.checked')
    
    // Uncheck it
    cy.get('input[data-testid="isDone"]').uncheck().should('not.be.checked')
  })

  it('should navigate back to home page', { tags: '@edit' }, () => {
    // Click Back button
    cy.goBack()
    
    // Verify we're back on home page
    cy.url().should('eq', Cypress.config().baseUrl + '/')
    cy.get('button').contains('Create Reminder').should('be.visible')
  })

  it('should handle server errors gracefully during update', { tags: '@edit' }, () => {
    // Mock API to return server error
    cy.intercept('PUT', '**/api/reminders/*', {
      statusCode: 500,
      body: { error: 'Internal server error' }
    }).as('updateReminderServerError')
    
    // Update and submit form
    cy.editReminder('Updated Title', 'Updated Description', '2024-12-25', false)
    
    // Verify API call was made
    cy.wait('@updateReminderServerError')
    
    // Should stay on edit page (not redirect on error)
    cy.url().should('include', '/reminder/1')
  })

  it('should load reminder data when page is accessed directly', { tags: '@edit' }, () => {
    // Visit the edit page directly (already done in beforeEach)
    // Verify that the API call was made to fetch the reminder
    cy.wait('@getReminder')
    
    // Verify the form is populated with the correct data
    cy.get('input[data-testid="title"]').should('have.value', 'Test Reminder 1')
    cy.get('input[data-testid="description"]').should('have.value', 'This is a test reminder for Cypress testing')
    cy.get('input[data-testid="limitDate"]').should('have.value', '2024-12-31')
    cy.get('input[data-testid="isDone"]').should('not.be.checked')
  })
})