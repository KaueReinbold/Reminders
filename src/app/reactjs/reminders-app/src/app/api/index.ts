import { ApiError } from "next/dist/server/api-utils";
import { useMutation, useQuery, useQueryClient } from "react-query";

export type Reminder = {
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

export interface Errors {
  "LimitDate.Date": string[];
  "Description": string[];
  "Title": string[];
}

export class ValidationError extends Error {
  errors: Errors;

  constructor(message: string, errors: Errors) {
    super(message);
    this.name = 'ValidationError';
    this.errors = errors;
  }
}

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;
const REMINDER_QUERY_NAME = 'reminders';

const headers = {
  'Content-Type': 'application/json',
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = ('0' + (date.getMonth() + 1)).slice(-2); // Months are 0-indexed in JavaScript
  const day = ('0' + date.getDate()).slice(-2);
  return `${year}-${month}-${day}`;
}

const getErrors = async (response: Response) => {
  const apiError = await response.json() as APIError;
  const errors = Object.entries(apiError.errors)
    .reduce((prev, [key, value]) => ({ ...prev, [key]: value[0] }), {} as Errors);

  throw new ValidationError(apiError.title, errors);
}

const mapReminder = (reminder: Reminder) => ({
  ...reminder,
  limitDateFormatted: reminder.limitDate ? formatDate(reminder.limitDate) : '',
  isDoneFormatted: reminder.isDone ? 'Yes' : 'No',
} as Reminder);

const getReminders: Promise<Reminder[]> = fetch(`${API_BASE_URL}/api/reminders`)
  .then(response => response.json())
  .then(data => data?.map(mapReminder));

const getReminder = async (id: string) => fetch(`${API_BASE_URL}/api/reminders/${id}`)
  .then(response => response.json())
  .then(mapReminder);

const createReminder = async (reminder: Reminder) => {
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

    throw new Error('Failed to update reminder');
  }

  return await response.json();
};

const updateReminder = async (reminder: Reminder) => {
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

const deleteReminder = async (id: string) => {
  const response = await fetch(`${API_BASE_URL}/api/reminders/${id}`, {
    method: 'DELETE',
    headers,
  });

  if (!response.ok) {
    throw new Error('Failed to delete reminder');
  }

  return id;
};

// Hooks

export const useReminders = () =>
  useQuery(REMINDER_QUERY_NAME, () => getReminders);

export const useReminder = (id: string) =>
  useQuery(['reminder', id], () => getReminder(id));

export const useCreateReminder = () => {
  const queryClient = useQueryClient();

  return useMutation(createReminder, {
    onSuccess: () => {
      return queryClient.invalidateQueries(REMINDER_QUERY_NAME);
    },
  });
};

export const useUpdateReminder = () => {
  const queryClient = useQueryClient();

  return useMutation(updateReminder, {
    onSuccess: (data, variables) => {
      queryClient.setQueryData(['reminder', { id: variables.id }], data);
    },
  });
};

export const useDeleteReminder = () => {
  const queryClient = useQueryClient();

  return useMutation(deleteReminder, {
    onSuccess: (data, variables) => {
      queryClient.removeQueries(['reminder', { id: variables }]);
    },
  });
};