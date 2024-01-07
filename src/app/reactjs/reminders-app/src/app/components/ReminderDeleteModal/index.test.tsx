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
});
