// Captures full-page screenshots per viewport for manual design review.
// Output: cypress/screenshots/visual-check.cy.js/
const shift = days => {
  const d = new Date()
  d.setDate(d.getDate() + days)
  const p = n => String(n).padStart(2, '0')
  return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}`
}

const fixtures = [
  { id: '1', title: 'Pay water bill', description: 'Was due last week', limitDate: shift(-4), limitDateFormatted: shift(-4), isDone: false },
  { id: '2', title: 'Call the notary', description: '', limitDate: shift(-1), limitDateFormatted: shift(-1), isDone: false },
  { id: '3', title: 'Team standup notes', description: 'Send summary to the group', limitDate: shift(0), limitDateFormatted: shift(0), isDone: false },
  { id: '4', title: 'Dentist appointment', description: 'Confirm time by phone', limitDate: shift(1), limitDateFormatted: shift(1), isDone: false },
  { id: '5', title: 'Renew passport with a much longer title to see wrapping behavior in the card', description: 'Gather documents first, check the consulate opening hours and book the earliest slot available', limitDate: shift(15), limitDateFormatted: shift(15), isDone: false },
  { id: '6', title: 'Book flights', description: 'Done yesterday', limitDate: shift(-1), limitDateFormatted: shift(-1), isDone: true },
]

describe('visual check', () => {
  beforeEach(() => {
    cy.intercept('GET', '**/api/reminders', { body: fixtures }).as('getReminders')
  })

  it('desktop 1280', { tags: '@visual' }, () => {
    cy.viewport(1280, 900)
    cy.visit('/')
    cy.wait('@getReminders')
    cy.get('article').should('have.length', 6)
    cy.screenshot('visual-desktop', { capture: 'fullPage', overwrite: true })
  })

  it('tablet 768', { tags: '@visual' }, () => {
    cy.viewport(768, 1024)
    cy.visit('/')
    cy.wait('@getReminders')
    cy.get('article').should('have.length', 6)
    cy.screenshot('visual-tablet', { capture: 'fullPage', overwrite: true })
  })

  it('mobile 375', { tags: '@visual' }, () => {
    cy.viewport(375, 667)
    cy.visit('/')
    cy.wait('@getReminders')
    cy.get('article').should('have.length', 6)
    cy.screenshot('visual-mobile', { capture: 'fullPage', overwrite: true })
  })
})
