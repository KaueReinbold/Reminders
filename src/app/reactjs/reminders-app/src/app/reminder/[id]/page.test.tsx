import { render, screen, fireEvent } from '@testing-library/react';
import Edit from './page';
import { ReminderActionStatus } from '@/app/hooks';

jest.mock(
  'next/navigation',
  require('@/app/util/testMocks').jestFunctionsMock['next/navigation'],
);
jest.mock(
  '@/app/hooks',
  require('@/app/util/testMocks').jestFunctionsMock['@/app/hooks'],
);

describe('Edit Component', () => {
  afterEach(() => {
    jest.restoreAllMocks();
  });

  it('should render Edit component', () => {
    render(<Edit />);

    expect(screen.getByText('Edit')).toBeInTheDocument();
    expect(screen.getByText('Delete')).toBeInTheDocument();
    expect(screen.getByText('Back')).toBeInTheDocument();
  });

  it('should call onUpdateReminder and handles form submission', async () => {
    render(<Edit />);

    fireEvent.click(screen.getByText('Edit'));

    await screen.findByText('Back');

    expect(
      require('@/app/hooks').useRemindersContext().onUpdateReminder,
    ).toHaveBeenCalled();
    expect(require('next/navigation').useRouter().push).toHaveBeenCalledWith(
      '/',
    );
  });

  it('should call onUpdateReminder and not redirect', async () => {
    jest
      .spyOn(require('@/app/hooks').useRemindersContext(), 'onUpdateReminder')
      .mockResolvedValue(ReminderActionStatus.Fail);

    render(<Edit />);

    fireEvent.click(screen.getByText('Edit'));

    await screen.findByText('Back');

    expect(
      require('@/app/hooks').useRemindersContext().onUpdateReminder,
    ).toHaveBeenCalled();
    expect(require('next/navigation').useRouter().push).not.toHaveBeenCalled();
  });

  it('should call onDeleteReminder and handles delete button click', async () => {
    render(<Edit />);

    fireEvent.click(screen.getByText('Delete'));

    await screen.findByText('Are you sure you want to delete this reminder?');

    const deleteButton = screen.getByTestId('delete-button');
    fireEvent.click(deleteButton);

    await screen.findByText('Back');

    expect(
      require('@/app/hooks').useRemindersContext().onDeleteReminder,
    ).toHaveBeenCalled();
    expect(require('next/navigation').useRouter().push).toHaveBeenCalledWith(
      '/',
    );
  });

  it('should call onDeleteReminder and not redirect', async () => {
    jest
      .spyOn(require('@/app/hooks').useRemindersContext(), 'onDeleteReminder')
      .mockResolvedValue(ReminderActionStatus.Fail);

    render(<Edit />);

    fireEvent.click(screen.getByText('Delete'));

    await screen.findByText('Are you sure you want to delete this reminder?');

    const deleteButton = screen.getByTestId('delete-button');
    fireEvent.click(deleteButton);

    await screen.findByText('Back');

    expect(
      require('@/app/hooks').useRemindersContext().onDeleteReminder,
    ).toHaveBeenCalled();
    expect(require('next/navigation').useRouter().push).not.toHaveBeenCalled();
  });

  it('should handle Back button click', () => {
    render(<Edit />);

    fireEvent.click(screen.getByText('Back'));

    expect(require('next/navigation').useRouter().push).toHaveBeenCalledWith(
      '/',
    );
  });
});
