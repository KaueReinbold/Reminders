// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************

// Custom commands for Reminders app
Cypress.Commands.add('createReminder', (title, description, limitDate) => {
  cy.get('input[data-testid="title"]', { timeout: 10000 }).should('be.visible').clear().type(title)
  cy.get('input[data-testid="description"]').should('be.visible').clear().type(description)
  cy.get('input[data-testid="limitDate"]').should('be.visible').clear().type(limitDate)
  cy.get('button[type="submit"]').contains('Create').should('be.visible').click()
})

Cypress.Commands.add('editReminder', (title, description, limitDate, isDone = false) => {
  cy.get('input[data-testid="title"]', { timeout: 10000 }).should('be.visible').clear().type(title)
  cy.get('input[data-testid="description"]').should('be.visible').clear().type(description)
  cy.get('input[data-testid="limitDate"]').should('be.visible').clear().type(limitDate)
  
  // Handle checkbox for done status
  if (isDone) {
    cy.get('input[data-testid="isDone"]').check()
  } else {
    cy.get('input[data-testid="isDone"]').uncheck()
  }
  
  cy.get('button[type="submit"]').contains('Edit').should('be.visible').click()
})

Cypress.Commands.add('deleteReminder', () => {
  cy.get('button').contains('Delete').should('be.visible').click()
  cy.get('button[data-testid="delete-button"]').should('be.visible').click()
})

// Navigation helpers
Cypress.Commands.add('goToCreateReminder', () => {
  cy.get('button').contains('Create Reminder').should('be.visible').click()
  cy.url().should('include', '/reminder/create')
})

Cypress.Commands.add('goToEditReminder', (reminderId) => {
  cy.get('tbody tr').contains(reminderId).parent().within(() => {
    cy.get('button').contains('Edit').click()
  })
  cy.url().should('include', `/reminder/${reminderId}`)
})

Cypress.Commands.add('goBack', () => {
  cy.get('button').contains('Back').should('be.visible').click()
  cy.url().should('eq', Cypress.config().baseUrl + '/')
})

// API intercept helpers
Cypress.Commands.add('mockRemindersAPI', () => {
  cy.intercept('GET', '**/api/reminders', { fixture: 'reminders.json' }).as('getReminders')
  cy.intercept('POST', '**/api/reminders', { statusCode: 201, body: { id: 999, title: 'New Reminder' } }).as('createReminder')
  cy.intercept('PUT', '**/api/reminders/*', { statusCode: 200, body: { id: 1, title: 'Updated Reminder' } }).as('updateReminder')
  cy.intercept('DELETE', '**/api/reminders/*', { statusCode: 204 }).as('deleteReminder')
})

// Command to wait for app to be ready
Cypress.Commands.add('waitForAppReady', () => {
  cy.get('main', { timeout: 15000 }).should('be.visible')
  cy.get('button').contains('Create Reminder', { timeout: 10000 }).should('be.visible')
})

// Verify reminder in list
Cypress.Commands.add('verifyReminderInList', (id, title, description, limitDate, isDone) => {
  cy.get('tbody tr').contains(id).parent().within(() => {
    cy.contains(title).should('be.visible')
    cy.contains(description).should('be.visible')
    cy.contains(limitDate).should('be.visible')
    cy.contains(isDone ? 'Yes' : 'No').should('be.visible')
  })
})