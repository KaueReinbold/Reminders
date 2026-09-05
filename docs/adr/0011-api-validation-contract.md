# ADR-0011: One validation and error contract across the .NET, Go and C++ APIs

- **Status**: accepted
- **Date**: 2026-09-05
- **Issue**: #394

## Context

Three API implementations sit behind an nginx round robin (ADR-0003). A client
cannot know which one answers a request, so any difference between them is a
bug the user sees intermittently. They differed on all three of the things a
write request depends on:

| | .NET | Go | C++ |
|---|---|---|---|
| `limitDate` binding | `DateTimeOffset`, accepts date-only and RFC3339 | `time.Time`, RFC3339 only | plain string, never parsed |
| Business rules | `limitDate` later than today, on create and update | none | none |
| Error body | ASP.NET validation problem details on binding, a custom `{statusCode, message, properties}` from the exception handler | `{"message": "..."}` | `{"message": "..."}` |

Observed effects, found while testing #327: creating a reminder failed roughly
two attempts in three depending on which backend answered, and toggling an
overdue reminder returned 400 from .NET because the update path revalidated a
past date that was already stored.

## Decision

One contract, implemented in all three.

### Accepted `limitDate` input

Both `YYYY-MM-DD` and RFC3339 (`2026-09-30T00:00:00Z`). A date-only value means
midnight UTC on that day. Responses always serialize RFC3339 in UTC.

Accepting both is a superset of what any backend took before, so no existing
client breaks. The Flutter client sends date-only today.

### Past dates

Rejected on create, accepted on update. A reminder that is already overdue must
stay editable, which is what the create rule broke. A create with a past date is
almost always a typo, so the guard stays where it helps.

### Error body

RFC 7807 problem details, for every 4xx and 5xx, from all three
implementations:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "limitDate": ["The Limit Date should be later than Today."]
  }
}
```

`errors` is present only on validation failures, keyed by the JSON field name in
camel case, never by an internal property path. Non validation errors carry
`type`, `title`, `status` and, where useful, `detail`. Messages are the strings
the .NET API already returned, so a client cannot tell which backend answered:
the same failure gives a byte identical body from all three.

Status codes: 400 for a malformed body or a failed validation rule, 404 for a
missing reminder, 500 for anything unexpected.

Two things stay outside the contract because they are not error shape: the
success status of a create (.NET answers 200, Go and C++ answer 201) and the
`errors` detail of a malformed body, where the .NET framework's own model
binding failure carries its parser message and a `traceId`.

## Options considered

- **Flat `{"message"}` everywhere.** Smallest change, already what two of the
  three emit. Rejected: it cannot carry per-field errors, so the client cannot
  mark the offending input, and the React app would lose behaviour it has.
- **Keep each implementation as it is and normalize in nginx.** Rejected: it
  hides the divergence rather than fixing it, and nginx cannot rewrite a body it
  does not understand.

## Consequences

- The React client needs a follow up: it keys field errors off `Title`,
  `Description` and `LimitDate.Date`, and the contract sends `title`,
  `description` and `limitDate`. Until it is updated, a rejected create shows no
  message, because `errors` is now always present so the `BadRequest` banner
  fallback in `getErrors` no longer fires. It can drop that dual-shape handling
  from #393 at the same time, since one shape is guaranteed.
- Go and C++ each grow a small problem details helper and a validation step.
  Neither gains a dependency.
- The C++ implementation has to parse `limitDate` rather than pass the string
  through, which is the first real parsing it does.
- Go's `GetByID` currently maps every database error to not found (noted on
  #403). Aligning status codes means a real failure now surfaces as 500, and the
  repository test that pinned the old behaviour changes with it.
- .NET's `GET /api/reminders/{id}` answered 204 with an empty body for a missing
  reminder, found while checking the contract. It now throws the same not found
  error the delete and update paths already threw, so it answers 404.
- .NET normalizes `limitDate` through a JSON converter rather than relying on the
  host time zone, so a date only value is midnight UTC everywhere.
- Any new API implementation has one document to conform to, and the Cypress
  `api-contract.cy.js` spec checks it against every backend, including that the
  error bodies are identical.
