import {
  act,
  fireEvent,
  render,
  renderHook,
  screen,
  waitFor,
} from '@testing-library/react';
import {
  ReminderActionStatus,
  RemindersContextProvider,
  useRemindersClearContext,
  useRemindersContext,
} from '.';
import { ReactNode, useState } from 'react';
import { mockReminder } from '@/app/util/testMocks';
import { useReminderActions } from '@/app/api';
import React from 'react';

jest.mock(
  'next/navigation',
  () => require('@/app/util/testMocks').jestObjectsMock['next/navigation'],
);
jest.mock(
  '@/app/api',
  () => require('@/app/util/testMocks').jestObjectsMock['@/app/api'],
);

const TestingComponent = ({ title }: { title?: string }) => {
  const {
    reminder,
    errors,
    onCreateReminder,
    onUpdateReminder,
    onDeleteReminder,
    clearReminder,
    dispatch,
  } = useRemindersContext();
  const clearReminderFromClearContext = useRemindersClearContext();
  const [result, setResult] = useState(ReminderActionStatus.Unknown);

  const handleCreateClick = async () => {
    setResult(await onCreateReminder());
  };

  const handleUpdateClick = async () => {
    setResult(await onUpdateReminder());
  };

  const handleDeleteClick = async () => {
    setResult(await onDeleteReminder());
  };

  const handleClearReminder = async () => {
    clearReminder();
  };

  const handleUpdateDispatch = async () => {
    dispatch({
      type: 'UPDATE_REMINDER',
      payload: { ...reminder, title },
    });
  };

  const handleTypeWrongType = async () => {
    dispatch({
      type: 'WRONG_TYPE!_',
    } as any);
  };

  const mapError = (error?: string[]) => error && error[0];

  return (
    <>
      <button onClick={handleCreateClick}>Create</button>
      <button onClick={handleUpdateClick}>Update</button>
      <button onClick={handleDeleteClick}>Delete</button>
      <button onClick={handleClearReminder}>Clear</button>
      <button onClick={handleUpdateDispatch}>Update Dispatch</button>
      <button onClick={handleTypeWrongType}>Wrong Type</button>
      <button onClick={clearReminderFromClearContext}>
        Clear from Clear Context
      </button>
      <span>{result}</span>
      <span>{mapError(errors?.['LimitDate.Date'])}</span>
      <span>{mapError(errors?.Description)}</span>
      <span>{mapError(errors?.Title)}</span>
      <span>{errors?.InternalServer}</span>
      <span>{errors?.BadRequest}</span>
      <span>{reminder?.title}</span>
    </>
  );
};

describe('RemindersContextProvider Tests', () => {
  const mockErrors = {
    Title: ["The field Title must be a text with a maximum length of '50'."],
    Description: [
      "The field Description must be a text with a maximum length of '200'.",
    ],
    'LimitDate.Date': ['The Limit Date should be later than Today.'],
  };

  async function renderProvider(children: ReactNode) {
    await act(async () => {
      render(<RemindersContextProvider>{children}</RemindersContextProvider>);
    });
  }

  afterEach(() => {
    jest.restoreAllMocks();
  });

  describe('createReminder', () => {
    it('should update context values on successful createReminder', async () => {
      await renderProvider(<TestingComponent />);

      await act(async () => {
        fireEvent.click(screen.getByText('Create'));
      });

      const { createReminder } = useReminderActions();

      expect(createReminder.mutateAsync).toHaveBeenCalledWith(mockReminder);

      expect(
        screen.getByText(ReminderActionStatus.Success),
      ).toBeInTheDocument();
    });

    it('should handle exception on createReminder', async () => {
      const consoleSpy = jest
        .spyOn(console, 'error')
        .mockImplementation(jest.fn);
      const errorMessage = 'Error';

      const { createReminder } = useReminderActions();

      jest.spyOn(createReminder, 'mutateAsync').mockRejectedValue(errorMessage);

      await renderProvider(<TestingComponent />);

      await act(async () => {
        fireEvent.click(screen.getByText('Create'));
      });

      expect(screen.getByText(ReminderActionStatus.Fail)).toBeInTheDocument();
      expect(consoleSpy).toHaveBeenCalledWith(errorMessage);
    });

    it('should handle errors on updateReminder', async () => {
      const { createReminder } = useReminderActions();

      jest.spyOn(createReminder, 'mutateAsync').mockResolvedValue({
        errors: mockErrors,
      });

      await renderProvider(<TestingComponent />);

      await act(async () => {
        fireEvent.click(screen.getByText('Create'));
      });

      expect(screen.getByText(ReminderActionStatus.Fail)).toBeInTheDocument();
      expect(screen.getByText(mockErrors.Title[0])).toBeInTheDocument();
      expect(screen.getByText(mockErrors.Description[0])).toBeInTheDocument();
      expect(
        screen.getByText(mockErrors['LimitDate.Date'][0]),
      ).toBeInTheDocument();
    });
  });

  describe('updateReminder', () => {
    it('should update context values on successful updateReminder', async () => {
      await renderProvider(<TestingComponent />);

      await act(async () => {
        fireEvent.click(screen.getByText('Update'));
      });

      const { updateReminder } = useReminderActions();

      expect(updateReminder.mutateAsync).toHaveBeenCalledWith(mockReminder);

      expect(
        screen.getByText(ReminderActionStatus.Success),
      ).toBeInTheDocument();
    });

    it('should handle exception on updateReminder', async () => {
      const consoleSpy = jest
        .spyOn(console, 'error')
        .mockImplementation(jest.fn);
      const errorMessage = 'Error';

      const { updateReminder } = useReminderActions();

      jest.spyOn(updateReminder, 'mutateAsync').mockRejectedValue(errorMessage);

      await renderProvider(<TestingComponent />);

      await act(async () => {
        fireEvent.click(screen.getByText('Update'));
      });

      expect(screen.getByText(ReminderActionStatus.Fail)).toBeInTheDocument();
      expect(consoleSpy).toHaveBeenCalledWith(errorMessage);
    });

    it('should handle errors on updateReminder', async () => {
      const { updateReminder } = useReminderActions();

      jest.spyOn(updateReminder, 'mutateAsync').mockResolvedValue({
        errors: mockErrors,
      });

      await renderProvider(<TestingComponent />);

      await act(async () => {
        fireEvent.click(screen.getByText('Update'));
      });

      expect(screen.getByText(ReminderActionStatus.Fail)).toBeInTheDocument();
      expect(screen.getByText(mockErrors.Title[0])).toBeInTheDocument();
      expect(screen.getByText(mockErrors.Description[0])).toBeInTheDocument();
      expect(
        screen.getByText(mockErrors['LimitDate.Date'][0]),
      ).toBeInTheDocument();
    });
  });

  describe('deleteReminder', () => {
    it('should update context values on successful on deleteReminder', async () => {
      await renderProvider(<TestingComponent />);

      await act(async () => {
        fireEvent.click(screen.getByText('Delete'));
      });

      const { deleteReminder } = useReminderActions();

      expect(deleteReminder.mutateAsync).toHaveBeenCalledWith(mockReminder.id);

      expect(
        screen.getByText(ReminderActionStatus.Success),
      ).toBeInTheDocument();
    });

    it('should handle exception on deleteReminder', async () => {
      const consoleSpy = jest
        .spyOn(console, 'error')
        .mockImplementation(jest.fn);
      const errorMessage = 'Error';

      const { deleteReminder } = useReminderActions();

      jest.spyOn(deleteReminder, 'mutateAsync').mockRejectedValue(errorMessage);

      await renderProvider(<TestingComponent />);

      await act(async () => {
        fireEvent.click(screen.getByText('Delete'));
      });

      expect(screen.getByText(ReminderActionStatus.Fail)).toBeInTheDocument();
      expect(consoleSpy).toHaveBeenCalledWith(errorMessage);
    });

    it('should handle errors on deleteReminder', async () => {
      const { deleteReminder } = useReminderActions();
      const deleteError = {
        BadRequest: 'Error',
      };

      jest.spyOn(deleteReminder, 'mutateAsync').mockResolvedValue({
        errors: deleteError,
      });

      await renderProvider(<TestingComponent />);

      await act(async () => {
        fireEvent.click(screen.getByText('Delete'));
      });

      expect(screen.getByText(ReminderActionStatus.Fail)).toBeInTheDocument();
      expect(screen.getByText(deleteError.BadRequest)).toBeInTheDocument();
    });
  });

  describe('clearReminder', () => {
    it('should call clearReminder from Default Context', async () => {
      await renderProvider(<TestingComponent />);

      await waitFor(() => {
        expect(screen.getByText(mockReminder.title)).toBeInTheDocument();
      });

      fireEvent.click(screen.getByText('Clear'));

      await waitFor(() => {
        expect(screen.queryByText(mockReminder.title)).not.toBeInTheDocument();
      });
    });

    it('should call clearReminder from Clear Context', async () => {
      await renderProvider(<TestingComponent />);

      fireEvent.click(screen.getByText('Clear from Clear Context'));
    });
  });

  describe('dispatch', () => {
    it('should call type UPDATE_REMINDER', async () => {
      const title = 'Update by Test';

      await renderProvider(<TestingComponent title={title} />);

      await waitFor(() => {
        expect(screen.getByText(mockReminder.title)).toBeInTheDocument();
      });

      fireEvent.click(screen.getByText('Update Dispatch'));

      await waitFor(() => {
        expect(screen.getByText(title)).toBeInTheDocument();
      });
    });

    it('should throw error when call type not found', async () => {
      jest.spyOn(console, 'error').mockImplementation(jest.fn);

      await renderProvider(<TestingComponent />);

      expect(() => fireEvent.click(screen.getByText('Wrong Type'))).toThrow();
    });
  });

  describe('use Contexts', () => {
    beforeEach(() => {
      jest
        .spyOn(require('use-context-selector'), 'useContextSelector')
        .mockImplementation(() => null);
    });

    it('should useRemindersContext throw error when context not within provider', async () => {
      expect(() => {
        useRemindersContext();
      }).toThrow(
        'useRemindersContext must be used within a RemindersContextProvider',
      );
    });

    it('should useRemindersClearContext throw error when context not within provider', async () => {
      expect(() => {
        useRemindersClearContext();
      }).toThrow(
        'useRemindersClearContext must be used within a RemindersContextProvider',
      );
    });
  });
});
