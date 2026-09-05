describe('Reminder Modal', () => {
  beforeEach(() => {
    cy.mockRemindersAPI()
    cy.visit('/')
    cy.waitForAppReady()
    cy.wait('@getReminders')
  })

  context('Create', () => {
    it('should open a blank draft due tomorrow', { tags: '@modal' }, () => {
      cy.openCreateSheet()

      const tomorrow = new Date()
      tomorrow.setDate(tomorrow.getDate() + 1)
      const expected = tomorrow.toISOString().slice(0, 10)

      cy.get('[role="dialog"]').within(() => {
        cy.contains('New reminder').should('be.visible')
        cy.get('[data-testid="title"]').should('have.value', '')
        cy.get('[data-testid="description"]').should('have.value', '')
        cy.get('[data-testid="limitDate"]').should('have.value', expected)
        cy.contains('Not done yet').should('be.visible')
        cy.contains('Delete reminder').should('not.exist')
      })
    })

    it('should keep save disabled while the title is blank', { tags: '@modal' }, () => {
      cy.openCreateSheet()

      cy.get('[data-testid="save-button"]').should('be.disabled')

      cy.get('[data-testid="title"]').type('   ')
      cy.get('[data-testid="save-button"]').should('be.disabled')

      cy.get('[data-testid="title"]').clear().type('Call the notary')
      cy.get('[data-testid="save-button"]').should('be.enabled')
    })

    it('should create a reminder', { tags: '@modal' }, () => {
      cy.openCreateSheet()
      cy.fillSheet('Modal Reminder', 'Created from the modal', '2024-12-25')
      cy.saveSheet()

      cy.wait('@createReminder').then(interception => {
        expect(interception.request.body).to.include({
          title: 'Modal Reminder',
          description: 'Created from the modal',
          limitDate: '2024-12-25',
          isDone: false
        })
      })

      cy.get('[role="dialog"]').should('not.exist')
    })

    it('should keep the modal open and show the API error', { tags: '@modal' }, () => {
      cy.intercept('POST', '**/api/reminders', {
        statusCode: 400,
        body: { message: 'Invalid body' }
      }).as('createReminderError')

      cy.openCreateSheet()
      cy.fillSheet('Modal Reminder', 'Created from the modal', '2024-12-25')
      cy.saveSheet()
      cy.wait('@createReminderError')

      cy.contains('Invalid body').should('be.visible')
      cy.get('[role="dialog"]').should('be.visible')
      cy.get('[data-testid="title"]').should('have.value', 'Modal Reminder')
    })
  })

  context('Edit', () => {
    it('should prefill the reminder and save changes', { tags: '@modal' }, () => {
      cy.openEditSheet('Test Reminder 1')

      cy.get('[data-testid="title"]').should('have.value', 'Test Reminder 1')
      cy.get('[data-testid="limitDate"]').should('have.value', '2024-12-31')

      cy.fillSheet('Updated Reminder', 'Updated from the modal', '2024-11-30')
      cy.get('[data-testid="isDone"]')
        .should('have.attr', 'aria-pressed', 'false')
        .click()
      cy.get('[data-testid="isDone"]')
        .should('have.attr', 'aria-pressed', 'true')
        .and('contain', 'Done')
      cy.saveSheet()

      cy.wait('@updateReminder').then(interception => {
        expect(interception.request.body).to.include({
          id: '1',
          title: 'Updated Reminder',
          description: 'Updated from the modal',
          limitDate: '2024-11-30',
          isDone: true
        })
      })

      cy.get('[role="dialog"]').should('not.exist')
    })
  })

  context('Close behavior', () => {
    it('should close on Escape, scrim click, Close and Cancel', { tags: '@modal' }, () => {
      cy.openCreateSheet()
      cy.get('body').type('{esc}')
      cy.get('[role="dialog"]').should('not.exist')

      cy.openCreateSheet()
      cy.get('[data-testid="reminder-sheet-scrim"]').click('topLeft')
      cy.get('[role="dialog"]').should('not.exist')

      cy.openCreateSheet()
      cy.get('button').contains('Close').click()
      cy.get('[role="dialog"]').should('not.exist')

      cy.openCreateSheet()
      cy.get('button').contains('Cancel').click()
      cy.get('[role="dialog"]').should('not.exist')
    })
  })

  context('Delete confirmation', () => {
    it('should ask for confirmation and delete the reminder', { tags: '@modal' }, () => {
      cy.openEditSheet('Test Reminder 1')
      cy.get('button').contains('Delete reminder').click()

      cy.contains('Delete this reminder?').should('be.visible')
      cy.contains('will be removed permanently.').should('be.visible')
      cy.get('[data-testid="delete-button"]').click()

      cy.wait('@deleteReminder').then(interception => {
        expect(interception.request.url).to.include('/api/reminders/1')
      })

      cy.get('[role="dialog"]').should('not.exist')
    })

    it('should keep the reminder when the confirmation is dismissed', { tags: '@modal' }, () => {
      cy.openEditSheet('Test Reminder 1')
      cy.get('button').contains('Delete reminder').click()
      cy.get('[data-testid="close-button"]').click()

      cy.contains('Delete this reminder?').should('not.exist')
      cy.contains('Edit reminder').should('be.visible')
      cy.get('article').should('have.length', 3)
    })

    it('should close only the confirmation on Escape', { tags: '@modal' }, () => {
      cy.openEditSheet('Test Reminder 1')
      cy.get('button').contains('Delete reminder').click()
      cy.contains('Delete this reminder?').should('be.visible')

      cy.get('body').type('{esc}')

      cy.contains('Delete this reminder?').should('not.exist')
      cy.contains('Edit reminder').should('be.visible')
    })
  })
})
