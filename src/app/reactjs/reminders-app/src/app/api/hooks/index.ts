import {
  createReminder,
  deleteReminder,
  getReminder,
  getReminders,
  updateReminder,
} from '@/app/api';
import { useMutation, useQuery } from '@/app/hooks';

const REMINDER_QUERY_NAME = 'reminders';

const useReminders = () => useQuery({
  queryKey: [REMINDER_QUERY_NAME],
  queryFn: getReminders,
});

const useReminder = (id: string) =>
  useQuery({
    queryKey: ['reminder', id],
    queryFn: () => getReminder(id),
    enabled: Boolean(id),
  });

const useCreateReminder = () => {
  return useMutation({
    mutationFn: createReminder,
  });
};

const useUpdateReminder = () => {
  return useMutation({
    mutationFn: updateReminder,
  });
};

const useDeleteReminder = () => {
  return useMutation({
    mutationFn: deleteReminder,
  });
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
