'use client';

import { Dispatch, ReactNode, useEffect, useReducer, useState, useMemo } from 'react';
import { createContext, useContextSelector } from 'use-context-selector';
import { useParams } from 'next/navigation';

import { Errors, Reminder, useReminder, useReminderActions } from '@/app/api';
import ValidationService from '@/app/services/ValidationService';

interface RemindersContextValue {
  reminder?: Reminder | null | undefined;
  errors?: Errors;
  dispatch: Dispatch<ReminderAction>;
  onCreateReminder: () => Promise<ReminderActionStatus>;
  onUpdateReminder: () => Promise<ReminderActionStatus>;
  onDeleteReminder: () => Promise<ReminderActionStatus>;
  clearReminder: () => void;
  clearFieldError?: ClearFieldErrorFn;
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

export type ClearFieldErrorFn = (field?: string) => void;

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

  const { data: reminderData } = useReminder(id);
  const { createReminder, updateReminder, deleteReminder } = useReminderActions();

  // Default reminder for creation (memoized so identity is stable)
  const defaultReminder = useMemo(() => ({
    title: '',
    description: '',
    limitDate: '',
    isDone: false,
  }), []);

  // If on create page (no id), initialize with defaultReminder
  const [reminder, dispatch] = useReducer(
    reminderReducer,
    defaultReminder
  );

  const [errors, setErrors] = useState<Errors>();

  const onCreateReminder = async (): Promise<ReminderActionStatus> => {
    try {
      const payload = reminder ?? reminderData ?? defaultReminder;

      // Client-side validation before sending to API
      const clientErrors: Errors = {} as Errors;

      const titleError = ValidationService.validateTitle(payload.title);
      if (titleError) clientErrors.Title = [titleError];

      const descError = ValidationService.validateDescription(payload.description);
      if (descError) clientErrors.Description = [descError];

      const dateError = ValidationService.validateLimitDate(payload.limitDate ?? '');
      if (dateError) clientErrors['LimitDate.Date'] = [dateError];

      if (Object.keys(clientErrors).length > 0) {
        setErrors(clientErrors);
        return ReminderActionStatus.Fail;
      }

      const result = await createReminder.mutateAsync(payload as any);

      if (result?.errors && Object.keys(result?.errors).length > 0) {
        setErrors(result?.errors);

        return ReminderActionStatus.Fail;
      }
    } catch (error) {
      console.error(error);

      return ReminderActionStatus.Fail;
    }

    return ReminderActionStatus.Success;
  };

  const onUpdateReminder = async (): Promise<ReminderActionStatus> => {
    try {
      const payload = reminder ?? reminderData ?? defaultReminder;

      // Client-side validation before sending to API
      const clientErrors: Errors = {} as Errors;

      const titleError = ValidationService.validateTitle(payload.title);
      if (titleError) clientErrors.Title = [titleError];

      const descError = ValidationService.validateDescription(payload.description);
      if (descError) clientErrors.Description = [descError];

      const dateError = ValidationService.validateLimitDate(payload.limitDate ?? '');
      if (dateError) clientErrors['LimitDate.Date'] = [dateError];

      if (Object.keys(clientErrors).length > 0) {
        setErrors(clientErrors);
        return ReminderActionStatus.Fail;
      }

      const result = await updateReminder.mutateAsync(payload as any);

      if (result?.errors && Object.keys(result?.errors).length > 0) {
        setErrors(result?.errors);

        return ReminderActionStatus.Fail;
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

  const clearFieldError = (field?: string) => {
    if (!field) return;

    setErrors(prev => {
      if (!prev) return undefined;
      const copy = { ...prev } as Record<string, any>;
      if (copy[field]) delete copy[field];
      const keys = Object.keys(copy);
      return keys.length > 0 ? (copy as Errors) : undefined;
    });
  };

  useEffect(() => {
    if (reminderData) {
      dispatch({ type: 'SET_REMINDER', payload: { ...defaultReminder, ...reminderData } });
    }
  }, [dispatch, reminderData, defaultReminder]);

  const value: RemindersContextValue = {
    reminder,
    errors,
    dispatch,
    onCreateReminder,
    onUpdateReminder,
    onDeleteReminder,
    clearReminder,
    clearFieldError,
  };

  return (
    <RemindersContext.Provider value={value}>
      {children}
    </RemindersContext.Provider>
  );
}
