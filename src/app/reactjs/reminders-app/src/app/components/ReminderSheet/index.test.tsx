import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';

import { ReminderSheet } from './index';

const reminder = {
  id: '1',
  title: 'Call the notary',
  description: 'Ask about the deed',
  limitDate: '2026-01-22T00:00:00+00:00',
  limitDateFormatted: '2026-01-22',
  isDone: false,
};

describe('ReminderSheet', () => {
  it('opens in create mode with a blank draft due tomorrow', () => {
    render(
      <ReminderSheet onClose={jest.fn()} onSave={jest.fn()} />,
    );

    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const expected = [
      tomorrow.getFullYear(),
      ('0' + (tomorrow.getMonth() + 1)).slice(-2),
      ('0' + tomorrow.getDate()).slice(-2),
    ].join('-');

    expect(screen.getByText('New reminder')).toBeInTheDocument();
    expect(screen.getByTestId('title')).toHaveValue('');
    expect(screen.getByTestId('limitDate')).toHaveValue(expected);
    expect(screen.getByText('Create reminder')).toBeInTheDocument();
    expect(screen.queryByText('Delete reminder')).not.toBeInTheDocument();
  });

  it('prefills the draft in edit mode', () => {
    render(
      <ReminderSheet
        reminder={reminder}
        onClose={jest.fn()}
        onSave={jest.fn()}
        onDelete={jest.fn()}
      />,
    );

    expect(screen.getByText('Edit reminder')).toBeInTheDocument();
    expect(screen.getByTestId('title')).toHaveValue('Call the notary');
    expect(screen.getByTestId('description')).toHaveValue('Ask about the deed');
    expect(screen.getByTestId('limitDate')).toHaveValue('2026-01-22');
    expect(screen.getByText('Save changes')).toBeInTheDocument();
    expect(screen.getByText('Delete reminder')).toBeInTheDocument();
  });

  it('disables save while the title is blank', () => {
    const onSave = jest.fn();

    render(<ReminderSheet onClose={jest.fn()} onSave={onSave} />);

    const save = screen.getByTestId('save-button');
    expect(save).toBeDisabled();

    fireEvent.change(screen.getByTestId('title'), {
      target: { value: '   ' },
    });
    expect(save).toBeDisabled();

    fireEvent.change(screen.getByTestId('title'), {
      target: { value: 'Buy milk' },
    });
    expect(save).toBeEnabled();

    fireEvent.click(save);
    expect(onSave).toHaveBeenCalledWith(
      expect.objectContaining({ title: 'Buy milk', isDone: false }),
    );
  });

  it('toggles the status button', () => {
    const onSave = jest.fn();

    render(
      <ReminderSheet
        reminder={reminder}
        onClose={jest.fn()}
        onSave={onSave}
      />,
    );

    expect(screen.getByText('Not done yet')).toBeInTheDocument();

    fireEvent.click(screen.getByTestId('isDone'));
    expect(screen.getByText('Done')).toBeInTheDocument();

    fireEvent.click(screen.getByTestId('save-button'));
    expect(onSave).toHaveBeenCalledWith(
      expect.objectContaining({ id: '1', isDone: true, limitDate: '2026-01-22' }),
    );
  });

  it('closes on Escape, scrim click, Close and Cancel', () => {
    const onClose = jest.fn();

    render(<ReminderSheet onClose={onClose} onSave={jest.fn()} />);

    fireEvent.keyDown(document, { key: 'Escape' });
    fireEvent.click(screen.getByTestId('reminder-sheet-scrim'));
    fireEvent.click(screen.getByText('Close'));
    fireEvent.click(screen.getByText('Cancel'));

    expect(onClose).toHaveBeenCalledTimes(4);
  });

  it('keeps clicks inside the sheet from closing it', () => {
    const onClose = jest.fn();

    render(<ReminderSheet onClose={onClose} onSave={jest.fn()} />);

    fireEvent.click(screen.getByRole('dialog'));

    expect(onClose).not.toHaveBeenCalled();
  });

  it('shows a field error next to the field it belongs to', () => {
    render(
      <ReminderSheet
        errors={{
          title: ['The field is Required'],
          limitDate: ['The Limit Date should be later than Today.'],
        }}
        onClose={jest.fn()}
        onSave={jest.fn()}
      />,
    );

    expect(screen.getByTestId('title-error')).toHaveTextContent(
      'The field is Required',
    );
    expect(screen.getByTestId('limitDate-error')).toHaveTextContent(
      'The Limit Date should be later than Today.',
    );
  });

  it('shows an error with no field mapping in the banner', () => {
    render(
      <ReminderSheet
        errors={{ request: ['Something went wrong. Please try again.'] }}
        onClose={jest.fn()}
        onSave={jest.fn()}
      />,
    );

    expect(
      screen.getByText('Something went wrong. Please try again.'),
    ).toBeInTheDocument();
    expect(screen.queryByTestId('title-error')).not.toBeInTheDocument();
  });

  it('should focus the title field on open and return focus on close', () => {
    const trigger = document.createElement('button');
    document.body.appendChild(trigger);
    trigger.focus();

    const { unmount } = render(<ReminderSheet onClose={jest.fn} onSave={jest.fn} />);

    expect(screen.getByTestId('title')).toHaveFocus();

    unmount();

    expect(trigger).toHaveFocus();
  });

  it('should not steal focus back to a trigger that unmounted with it', () => {
    const trigger = document.createElement('button');
    document.body.appendChild(trigger);
    trigger.focus();

    const { unmount } = render(<ReminderSheet onClose={jest.fn} onSave={jest.fn} />);

    // The trigger goes away while the sheet is open, as it does when the
    // reminder is deleted from inside the sheet.
    trigger.remove();

    unmount();

    expect(document.activeElement).not.toBe(trigger);
  });
});
