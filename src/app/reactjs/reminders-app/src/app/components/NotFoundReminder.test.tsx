import { render, screen, fireEvent } from '@testing-library/react';
import { useRouter } from 'next/navigation';
import NotFoundReminder from './NotFoundReminder';

// Mock Next.js router
jest.mock('next/navigation', () => ({
  useRouter: jest.fn(),
}));

describe('NotFoundReminder', () => {
  const mockPush = jest.fn();

  beforeEach(() => {
    (useRouter as jest.Mock).mockReturnValue({
      push: mockPush,
    });
    mockPush.mockClear();
  });

  it('should render 404 error message', () => {
    render(<NotFoundReminder />);

    expect(screen.getByText('404 - Reminder Not Found')).toBeInTheDocument();
    expect(
      screen.getByText(
        "The reminder you're looking for doesn't exist or may have been deleted."
      )
    ).toBeInTheDocument();
  });

  it('should render Back to Home button', () => {
    render(<NotFoundReminder />);

    const backButton = screen.getByRole('button', { name: 'Back to Home' });
    expect(backButton).toBeInTheDocument();
  });

  it('should navigate to home when Back to Home button is clicked', () => {
    render(<NotFoundReminder />);

    const backButton = screen.getByRole('button', { name: 'Back to Home' });
    fireEvent.click(backButton);

    expect(mockPush).toHaveBeenCalledWith('/');
  });
});