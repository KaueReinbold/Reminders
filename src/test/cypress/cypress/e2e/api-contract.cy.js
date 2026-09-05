// The shared validation and error contract (ADR-0011), checked against every backend.
// No UI, no intercepts: real HTTP against each API directly, bypassing the nginx
// round robin so a failure names the implementation that broke the contract.
// Requires the api profile running: docker compose --profile api up -d
// URLs come from CYPRESS_apiUrl, CYPRESS_goApiUrl and CYPRESS_cppApiUrl.

const backends = () => [
  { name: '.NET', url: Cypress.env('apiUrl') },
  { name: 'Go', url: Cypress.env('goApiUrl') },
  { name: 'C++', url: Cypress.env('cppApiUrl') },
]

const CLIENT_ERROR_TYPE = 'https://tools.ietf.org/html/rfc9110#section-15.5.1'
const INVALID_LIMIT_DATE = 'The Limit Date should be later than Today.'

const dateOnly = (days) =>
  new Date(Date.now() + days * 24 * 60 * 60 * 1000).toISOString().slice(0, 10)

// Every backend answers these with the same body, collected here so the last
// tests can compare them.
const pastDateProblems = {}
const notFoundProblems = {}

describe('API contract', { tags: '@api' }, () => {
  backends().forEach(({ name, url }) => {
    context(name, () => {
      it('accepts a date only limitDate and answers RFC3339 in UTC', () => {
        cy.request('POST', `${url}/api/reminders`, {
          title: 'Contract date only',
          description: 'limitDate sent as YYYY-MM-DD',
          limitDate: dateOnly(7),
          isDone: false,
        }).then((response) => {
          expect(response.status).to.be.oneOf([200, 201])
          expect(response.body.limitDate).to.match(/^\d{4}-\d{2}-\d{2}T00:00:00(\.0+)?Z$/)
          expect(response.body.limitDate.slice(0, 10)).to.eq(dateOnly(7))

          cy.request('DELETE', `${url}/api/reminders/${response.body.id}`)
        })
      })

      it('rejects a past limitDate on create with problem details', () => {
        cy.request({
          method: 'POST',
          url: `${url}/api/reminders`,
          body: {
            title: 'Contract past date',
            description: 'limitDate in the past',
            limitDate: '2020-01-01',
            isDone: false,
          },
          failOnStatusCode: false,
        }).then((response) => {
          expect(response.status).to.eq(400)
          expect(response.headers['content-type']).to.contain('application/problem+json')
          expect(response.body.type).to.eq(CLIENT_ERROR_TYPE)
          expect(response.body.status).to.eq(400)
          expect(response.body.errors.limitDate).to.deep.eq([INVALID_LIMIT_DATE])

          pastDateProblems[name] = response.body
        })
      })

      it('accepts a past limitDate on update so an overdue reminder stays editable', () => {
        cy.request('POST', `${url}/api/reminders`, {
          title: 'Contract overdue',
          description: 'Created in the future, moved to the past',
          limitDate: dateOnly(1),
          isDone: false,
        }).then((created) => {
          const id = created.body.id

          cy.request('PUT', `${url}/api/reminders/${id}`, {
            id,
            title: 'Contract overdue',
            description: 'Created in the future, moved to the past',
            limitDate: '2020-01-01',
            isDone: true,
          }).then((response) => {
            expect(response.status).to.eq(200)
            expect(response.body.isDone).to.be.true
          })

          cy.request('DELETE', `${url}/api/reminders/${id}`)
        })
      })

      it('answers a missing reminder with 404 problem details', () => {
        cy.request({
          method: 'GET',
          url: `${url}/api/reminders/00000000-0000-0000-0000-00000000dead`,
          failOnStatusCode: false,
        }).then((response) => {
          expect(response.status).to.eq(404)
          expect(response.headers['content-type']).to.contain('application/problem+json')
          expect(response.body.status).to.eq(404)
          expect(response.body.type).to.eq(CLIENT_ERROR_TYPE)
          expect(response.body.title).to.be.a('string').and.not.be.empty

          notFoundProblems[name] = response.body
        })
      })

      it('answers a malformed body with 400 problem details', () => {
        cy.request({
          method: 'POST',
          url: `${url}/api/reminders`,
          body: '{not json',
          headers: { 'Content-Type': 'application/json' },
          failOnStatusCode: false,
        }).then((response) => {
          expect(response.status).to.eq(400)
          expect(response.headers['content-type']).to.contain('application/problem+json')
          expect(response.body.status).to.eq(400)
          expect(response.body.title).to.be.a('string').and.not.be.empty
        })
      })
    })
  })

  // Declared last on purpose: mocha runs a suite's own tests before its child
  // suites, so these comparisons live in a suite of their own.
  context('every backend', () => {
    const sameEverywhere = (collected) => {
      const bodies = backends().map(({ name }) => collected[name])

      expect(bodies.filter(Boolean)).to.have.length(backends().length)
      bodies.forEach((body) => expect(body).to.deep.eq(bodies[0]))
    }

    it('returns the same validation error body', () => sameEverywhere(pastDateProblems))

    it('returns the same not found body', () => sameEverywhere(notFoundProblems))
  })
})
