import { act, renderHook } from '@testing-library/react';
import {
  useCreateReminder,
  useDeleteReminder,
  useReminder,
  useReminders,
  useUpdateReminder,
} from '.';

jest.mock(
  '@/app/hooks',
  require('@/app/util/testMocks').jestMocks['@/app/hooks'],
);

describe('Reminder Hooks', () => {
  beforeEach(() => {
    jest.restoreAllMocks();
  });

  it.skip('useReminders hook', () => {
    const { result } = renderHook(() => useReminders());

    expect(result.current.data).toEqual([{ id: '1', title: 'Reminder 1' }]);
    expect(result.current.isLoading).toBe(false);
    expect(result.current.isError).toBe(false);
  });

  it.skip('useReminder hook', () => {
    const { result } = renderHook(() => useReminder('1'));

    expect(result.current.data).toEqual({ id: '1', title: 'Reminder 1' });
    expect(result.current.isLoading).toBe(false);
    expect(result.current.isError).toBe(false);
  });

  it.skip('useCreateReminder hook', () => {
    const { result } = renderHook(() => useCreateReminder());

    act(() => {
      // result.current.mutate();
    });
    expect(result.current.mutate).toHaveBeenCalled();
  });

  it.skip('useUpdateReminder hook', () => {
    const { result } = renderHook(() => useUpdateReminder());

    act(() => {
      // result.current.mutate();
    });
    expect(result.current.mutate).toHaveBeenCalled();
  });

  it.skip('useDeleteReminder hook', () => {
    const { result } = renderHook(() => useDeleteReminder());

    act(() => {
      // result.current.mutate();
    });
    expect(result.current.mutate).toHaveBeenCalled();
  });
});
