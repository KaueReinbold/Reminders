import {
  REMINDER_QUERY_NAME,
  createReminder,
  deleteReminder,
  getReminder,
  getReminders,
  updateReminder,
} from '../index';
import { useMutation, useQuery, useRemindersQueryClient } from '@/app/hooks';

const useReminders = () => useQuery(REMINDER_QUERY_NAME, () => getReminders);

const useReminder = (id: string) =>
  useQuery(['reminder', id], () => getReminder(id), { enabled: Boolean(id) });

const useCreateReminder = () => {
  const queryClient = useRemindersQueryClient();

  return useMutation(createReminder, {
    onSuccess: () => {
      return queryClient.invalidateQueries(REMINDER_QUERY_NAME);
    },
  });
};

const useUpdateReminder = () => {
  const queryClient = useRemindersQueryClient();

  return useMutation(updateReminder, {
    onSuccess: (data, variables) => {
      queryClient.setQueryData(['reminder', { id: variables.id }], data);
    },
  });
};

const useDeleteReminder = () => {
  const queryClient = useRemindersQueryClient();

  return useMutation(deleteReminder, {
    onSuccess: (data, variables) => {
      queryClient.removeQueries(['reminder', { id: variables }]);
    },
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
