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

// Mirrors the server-side validation so the demo shows the same error states.
const validate = (reminder: Reminder): Errors | null => {
  const errors = {} as Errors;
  const title = ValidationService.validateTitle(reminder.title);
  const description = ValidationService.validateDescription(
    reminder.description,
  );
  const limitDate = ValidationService.validateLimitDate(reminder.limitDate);

  if (title) errors.Title = [title];
  if (description) errors.Description = [description];
  if (limitDate) errors['LimitDate.Date'] = [limitDate];

  return Object.keys(errors).length > 0 ? errors : null;
};

const notFound = (id?: string): Errors => ({
  BadRequest: `Reminder ${id} was not found`,
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
  const errors = validate(reminder);

  if (errors) return Promise.resolve({ errors });

  const created = mapReminder({ ...reminder, id: String(nextId++) });
  reminders = [...reminders, created];

  return Promise.resolve({ result: { ...created } });
};

const updateReminder = (
  reminder: Reminder,
): Promise<MutateResult<Reminder>> => {
  const errors = validate(reminder);

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
