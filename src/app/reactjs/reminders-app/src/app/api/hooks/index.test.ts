import { act, renderHook } from '@testing-library/react';
import {
  useCreateReminder,
  useDeleteReminder,
  useReminder,
  useReminderActions,
  useReminders,
  useUpdateReminder,
} from '.';
import { mockReminder, mockReminders } from '@/app/util/testMocks';

jest.mock(
  '@/app/api',
  () => require('@/app/util/testMocks').jestFunctionsMock['@/app/api'],
);
jest.mock('@/app/hooks', () => ({
  useMutation: jest.fn(),
  useQuery: jest.fn().mockImplementation(() => mockReminders),
}));

describe('Reminder Hooks', () => {
  beforeEach(() => {
    jest.restoreAllMocks();
  });

  it('useReminders hook', () => {
    const { result } = renderHook(() => useReminders());

    expect(result.current).toEqual(mockReminders);
  });

  it('useReminder hook', () => {
    jest
      .spyOn(require('@/app/hooks'), 'useQuery')
      .mockImplementation(() => mockReminder);

    const { result } = renderHook(() => useReminder('1'));

    expect(result.current).toEqual(mockReminder);
  });

  it('useCreateReminder hook', () => {
    jest
      .spyOn(require('@/app/hooks'), 'useMutation')
      .mockImplementation(() => ({ mutateAsync: jest.fn() }));

    const { result } = renderHook(() => useCreateReminder());

    act(() => {
      (result.current as any).mutateAsync();
    });
    expect(result.current.mutateAsync).toHaveBeenCalled();
  });

  it('useUpdateReminder hook', () => {
    jest
      .spyOn(require('@/app/hooks'), 'useMutation')
      .mockImplementation(() => ({ mutateAsync: jest.fn() }));

    const { result } = renderHook(() => useUpdateReminder());

    act(() => {
      (result.current as any).mutateAsync();
    });
    expect(result.current.mutateAsync).toHaveBeenCalled();
  });

  it('useDeleteReminder hook', () => {
    jest
      .spyOn(require('@/app/hooks'), 'useMutation')
      .mockImplementation(() => ({ mutateAsync: jest.fn() }));

    const { result } = renderHook(() => useDeleteReminder());

    act(() => {
      (result.current as any).mutateAsync();
    });
    expect(result.current.mutateAsync).toHaveBeenCalled();
  });

  it('useReminderActions hook', () => {
    jest
      .spyOn(require('@/app/hooks'), 'useMutation')
      .mockImplementation(() => ({ mutateAsync: jest.fn() }));

    const { result } = renderHook(() => useReminderActions());

    act(() => {
      (result.current as any).createReminder.mutateAsync();
      (result.current as any).updateReminder.mutateAsync();
      (result.current as any).deleteReminder.mutateAsync();
    });
    expect(result.current.createReminder.mutateAsync).toHaveBeenCalled();
    expect(result.current.updateReminder.mutateAsync).toHaveBeenCalled();
    expect(result.current.deleteReminder.mutateAsync).toHaveBeenCalled();
  });
});
