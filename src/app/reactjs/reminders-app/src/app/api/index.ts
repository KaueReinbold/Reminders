import { mapReminder } from './mapReminder';
import * as mock from './mock';
import { APIError, Errors, MutateResult, Reminder } from './types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

// Demo mode: serve reminders from an in-browser store instead of the API.
// Enabled by the GitHub Pages build, off everywhere else.
const IS_MOCK_API = process.env.NEXT_PUBLIC_MOCK_API === 'true';

const headers = {
  'Content-Type': 'application/json',
};

const getErrors = async (response: Response): Promise<Errors> => {
  try {
    const apiError = (await response.json()) as APIError;
    const errors = apiError.errors ?? {};

    // A non validation failure carries no `errors` map, only `title`/`detail`.
    return Object.keys(errors).length > 0
      ? errors
      : {
          request: [
            apiError.detail ??
              apiError.title ??
              apiError.message ??
              'Request failed',
          ],
        };
  } catch (error) {
    console.error(error);

    return { request: ['Failed to perform errors validation'] };
  }
};

const liveGetReminders = (): Promise<Reminder[]> =>
  fetch(`${API_BASE_URL}/api/reminders`)
    .then(response => response.json())
    .then(data => data?.map(mapReminder));

const liveGetReminder = (id: string): Promise<Reminder> =>
  fetch(`${API_BASE_URL}/api/reminders/${id}`)
    .then(response => response.json())
    .then(mapReminder);

const liveCreateReminder = async (
  reminder: Reminder,
): Promise<MutateResult<Reminder>> => {
  const body = JSON.stringify(reminder);
  const response = await fetch(`${API_BASE_URL}/api/reminders`, {
    method: 'POST',
    headers,
    body,
  });

  const result = {} as MutateResult<Reminder>;

  if (response.ok) {
    result.result = await response.json();
  } else {
    result.errors = await getErrors(response);
  }

  return result;
};

const liveUpdateReminder = async (
  reminder: Reminder,
): Promise<MutateResult<Reminder>> => {
  const body = JSON.stringify(reminder);
  const response = await fetch(`${API_BASE_URL}/api/reminders/${reminder.id}`, {
    method: 'PUT',
    headers,
    body,
  });

  const result = {} as MutateResult<Reminder>;

  if (response.ok) {
    result.result = await response.json();
  } else {
    result.errors = await getErrors(response);
  }

  return result;
};

const liveDeleteReminder = async (
  id: string,
): Promise<MutateResult<string>> => {
  const response = await fetch(`${API_BASE_URL}/api/reminders/${id}`, {
    method: 'DELETE',
    headers,
  });

  const result = {} as MutateResult<string>;

  if (response.ok) {
    result.result = id;
  } else {
    result.errors = await getErrors(response);
  }

  return result;
};

const getReminders = IS_MOCK_API ? mock.getReminders : liveGetReminders;
const getReminder = IS_MOCK_API ? mock.getReminder : liveGetReminder;
const createReminder = IS_MOCK_API ? mock.createReminder : liveCreateReminder;
const updateReminder = IS_MOCK_API ? mock.updateReminder : liveUpdateReminder;
const deleteReminder = IS_MOCK_API ? mock.deleteReminder : liveDeleteReminder;

export type { Reminder, Errors };

export {
  API_BASE_URL,
  IS_MOCK_API,
  getReminders,
  getReminder,
  createReminder,
  updateReminder,
  deleteReminder,
  getErrors,
};

export * from './errors';

export * from './hooks';
