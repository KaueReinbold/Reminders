import { act, fireEvent, render, screen } from '@testing-library/react';
import {
  ReminderActionStatus,
  RemindersContextProvider,
  useRemindersContext,
} from '.';
import { ReactNode, useState } from 'react';
import { mockReminder } from '@/app/util/testMocks';
import { ValidationError, useReminderActions } from '@/app/api';

jest.mock(
  'next/navigation',
  () => require('@/app/util/testMocks').jestRemindersMocks['next/navigation'],
);
jest.mock(
  '@/app/api',
  () => require('@/app/util/testMocks').jestRemindersMocks['@/app/api'],
);

const TestingComponent = () => {
  const { errors, onCreateReminder, onUpdateReminder, onDeleteReminder } =
    useRemindersContext();
  const [result, setResult] = useState(ReminderActionStatus.Unknown);

  const handleCreateClick = async () => {
    const onCreateReminderResult = await onCreateReminder();
    setResult(await onCreateReminder());
  };

  const handleUpdateClick = async () => {
    const onUpdateReminderResult = await onUpdateReminder();
    setResult(await onUpdateReminder());
  };

  const handleDeleteClick = async () => {
    setResult(await onDeleteReminder());
  };

  return (
    <>
      <button onClick={handleCreateClick}>Create</button>
      <button onClick={handleUpdateClick}>Update</button>
      <button onClick={handleDeleteClick}>Delete</button>
      <span>{result}</span>
      <span>{errors?.['LimitDate.Date'][0]}</span>
      <span>{errors?.Description[0]}</span>
      <span>{errors?.Title[0]}</span>
      <span>{errors?.ServerError[0]}</span>
    </>
  );
};

describe('RemindersContextProvider Tests', () => {
  async function renderProvider(children: ReactNode) {
    await act(async () => {
      render(<RemindersContextProvider>{children}</RemindersContextProvider>);
    });
  }

  afterEach(() => {
    jest.restoreAllMocks();
  });

  it('should update context values on successful createReminder', async () => {
    await renderProvider(<TestingComponent />);

    await act(async () => {
      fireEvent.click(screen.getByText('Create'));
    });

    const { createReminder } = useReminderActions();

    expect(createReminder.mutateAsync).toHaveBeenCalledWith(mockReminder);

    expect(screen.getByText(ReminderActionStatus.Success)).toBeInTheDocument();
  });

  it.skip('should handle errors on createReminder', async () => {
    const { createReminder } = useReminderActions();
    const error = new ValidationError('', {
      Title: ["The field Title must be a text with a maximum length of '50'."],
      Description: [
        "The field Description must be a text with a maximum length of '200'.",
      ],
      'LimitDate.Date': ['The Limit Date should be later than Today.'],
    } as any);

    jest.spyOn(createReminder, 'mutateAsync').mockRejectedValue(error);

    await renderProvider(<TestingComponent />);

    await act(async () => {
      fireEvent.click(screen.getByText('Create'));
    });

    expect(screen.getByText(ReminderActionStatus.Fail)).toBeInTheDocument();

    act(() => {
      expect(
        screen.getByText(error.errors['LimitDate.Date'][0]),
      ).toBeInTheDocument();
      expect(screen.getByText(error.errors.Title[0])).toBeInTheDocument();
      expect(screen.getByText(error.errors.Description[0])).toBeInTheDocument();
    });
  });

  it('should update context values on successful updateReminder', async () => {
    await renderProvider(<TestingComponent />);

    await act(async () => {
      fireEvent.click(screen.getByText('Update'));
    });

    const { updateReminder } = useReminderActions();

    expect(updateReminder.mutateAsync).toHaveBeenCalledWith(mockReminder);

    expect(screen.getByText(ReminderActionStatus.Success)).toBeInTheDocument();
  });

  it.todo('should handle errors on updateReminder');

  it('should handle errors on deleteReminder', async () => {
    await renderProvider(<TestingComponent />);

    await act(async () => {
      fireEvent.click(screen.getByText('Delete'));
    });

    const { deleteReminder } = useReminderActions();

    expect(deleteReminder.mutateAsync).toHaveBeenCalledWith(mockReminder.id);

    expect(screen.getByText(ReminderActionStatus.Success)).toBeInTheDocument();
  });
});
