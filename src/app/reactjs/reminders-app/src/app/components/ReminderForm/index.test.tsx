import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ReminderForm } from '.';
import { mockReminder } from '@/app/util/testMocks';

jest.mock(
  '@/app/hooks',
  require('@/app/util/testMocks').jestMocks['@/app/hooks'],
);

describe('ReminderForm', () => {
  afterEach(() => {
    jest.restoreAllMocks();
  });

  it('should render without errors', async () => {
    render(<ReminderForm />);

    expect(screen.findByLabelText('Title')).resolves.toBeInTheDocument();
    expect(screen.findByLabelText('Description')).resolves.toBeInTheDocument();
    expect(screen.findByLabelText('Limit Date')).resolves.toBeInTheDocument();
  });

  it('should render without errors editing', async () => {
    render(<ReminderForm editing />);

    expect(screen.queryByLabelText('Id')).toBeInTheDocument();
    expect(screen.findByLabelText('Title')).resolves.toBeInTheDocument();
    expect(screen.findByLabelText('Description')).resolves.toBeInTheDocument();
    expect(screen.findByLabelText('Limit Date')).resolves.toBeInTheDocument();
    expect(screen.queryByLabelText('Done')).toBeInTheDocument();
    expect(screen.queryByLabelText('Done')).toBeChecked();
  });

  it('handles user input correctly', async () => {
    render(<ReminderForm />);

    fireEvent.change(screen.getByTestId('title'), {
      target: { value: 'Updated Title' },
    });
    fireEvent.change(screen.getByTestId('description'), {
      target: { value: 'Updated Description' },
    });
    fireEvent.change(screen.getByTestId('limitDate'), {
      target: { value: '2023-12-31' },
    });

    expect(screen.findByTestId('title')).resolves.toHaveValue('Updated Title');
    expect(screen.findByTestId('description')).resolves.toHaveValue(
      'Updated Description',
    );
    expect(screen.findByTestId('limitDate')).resolves.toHaveValue('2023-12-31');
  });

  it('should display the error messages correctly', () => {
    render(<ReminderForm />);

    jest.spyOn(console, 'error').mockImplementation(() => {});

    fireEvent.change(screen.getByTestId('title'), { target: { value: '' } });
    fireEvent.change(screen.getByTestId('description'), {
      target: { value: '' },
    });
    fireEvent.change(screen.getByTestId('limitDate'), {
      target: { value: '' },
    });

    expect(
      screen.findByText('Title cannot be empty'),
    ).resolves.toBeInTheDocument();
    expect(
      screen.findByText('Description cannot be empty'),
    ).resolves.toBeInTheDocument();
    expect(
      screen.findByText('Limit Date cannot be empty'),
    ).resolves.toBeInTheDocument();
  });

  it('should handles "Done" checkbox correctly in editing mode', async () => {
    render(<ReminderForm editing />);

    expect(screen.getByLabelText('Done')).toBeInTheDocument();

    fireEvent.change(screen.getByTestId('isDone'), {
      target: { checked: false },
    });

    expect(await screen.findByTestId('isDone')).not.toBeChecked();
  });

  it('should hide Id and IsDone when not editing', async () => {
    render(<ReminderForm />);

    expect(screen.queryByTestId('reminderId')).not.toBeInTheDocument();
    expect(screen.queryByTestId('isDone')).not.toBeInTheDocument();
  });

  it('should call dispatch', async () => {
    const dispatch = jest.fn();
    jest
      .spyOn(require('@/app/hooks'), 'useRemindersContext')
      .mockImplementation(() => ({
        reminder: mockReminder,
        dispatch,
      }));

    render(<ReminderForm editing />);

    fireEvent.change(screen.getByTestId('title'), {
      target: { value: 'Updated Title' },
    });
    fireEvent.change(screen.getByTestId('description'), {
      target: { value: 'Updated Description' },
    });
    fireEvent.change(screen.getByTestId('limitDate'), {
      target: { value: '2023-12-31' },
    });

    await userEvent.click(screen.getByTestId('isDone'));

    expect(dispatch).toHaveBeenCalledWith({
      type: 'UPDATE_REMINDER',
      payload: { title: 'Updated Title' },
    });
    expect(dispatch).toHaveBeenCalledWith({
      type: 'UPDATE_REMINDER',
      payload: { description: 'Updated Description' },
    });
    expect(dispatch).toHaveBeenCalledWith({
      type: 'UPDATE_REMINDER',
      payload: { limitDate: '2023-12-31' },
    });
    expect(dispatch).toHaveBeenCalledWith({
      type: 'UPDATE_REMINDER',
      payload: { isDone: false },
    });
  });
});
