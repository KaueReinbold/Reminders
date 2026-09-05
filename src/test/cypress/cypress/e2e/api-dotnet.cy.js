// E2E coverage for the .NET API (no UI, no intercepts: real HTTP against the running API)
// Requires the api profile running: docker compose --profile api up -d
// API base URL comes from CYPRESS_apiUrl (default http://localhost:5000)

const api = () => Cypress.env('apiUrl')

const futureDate = (days = 7) =>
  new Date(Date.now() + days * 24 * 60 * 60 * 1000).toISOString()

describe('.NET API', { tags: '@api' }, () => {
  context('GET /api/reminders', () => {
    it('returns 200 with a list', () => {
      cy.request(`${api()}/api/reminders`).then((response) => {
        expect(response.status).to.eq(200)
        expect(response.body).to.be.an('array')
      })
    })

    it('returns the reminders count', () => {
      cy.request(`${api()}/api/reminders/count`).then((response) => {
        expect(response.status).to.eq(200)
        expect(response.body).to.be.a('number')
      })
    })
  })

  context('CRUD lifecycle', () => {
    it('creates, reads, updates and deletes a reminder', () => {
      const reminder = {
        title: 'Cypress API test',
        description: 'Created by api-dotnet.cy.js',
        limitDate: futureDate(),
        isDone: false,
      }

      // CREATE
      cy.request('POST', `${api()}/api/reminders`, reminder)
        .then((response) => {
          expect(response.status).to.eq(200)
          expect(response.body.id).to.be.a('string').and.not.be.empty
          expect(response.body.title).to.eq(reminder.title)
          expect(response.body.isDone).to.be.false
          return cy.wrap(response.body.id)
        })
        .then((id) => {
          // READ
          cy.request(`${api()}/api/reminders/${id}`).then((response) => {
            expect(response.status).to.eq(200)
            expect(response.body.id).to.eq(id)
            expect(response.body.title).to.eq(reminder.title)
          })

          // UPDATE
          const updated = {
            ...reminder,
            id,
            title: 'Cypress API test updated',
            isDone: true,
          }
          cy.request('PUT', `${api()}/api/reminders/${id}`, updated).then((response) => {
            expect(response.status).to.eq(200)
            expect(response.body.title).to.eq(updated.title)
            expect(response.body.isDone).to.be.true
          })

          // DELETE (soft delete: reminder leaves the list)
          cy.request('DELETE', `${api()}/api/reminders/${id}`).then((response) => {
            expect(response.status).to.eq(200)
          })

          cy.request(`${api()}/api/reminders`).then((response) => {
            const ids = response.body.map((r) => r.id)
            expect(ids).to.not.include(id)
          })
        })
    })
  })

  context('Validation', () => {
    it('rejects a reminder with a past limit date', () => {
      cy.request({
        method: 'POST',
        url: `${api()}/api/reminders`,
        body: {
          title: 'Invalid reminder',
          description: 'Limit date in the past',
          limitDate: '2020-01-01T00:00:00Z',
          isDone: false,
        },
        failOnStatusCode: false,
      }).then((response) => {
        expect(response.status).to.eq(400)
        expect(response.body.errors).to.have.property('limitDate')
      })
    })

    it('rejects an update when route id and body id do not match', () => {
      cy.request({
        method: 'PUT',
        url: `${api()}/api/reminders/00000000-0000-0000-0000-000000000001`,
        body: {
          id: '00000000-0000-0000-0000-000000000002',
          title: 'Mismatch',
          description: 'Ids do not match',
          limitDate: futureDate(),
          isDone: false,
        },
        failOnStatusCode: false,
      }).then((response) => {
        expect(response.status).to.eq(409)
      })
    })

    it('returns 404 for deleting a nonexistent reminder', () => {
      cy.request({
        method: 'DELETE',
        url: `${api()}/api/reminders/00000000-0000-0000-0000-00000000dead`,
        failOnStatusCode: false,
      }).then((response) => {
        expect(response.status).to.eq(404)
      })
    })
  })
})
