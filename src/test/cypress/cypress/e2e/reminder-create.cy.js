describe('Create Reminder', () => {
  beforeEach(() => {
    // Mock API responses
    cy.mockRemindersAPI()
    
    // Visit create page
    cy.visit('/reminder/create')
    
    // Wait for page to load
    cy.get('button').contains('Create', { timeout: 10000 }).should('be.visible')
  })

  it('should display the create reminder form', { tags: '@create' }, () => {
    // Verify page elements
    cy.get('form').should('be.visible')
    
    // Verify form fields are present
    cy.get('input[data-testid="title"]').should('be.visible')
    cy.get('input[data-testid="description"]').should('be.visible')
    cy.get('input[data-testid="limitDate"]').should('be.visible')
    
    // Verify ID field is not present (only in edit mode)
    cy.get('input[data-testid="reminderId"]').should('not.exist')
    
    // Verify Done checkbox is not present (only in edit mode)
    cy.get('input[data-testid="isDone"]').should('not.exist')
    
    // Verify buttons
    cy.get('button').contains('Create').should('be.visible')
    cy.get('button').contains('Back').should('be.visible')
  })

  it('should create a new reminder successfully', { tags: '@create' }, () => {
    // Fill out the form
    const title = 'New Cypress Reminder'
    const description = 'This reminder was created by Cypress test'
    const limitDate = '2024-12-25'
    
    cy.createReminder(title, description, limitDate)
    
    // Verify API call was made
    cy.wait('@createReminder').then((interception) => {
      expect(interception.request.body).to.include({
        title,
        description,
        limitDate
      })
    })
    
    // Should redirect to home page after successful creation
    cy.url().should('eq', Cypress.config().baseUrl + '/')
  })

  it('should handle form validation errors', { tags: '@create' }, () => {
    // Mock API to return validation errors
    cy.intercept('POST', '**/api/reminders', {
      statusCode: 400,
      body: {
        errors: {
          Title: ["The field Title must be a text with a maximum length of '50'."],
          Description: ["The field Description must be a text with a maximum length of '200'."],
          'LimitDate.Date': ['The Limit Date should be later than Today.']
        }
      }
    }).as('createReminderError')
    
    // Fill form with some data to trigger validation
    cy.get('input[data-testid="title"]').type('x'.repeat(60)) // Too long
    cy.get('input[data-testid="description"]').type('x'.repeat(250)) // Too long
    cy.get('input[data-testid="limitDate"]').type('2020-01-01') // Past date
    
    // Try to submit form
    cy.get('button').contains('Create').click()
    
    // Wait for API call
    cy.wait('@createReminderError')
    
    // Verify error messages are displayed
    cy.contains("The field Title must be a text with a maximum length of '50'.").should('be.visible')
    cy.contains("The field Description must be a text with a maximum length of '200'.").should('be.visible')
    cy.contains('The Limit Date should be later than Today.').should('be.visible')
    
    // Should stay on create page
    cy.url().should('include', '/reminder/create')
  })

  it('should navigate back to home page', { tags: '@create' }, () => {
    // Click Back button
    cy.goBack()
    
    // Verify we're back on home page
    cy.url().should('eq', Cypress.config().baseUrl + '/')
    cy.get('button').contains('Create Reminder').should('be.visible')
  })

  it('should handle server errors gracefully', { tags: '@create' }, () => {
    // Mock API to return server error
    cy.intercept('POST', '**/api/reminders', {
      statusCode: 500,
      body: { error: 'Internal server error' }
    }).as('createReminderServerError')
    
    // Fill and submit form
    cy.createReminder('Test Title', 'Test Description', '2024-12-31')
    
    // Verify API call was made
    cy.wait('@createReminderServerError')
    
    // Should stay on create page (not redirect on error)
    cy.url().should('include', '/reminder/create')
    
    // Could verify error message is displayed if app shows server errors
    // This would depend on how the app handles server errors
  })

  it('should clear form fields when typing', { tags: '@create' }, () => {
    // Type in fields
    cy.get('input[data-testid="title"]').type('Test Title')
    cy.get('input[data-testid="description"]').type('Test Description')
    cy.get('input[data-testid="limitDate"]').type('2024-12-31')
    
    // Verify values are entered
    cy.get('input[data-testid="title"]').should('have.value', 'Test Title')
    cy.get('input[data-testid="description"]').should('have.value', 'Test Description')
    cy.get('input[data-testid="limitDate"]').should('have.value', '2024-12-31')
    
    // Clear fields
    cy.get('input[data-testid="title"]').clear()
    cy.get('input[data-testid="description"]').clear()
    cy.get('input[data-testid="limitDate"]').clear()
    
    // Verify fields are cleared
    cy.get('input[data-testid="title"]').should('have.value', '')
    cy.get('input[data-testid="description"]').should('have.value', '')
    cy.get('input[data-testid="limitDate"]').should('have.value', '')
  })
})