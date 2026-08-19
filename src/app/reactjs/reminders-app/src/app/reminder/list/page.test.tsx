import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import RemindersList from './page';
import {
  mockQueryClient,
  mockReminders,
  mockUpdateMutateAsync,
} from '@/app/util/testMocks';

jest.mock(
  'next/navigation',
  require('@/app/util/testMocks').jestFunctionsMock['next/navigation'],
);
jest.mock('@/app/api', require('@/app/util/testMocks').jestFunctionsMock['@/app/api']);
jest.mock(
  '@/app/hooks',
  require('@/app/util/testMocks').jestFunctionsMock['@/app/hooks'],
);

describe('RemindersList', () => {
  afterEach(() => {
    jest.clearAllMocks();
  });

  it('renders reminders grouped with section headers', () => {
    render(<RemindersList />);

    expect(screen.getByText('Create Reminder')).toBeInTheDocument();

    // Mock dates are in the past: open reminder is Overdue, done one is Done.
    expect(screen.getByText('Overdue')).toBeInTheDocument();
    expect(screen.getByText('Done')).toBeInTheDocument();

    mockReminders.forEach(mockReminder => {
      expect(screen.getByText(mockReminder.title)).toBeInTheDocument();
      expect(screen.getByText(mockReminder.description)).toBeInTheDocument();
    });
  });

  it('handles Create Reminder click', () => {
    render(<RemindersList />);

    fireEvent.click(screen.getByText('Create Reminder'));

    expect(require('@/app/hooks').useRemindersClearContext).toHaveBeenCalled();
    expect(require('next/navigation').useRouter().push).toHaveBeenCalledWith(
      '/reminder/create',
    );
  });

  it('handles edit click on a card', () => {
    render(<RemindersList />);

    // First card is the open reminder (Overdue group renders before Done).
    fireEvent.click(screen.getAllByLabelText('Edit reminder')[0]);

    expect(require('@/app/hooks').useRemindersClearContext).toHaveBeenCalled();
    expect(require('next/navigation').useRouter().push).toHaveBeenCalledWith(
      '/reminder/edit?id=2',
    );
  });

  it('toggles a reminder optimistically', async () => {
    render(<RemindersList />);

    fireEvent.click(screen.getByLabelText('Mark done'));

    const openReminder = mockReminders.find(reminder => !reminder.isDone);
    const toggled = { ...openReminder, isDone: true };

    expect(mockQueryClient.setQueryData).toHaveBeenCalledWith(
      ['reminders'],
      expect.any(Function),
    );

    await waitFor(() => {
      expect(mockUpdateMutateAsync).toHaveBeenCalledWith(toggled);
    });
  });

  it('rolls back the optimistic toggle when the update fails', async () => {
    mockUpdateMutateAsync.mockResolvedValueOnce({
      errors: { BadRequest: 'failed' },
    });

    render(<RemindersList />);

    fireEvent.click(screen.getByLabelText('Mark done'));

    await waitFor(() => {
      expect(mockQueryClient.setQueryData).toHaveBeenLastCalledWith(
        ['reminders'],
        mockReminders,
      );
    });
  });
});
