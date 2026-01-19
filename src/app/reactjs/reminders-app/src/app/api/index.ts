import { APIError, Errors, MutateResult, Reminder } from './types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

const headers = {
  'Content-Type': 'application/json',
};

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  const year = date.getUTCFullYear();
  const month = ('0' + (date.getUTCMonth() + 1)).slice(-2);
  const day = ('0' + date.getUTCDate()).slice(-2);
  return `${year}-${month}-${day}`;
};

const getErrors = async (response: Response): Promise<Errors> => {
  let errors = {} as Errors;

  try {
    const apiError = (await response.json()) as APIError;

    errors = apiError.errors;

    if (Object.keys(errors).length === 0) {
      errors = {
        BadRequest: apiError.title,
      };
    }
  } catch (error) {
    console.error(error);
    
    errors = {
      InternalServer: 'Failed to perform errors validation',
    } as Errors;
  }

  return errors;
};

const mapReminder = (reminder: Reminder): Reminder =>
  ({
    ...reminder,
    limitDateFormatted: reminder.limitDate
      ? formatDate(reminder.limitDate)
      : '',
    isDoneFormatted: reminder.isDone ? 'Yes' : 'No',
  }) as Reminder;

const getReminders = (): Promise<Reminder[]> =>
  fetch(`${API_BASE_URL}/api/reminders`)
    .then(response => response.json())
    .then(data => data?.map(mapReminder));

const getReminder = (id: string): Promise<Reminder> =>
  fetch(`${API_BASE_URL}/api/reminders/${id}`)
    .then(response => response.json())
    .then(mapReminder);

const createReminder = async (
  reminder: Reminder,
): Promise<MutateResult<Reminder>> => {
  const payload = {
    ...reminder,
    limitDate: reminder.limitDate
      ? reminder.limitDate.includes('T')
        ? reminder.limitDate
        : `${reminder.limitDate}T00:00:00Z`
      : '',
  };
  const body = JSON.stringify(payload);
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

const updateReminder = async (
  reminder: Reminder,
): Promise<MutateResult<Reminder>> => {
  const payload = {
    ...reminder,
    limitDate: reminder.limitDate
      ? reminder.limitDate.includes('T')
        ? reminder.limitDate
        : `${reminder.limitDate}T00:00:00Z`
      : '',
  };
  const body = JSON.stringify(payload);
  const response = await fetch(`${API_BASE_URL}/api/reminders/${reminder.id}`, {
    method: 'PUT',
    headers,
    body,
  });

  const result = { errors: [] } as MutateResult<Reminder>;

  if (response.ok) {
    result.result = await response.json();
  } else {
    result.errors = await getErrors(response);
  }

  return result;
};

const deleteReminder = async (id: string): Promise<MutateResult<string>> => {
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

export type { Reminder, Errors };

export {
  API_BASE_URL,
  getReminders,
  getReminder,
  createReminder,
  updateReminder,
  deleteReminder,
  getErrors,
};

export * from './hooks';
