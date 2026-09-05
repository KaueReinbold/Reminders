export type Reminder = {
  id?: string;
  title: string;
  description: string;
  limitDate: string;
  limitDateFormatted?: string;
  isDone: boolean;
  isDoneFormatted?: string;
};

export type APIError = {
  type: string;
  title: string;
  status: number;
  detail?: string;
  errors?: Errors;
  traceId?: string;
};

// ADR-0011: every API error is RFC 7807 problem details. `errors` is present
// only on a validation failure, keyed by the camelCase JSON field name
// (`title`, `description`, `limitDate`). The client adds `request` for a
// failure the API did not attribute to a field, so nothing is swallowed.
export type Errors = Record<string, string[]>;

export type MutateResult<T> = {
  result?: T;
  errors?: Errors;
};
