type Reminder = {
  id?: string;
  title: string;
  description: string;
  limitDate: string;
  limitDateFormatted?: string;
  isDone: boolean;
  isDoneFormatted?: string;
};

interface APIError {
  type: string;
  title: string;
  status: number;
  errors: Errors;
  traceId: string;
}

interface Errors {
  'LimitDate.Date': string[];
  Description: string[];
  Title: string[];
  ServerError: string;
}

class ValidationError extends Error {
  errors: Errors;

  constructor(message: string, errors: Errors) {
    super(message);
    this.name = 'ValidationError';
    this.errors = errors;
  }
}

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

const headers = {
  'Content-Type': 'application/json',
};

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = ('0' + (date.getMonth() + 1)).slice(-2); // Months are 0-indexed in JavaScript
  const day = ('0' + date.getDate()).slice(-2);
  return `${year}-${month}-${day}`;
};

const getErrors = async (response: Response) => {
  const apiError = (await response.json()) as APIError;
  const errors = Object.entries(apiError.errors).reduce(
    (prev, [key, value]) => ({ ...prev, [key]: value[0] }),
    {} as Errors,
  );

  throw new ValidationError(apiError.title, errors);
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

const createReminder = async (reminder: Reminder): Promise<Reminder> => {
  const body = JSON.stringify(reminder);
  const response = await fetch(`${API_BASE_URL}/api/reminders`, {
    method: 'POST',
    headers,
    body,
  });

  if (!response.ok) {
    if (response.status === 400) {
      await getErrors(response);
    }

    throw new Error('Failed to create reminder');
  }

  return await response.json();
};

const updateReminder = async (reminder: Reminder): Promise<Reminder> => {
  const body = JSON.stringify(reminder);
  const response = await fetch(`${API_BASE_URL}/api/reminders/${reminder.id}`, {
    method: 'PUT',
    headers,
    body,
  });

  if (!response.ok) {
    if (response.status === 400) {
      return getErrors(response);
    }

    throw new Error('Failed to update reminder');
  }

  return await response.json();
};

const deleteReminder = async (id: string): Promise<string> => {
  const response = await fetch(`${API_BASE_URL}/api/reminders/${id}`, {
    method: 'DELETE',
    headers,
  });

  if (!response.ok) {
    throw new Error('Failed to delete reminder');
  }

  return id;
};

export type { Reminder, Errors };

export { ValidationError };

export {
  API_BASE_URL,
  getReminders,
  getReminder,
  createReminder,
  updateReminder,
  deleteReminder,
};

export * from './hooks';
