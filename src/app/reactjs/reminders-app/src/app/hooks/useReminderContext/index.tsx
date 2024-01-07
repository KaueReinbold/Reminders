'use client';

import { Dispatch, ReactNode, useEffect, useReducer, useState } from 'react';
import { createContext, useContextSelector } from 'use-context-selector';
import { useParams } from 'next/navigation';

import {
  Errors,
  Reminder,
  ValidationError,
  useReminder,
  useReminderActions,
} from '@/app/api';

interface RemindersContextValue {
  reminder?: Reminder | null | undefined;
  errors?: Errors;
  dispatch: Dispatch<ReminderAction>;
  onCreateReminder: () => Promise<void>;
  onUpdateReminder: () => Promise<void>;
  onDeleteReminder: () => Promise<void>;
  clearReminder: () => void;
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
      'useRemindersContext must be used within a RemindersContextProvider',
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

  const { data: reminderData } = useReminder(id);

  const { createReminder, updateReminder, deleteReminder } =
    useReminderActions();
  const [reminder, dispatch] = useReducer(reminderReducer, null);

  const [errors, setErrors] = useState<Errors>();

  const onCreateReminder = async () => {
    try {
      if (reminder) {
        await createReminder.mutateAsync(reminder);
      }
    } catch (error) {
      validateErrors(error);
    }
  };

  const onUpdateReminder = async () => {
    try {
      if (reminder) {
        await updateReminder.mutateAsync(reminder);
      }
    } catch (error) {
      validateErrors(error);
    }
  };

  const onDeleteReminder = async () => {
    try {
      await deleteReminder.mutateAsync(id);
    } catch (error) {
      validateErrors(error);
    }
  };

  const validateErrors = (error: unknown) => {
    if (error instanceof ValidationError) {
      setErrors(error.errors);
    } else if (error instanceof Error) {
      setErrors({ ServerError: error.message } as Errors);
    }
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