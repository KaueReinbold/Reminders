import { Errors } from './types';

// The fields the forms render an error next to. Anything else goes to a banner,
// so an error the UI cannot place still reaches the user.
const FIELD_ERROR_KEYS = ['title', 'description', 'limitDate'];

export const fieldError = (errors: Errors | undefined, field: string) =>
  errors?.[field]?.join(' ');

export const unmappedError = (errors?: Errors) => {
  const messages = Object.entries(errors ?? {})
    .filter(([key]) => !FIELD_ERROR_KEYS.includes(key))
    .flatMap(([, value]) => value ?? []);

  return messages.length > 0 ? messages.join(' ') : undefined;
};
