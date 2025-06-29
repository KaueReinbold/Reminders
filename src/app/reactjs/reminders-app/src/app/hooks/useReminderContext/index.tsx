'use client';

import { Dispatch, ReactNode, useEffect, useReducer, useState } from 'react';
import { createContext, useContextSelector } from 'use-context-selector';
import { useParams } from 'next/navigation';

import { Errors, Reminder, useReminder, useReminderActions } from '@/app/api';

interface RemindersContextValue {
  reminder?: Reminder | null | undefined;
  errors?: Errors;
  error?: Error | null;
  dispatch: Dispatch<ReminderAction>;
  onCreateReminder: () => Promise<ReminderActionStatus>;
  onUpdateReminder: () => Promise<ReminderActionStatus>;
  onDeleteReminder: () => Promise<ReminderActionStatus>;
  clearReminder: () => void;
}

export enum ReminderActionStatus {
  Unknown,
  Success,
  Fail,
}

const RemindersContext = createContext<RemindersContextValue | undefined>(
  undefined,
);

export function useRemindersContext() {
  const context = useContextSelector(RemindersContext, state => state);

  if (!context) {
    throw new Error(
      'useRemindersContext must be used within a RemindersContextProvider',
    );
  }

  return context;
}

export function useRemindersClearContext() {
  const context = useContextSelector(
    RemindersContext,
    state => state?.clearReminder,
  );

  if (!context) {
    throw new Error(
      'useRemindersClearContext must be used within a RemindersContextProvider',
    );
  }

  return context;
}

type ReminderAction =
  | { type: 'SET_REMINDER'; payload: Reminder }
  | { type: 'UPDATE_REMINDER'; payload: Partial<Reminder> }
  | { type: 'CLEAR_REMINDER' };

const reminderReducer = (
  state: Reminder | null,
  action: ReminderAction,
): Reminder | null => {
  switch (action.type) {
    case 'SET_REMINDER':
      return action.payload;
    case 'UPDATE_REMINDER':
      return { ...state, ...action.payload } as Reminder | null;
    case 'CLEAR_REMINDER':
      return null;
    default:
      throw new Error();
  }
};

export function RemindersContextProvider({
  children,
}: {
  children: ReactNode;
}) {
  const { id } = useParams<{ id: string }>();

  const { data: reminderData, error } = useReminder(id);

  const queryError = error as Error | null;

  const { createReminder, updateReminder, deleteReminder } =
    useReminderActions();
  const [reminder, dispatch] = useReducer(reminderReducer, null);

  const [errors, setErrors] = useState<Errors>();

  const onCreateReminder = async (): Promise<ReminderActionStatus> => {
    try {
      if (reminder) {
        const result = await createReminder.mutateAsync(reminder);

        if (result?.errors) {
          setErrors(result?.errors);

          return ReminderActionStatus.Fail;
        }
      }
    } catch (error) {
      console.error(error);

      return ReminderActionStatus.Fail;
    }

    return ReminderActionStatus.Success;
  };

  const onUpdateReminder = async (): Promise<ReminderActionStatus> => {
    try {
      if (reminder) {
        const result = await updateReminder.mutateAsync(reminder);

        if (result?.errors) {
          setErrors(result?.errors);

          return ReminderActionStatus.Fail;
        }
      }
    } catch (error) {
      console.error(error);

      return ReminderActionStatus.Fail;
    }

    return ReminderActionStatus.Success;
  };

  const onDeleteReminder = async (): Promise<ReminderActionStatus> => {
    try {
      const result = await deleteReminder.mutateAsync(id);

      if (result?.errors) {
        setErrors(result?.errors);

        return ReminderActionStatus.Fail;
      }
    } catch (error) {
      console.error(error);

      return ReminderActionStatus.Fail;
    }

    return ReminderActionStatus.Success;
  };

  const clearReminder = () => {
    dispatch({ type: 'CLEAR_REMINDER' });
    setErrors(undefined);
  };

  useEffect(() => {
    if (reminderData) {
      dispatch({ type: 'SET_REMINDER', payload: reminderData });
    }
  }, [dispatch, reminderData]);

  const value: RemindersContextValue = {
    reminder,
    errors,
    error: queryError,
    dispatch,
    onCreateReminder,
    onUpdateReminder,
    onDeleteReminder,
    clearReminder,
  };

  return (
    <RemindersContext.Provider value={value}>
      {children}
    </RemindersContext.Provider>
  );
}
