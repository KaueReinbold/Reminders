import {
  createReminder,
  deleteReminder,
  getReminder,
  getReminders,
  updateReminder,
} from '../index';
import { useMutation, useQuery } from '@/app/hooks';

const REMINDER_QUERY_NAME = 'reminders';

const useReminders = () => useQuery(REMINDER_QUERY_NAME, getReminders);

const useReminder = (id: string) =>
  useQuery(['reminder', id], () => getReminder(id), { enabled: Boolean(id) });

const useCreateReminder = () => {
  return useMutation(createReminder);
};

const useUpdateReminder = () => {
  return useMutation(updateReminder);
};

const useDeleteReminder = () => {
  return useMutation(deleteReminder);
};

const useReminderActions = () => {
  return {
    createReminder: useCreateReminder(),
    updateReminder: useUpdateReminder(),
    deleteReminder: useDeleteReminder(),
  };
};

export {
  useReminders,
  useReminder,
  useCreateReminder,
  useUpdateReminder,
  useDeleteReminder,
  useReminderActions,
};
