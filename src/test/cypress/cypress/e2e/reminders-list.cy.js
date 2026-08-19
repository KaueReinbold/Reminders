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

    // Wait for API call and verify data is loaded
    cy.wait('@getReminders')

    // Fixture dates are in the past: open reminders group under Overdue,
    // the completed one under Done. Section headers replace the old table.
    cy.get('h2').contains('Overdue').should('be.visible')
    cy.get('h2').contains('Done').should('be.visible')

    // Verify all reminders render as cards
    cy.get('article').should('have.length', 3)

    // Overdue group is sorted ascending by limit date
    cy.get('article').first().within(() => {
      cy.contains('Test Reminder 3').should('be.visible')
    })
    cy.contains('Test Reminder 1').should('be.visible')

    // Verify card controls are present
    cy.get('button[aria-label="Edit reminder"]').should('have.length', 3)
    cy.get('button[aria-label="Mark done"]').should('have.length', 2)
    cy.get('button[aria-label="Mark not done"]').should('have.length', 1)
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
    cy.get('article').should('have.length', 3)
  })

  it('should toggle a reminder from the list', { tags: '@list' }, () => {
    cy.wait('@getReminders')

    // Any background refetch (e.g. on window focus) must serve the toggled
    // state, otherwise it would overwrite the optimistic update with the
    // original fixture.
    cy.fixture('reminders.json').then(reminders => {
      const toggled = reminders.map(reminder =>
        reminder.id === '3' ? { ...reminder, isDone: true } : reminder
      )
      cy.intercept('GET', '**/api/reminders', { body: toggled }).as('getRemindersToggled')
    })

    // Toggle the first open reminder (id 3, earliest overdue);
    // optimistic update moves it to Done
    cy.get('button[aria-label="Mark done"]').first().click()
    cy.wait('@updateReminder')

    cy.get('button[aria-label="Mark not done"]').should('have.length', 2)
    cy.get('button[aria-label="Mark done"]').should('have.length', 1)
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

    // Click edit button on the first reminder's card
    cy.goToEditReminder('1', 'Test Reminder 1')

    // Verify we're on the edit page
    cy.url().should('include', '/reminder/edit/?id=1')
    cy.get('button').contains('Edit').should('be.visible')
    cy.get('button').contains('Delete').should('be.visible')
    cy.get('button').contains('Back').should('be.visible')
  })
})
