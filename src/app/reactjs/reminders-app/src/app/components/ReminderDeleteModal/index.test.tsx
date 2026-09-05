import { render, screen, fireEvent } from '@testing-library/react';
import { ReminderDeleteModal } from '.';

describe('ReminderDeleteModal', () => {
  it('should render the delete modal', () => {
    render(
      <ReminderDeleteModal
        openDelete={true}
        toggleOpenDelete={jest.fn}
        onDelete={jest.fn}
      />,
    );

    const deleteModal = screen.getByTestId('delete-button');
    expect(deleteModal).toBeInTheDocument();
  });

  it('should call the delete function when delete button is clicked', () => {
    const deleteReminder = jest.fn();

    render(
      <ReminderDeleteModal
        openDelete={true}
        toggleOpenDelete={jest.fn}
        onDelete={deleteReminder}
      />,
    );

    const deleteButton = screen.getByTestId('delete-button');
    fireEvent.click(deleteButton);

    expect(deleteReminder).toHaveBeenCalled();
  });

  it('should call the toggle function when close button is clicked', () => {
    const toggleOpenDelete = jest.fn();

    render(
      <ReminderDeleteModal
        openDelete={true}
        toggleOpenDelete={toggleOpenDelete}
        onDelete={jest.fn}
      />,
    );

    const deleteButton = screen.getByTestId('close-button');
    fireEvent.click(deleteButton);

    expect(toggleOpenDelete).toHaveBeenCalled();
  });

  it('should quote the reminder title in the confirmation body', () => {
    render(
      <ReminderDeleteModal
        openDelete={true}
        reminderTitle="Call the notary"
        toggleOpenDelete={jest.fn}
        onDelete={jest.fn}
      />,
    );

    expect(screen.getByText('Delete this reminder?')).toBeInTheDocument();
    expect(
      screen.getByText('\u201CCall the notary\u201D will be removed permanently.'),
    ).toBeInTheDocument();
    expect(screen.getByText('Keep it')).toBeInTheDocument();
  });

  it('should render nothing when closed', () => {
    render(
      <ReminderDeleteModal
        openDelete={false}
        toggleOpenDelete={jest.fn}
        onDelete={jest.fn}
      />,
    );

    expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
  });

  it('should ignore Escape while closed so a toggle cannot reopen it', () => {
    const toggleOpenDelete = jest.fn();

    render(
      <ReminderDeleteModal
        openDelete={false}
        toggleOpenDelete={toggleOpenDelete}
        onDelete={jest.fn}
      />,
    );

    fireEvent.keyDown(document, { key: 'Escape' });

    expect(toggleOpenDelete).not.toHaveBeenCalled();
  });

  it('should close on Escape and on scrim click', () => {
    const toggleOpenDelete = jest.fn();

    render(
      <ReminderDeleteModal
        openDelete={true}
        toggleOpenDelete={toggleOpenDelete}
        onDelete={jest.fn}
      />,
    );

    fireEvent.keyDown(document, { key: 'Escape' });
    fireEvent.click(screen.getByTestId('delete-modal-scrim'));

    expect(toggleOpenDelete).toHaveBeenCalledTimes(2);
  });

  it('should focus Keep it on open and return focus to the trigger on close', () => {
    const trigger = document.createElement('button');
    document.body.appendChild(trigger);
    trigger.focus();

    const { rerender } = render(
      <ReminderDeleteModal
        openDelete={false}
        toggleOpenDelete={jest.fn}
        onDelete={jest.fn}
      />,
    );

    rerender(
      <ReminderDeleteModal
        openDelete={true}
        toggleOpenDelete={jest.fn}
        onDelete={jest.fn}
      />,
    );

    expect(screen.getByTestId('close-button')).toHaveFocus();

    rerender(
      <ReminderDeleteModal
        openDelete={false}
        toggleOpenDelete={jest.fn}
        onDelete={jest.fn}
      />,
    );

    expect(trigger).toHaveFocus();
  });
});
