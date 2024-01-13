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
  errors: Errors;
  traceId: string;
};

export type Errors = {
  'LimitDate.Date'?: string[];
  Description?: string[];
  Title?: string[];
  InternalServer?: string;
  BadRequest?: string;
};

export type MutateResult<T> = {
  result: T;
  errors: Errors;
};
