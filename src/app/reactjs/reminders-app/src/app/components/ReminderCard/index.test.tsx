import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';

import { Reminder } from '@/app/api/types';
import { ReminderCard } from './index';

const dateOnly = (date: Date) => {
  const year = date.getFullYear();
  const month = `0${date.getMonth() + 1}`.slice(-2);
  const day = `0${date.getDate()}`.slice(-2);
  return `${year}-${month}-${day}`;
};

const shiftedDate = (days: number) => {
  const date = new Date();
  date.setDate(date.getDate() + days);
  return dateOnly(date);
};

const baseReminder: Reminder = {
  id: '1',
  title: 'Card Title',
  description: 'Card Description',
  limitDate: shiftedDate(1),
  limitDateFormatted: shiftedDate(1),
  isDone: false,
};

describe('ReminderCard', () => {
  it('renders title, description, and date pill', () => {
    render(
      <ReminderCard
        reminder={baseReminder}
        onToggle={jest.fn()}
        onEdit={jest.fn()}
      />,
    );

    expect(screen.getByText('Card Title')).toBeInTheDocument();
    expect(screen.getByText('Card Description')).toBeInTheDocument();
    expect(screen.getByText('Tomorrow')).toBeInTheDocument();
  });

  it('shows overdue label for a late open reminder', () => {
    render(
      <ReminderCard
        reminder={{
          ...baseReminder,
          limitDate: shiftedDate(-1),
          limitDateFormatted: shiftedDate(-1),
        }}
        onToggle={jest.fn()}
        onEdit={jest.fn()}
      />,
    );

    expect(screen.getByText('Yesterday')).toBeInTheDocument();
  });

  it('calls onToggle with the reminder when the checkbox is clicked', () => {
    const onToggle = jest.fn();

    render(
      <ReminderCard
        reminder={baseReminder}
        onToggle={onToggle}
        onEdit={jest.fn()}
      />,
    );

    fireEvent.click(screen.getByLabelText('Mark done'));

    expect(onToggle).toHaveBeenCalledWith(baseReminder);
  });

  it('labels the checkbox for a done reminder', () => {
    render(
      <ReminderCard
        reminder={{ ...baseReminder, isDone: true }}
        onToggle={jest.fn()}
        onEdit={jest.fn()}
      />,
    );

    expect(screen.getByLabelText('Mark not done')).toBeInTheDocument();
  });

  it('calls onEdit with the reminder id when the edit button is clicked', () => {
    const onEdit = jest.fn();

    render(
      <ReminderCard
        reminder={baseReminder}
        onToggle={jest.fn()}
        onEdit={onEdit}
      />,
    );

    fireEvent.click(screen.getByLabelText('Edit reminder'));

    expect(onEdit).toHaveBeenCalledWith('1');
  });
});
