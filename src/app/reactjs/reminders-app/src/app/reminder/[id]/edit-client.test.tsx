import { render, screen, waitFor } from '@testing-library/react';
import { useRouter } from 'next/navigation';
import EditClient from './edit-client';
import { useRemindersContext } from '@/app/hooks';

// Mock dependencies
jest.mock('next/navigation', () => ({
  useRouter: jest.fn(),
}));

jest.mock('@/app/hooks', () => ({
  useRemindersContext: jest.fn(),
  ReminderActionStatus: {
    Success: 1,
    Fail: 2,
  },
}));

describe('EditClient 404 Error Handling', () => {
  const mockPush = jest.fn();

  beforeEach(() => {
    (useRouter as jest.Mock).mockReturnValue({
      push: mockPush,
    });
    mockPush.mockClear();
  });

  it('should render NotFoundReminder component when 404 error occurs', async () => {
    // Mock context with 404 error
    (useRemindersContext as jest.Mock).mockReturnValue({
      onUpdateReminder: jest.fn(),
      onDeleteReminder: jest.fn(),
      error: new Error('HTTP 404'),
    });

    render(<EditClient />);

    // Should display 404 page
    await waitFor(() => {
      expect(screen.getByText('404 - Reminder Not Found')).toBeInTheDocument();
    });
    
    expect(
      screen.getByText(
        "The reminder you're looking for doesn't exist or may have been deleted."
      )
    ).toBeInTheDocument();
    
    expect(screen.getByRole('button', { name: 'Back to Home' })).toBeInTheDocument();
  });

  it('should render normal edit form when no error', async () => {
    // Mock context without error
    (useRemindersContext as jest.Mock).mockReturnValue({
      onUpdateReminder: jest.fn(),
      onDeleteReminder: jest.fn(),
      error: null,
    });

    render(<EditClient />);

    // Should not display 404 page
    expect(screen.queryByText('404 - Reminder Not Found')).not.toBeInTheDocument();
  });

  it('should render normal edit form when error is not 404', async () => {
    // Mock context with non-404 error
    (useRemindersContext as jest.Mock).mockReturnValue({
      onUpdateReminder: jest.fn(),
      onDeleteReminder: jest.fn(),
      error: new Error('HTTP 500'),
    });

    render(<EditClient />);

    // Should not display 404 page for non-404 errors
    expect(screen.queryByText('404 - Reminder Not Found')).not.toBeInTheDocument();
  });
});