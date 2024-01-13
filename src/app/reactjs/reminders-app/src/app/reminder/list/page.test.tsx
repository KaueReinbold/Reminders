import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import RemindersList from './page';
import { mockReminders } from '@/app/util/testMocks';

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
    jest.restoreAllMocks();
  });

  it('should render without errors', async () => {
    render(<RemindersList />);

    expect(screen.getByText('Create Reminder')).toBeInTheDocument();

    await waitFor(async () => {
      mockReminders.forEach(mockReminder => {
        expect(screen.getByText(mockReminder.id)).toBeInTheDocument();
        expect(screen.getByText(mockReminder.title)).toBeInTheDocument();
        expect(screen.getByText(mockReminder.description)).toBeInTheDocument();
        expect(
          screen.getByText(mockReminder.limitDateFormatted),
        ).toBeInTheDocument();
        expect(
          screen.getByText(mockReminder.isDoneFormatted),
        ).toBeInTheDocument();
      });
    });
  });

  it('should handle Create Reminder click correctly', () => {
    render(<RemindersList />);

    fireEvent.click(screen.getByText('Create Reminder'));

    expect(require('@/app/hooks').useRemindersClearContext).toHaveBeenCalled();
    expect(require('next/navigation').useRouter().push).toHaveBeenCalledWith(
      '/reminder/create',
    );
  });

  it('should handle Edit click correctly', async () => {
    render(<RemindersList />);

    fireEvent.click(screen.getAllByText('Edit')[0]);

    expect(require('@/app/hooks').useRemindersClearContext).toHaveBeenCalled();
    expect(require('next/navigation').useRouter().push).toHaveBeenCalledWith(
      '/reminder/1',
    );
  });
});
