import ValidationService from '@/app/services/ValidationService';

import { mapReminder } from './mapReminder';
import { Errors, MutateResult, Reminder } from './types';

const MS_PER_DAY = 86400000;

const dateFromToday = (days: number): string =>
  new Date(Date.now() + days * MS_PER_DAY).toISOString().slice(0, 10);

const seed = (): Reminder[] =>
  [
    {
      id: '1',
      title: 'Renew the library books',
      description: 'Two novels are past their return date.',
      limitDate: dateFromToday(-2),
      isDone: false,
    },
    {
      id: '2',
      title: 'Stand-up with the team',
      description: 'Share the demo plan for the sprint review.',
      limitDate: dateFromToday(0),
      isDone: false,
    },
    {
      id: '3',
      title: 'Buy groceries',
      description: 'Coffee, oats, olive oil.',
      limitDate: dateFromToday(1),
      isDone: false,
    },
    {
      id: '4',
      title: 'Book flights',
      description: 'Compare fares before prices climb.',
      limitDate: dateFromToday(6),
      isDone: false,
    },
    {
      id: '5',
      title: 'Submit the expense report',
      description: 'Attach the receipts from last week.',
      limitDate: dateFromToday(-1),
      isDone: true,
    },
  ].map(mapReminder);

let reminders = seed();
let nextId = reminders.length + 1;

// The message every API returns for a limit date that is not in the future.
const INVALID_LIMIT_DATE = 'The Limit Date should be later than Today.';

// Whole-day comparison in UTC, matching the server rule: a later day, not a
// later instant. A missing date is the zero date on the server, so it fails too.
const isFutureDay = (limitDate: string): boolean => {
  const parsed = Date.parse(limitDate);

  if (Number.isNaN(parsed)) return false;

  const day = (ms: number) => Math.floor(ms / MS_PER_DAY);

  return day(parsed) > day(Date.now());
};

// Mirrors the API validation contract (ADR-0011) so the demo fails the same way
// the live APIs do: an errors map keyed by the camelCase JSON field name. The
// past date rule runs on create only, so an overdue reminder stays editable.
const validate = (reminder: Reminder, creating: boolean): Errors | null => {
  const errors = {} as Errors;
  const title = ValidationService.validateTitle(reminder.title);
  const description = ValidationService.validateDescription(
    reminder.description,
  );
  const limitDate = ValidationService.validateLimitDate(reminder.limitDate);

  if (title) errors.title = [title];
  if (description) errors.description = [description];
  if (limitDate) errors.limitDate = [limitDate];
  else if (creating && !isFutureDay(reminder.limitDate))
    errors.limitDate = [INVALID_LIMIT_DATE];

  return Object.keys(errors).length > 0 ? errors : null;
};

// A 404 carries no `errors` map, so the live client files its message under
// `request`. The mock does the same.
const notFound = (id?: string): Errors => ({
  request: [`Reminder ${id} was not found`],
});

const getReminders = (): Promise<Reminder[]> =>
  Promise.resolve(reminders.map(reminder => ({ ...reminder })));

const getReminder = (id: string): Promise<Reminder> => {
  const found = reminders.find(reminder => reminder.id === id);
  return Promise.resolve(found ? { ...found } : ({} as Reminder));
};

const createReminder = (
  reminder: Reminder,
): Promise<MutateResult<Reminder>> => {
  const errors = validate(reminder, true);

  if (errors) return Promise.resolve({ errors });

  const created = mapReminder({ ...reminder, id: String(nextId++) });
  reminders = [...reminders, created];

  return Promise.resolve({ result: { ...created } });
};

const updateReminder = (
  reminder: Reminder,
): Promise<MutateResult<Reminder>> => {
  const errors = validate(reminder, false);

  if (errors) return Promise.resolve({ errors });

  if (!reminders.some(item => item.id === reminder.id))
    return Promise.resolve({ errors: notFound(reminder.id) });

  const updated = mapReminder({ ...reminder });
  reminders = reminders.map(item => (item.id === updated.id ? updated : item));

  return Promise.resolve({ result: { ...updated } });
};

const deleteReminder = (id: string): Promise<MutateResult<string>> => {
  if (!reminders.some(reminder => reminder.id === id))
    return Promise.resolve({ errors: notFound(id) });

  reminders = reminders.filter(reminder => reminder.id !== id);

  return Promise.resolve({ result: id });
};

// Test helper: restores the seed data between cases.
const resetReminders = () => {
  reminders = seed();
  nextId = reminders.length + 1;
};

export {
  getReminders,
  getReminder,
  createReminder,
  updateReminder,
  deleteReminder,
  resetReminders,
};
