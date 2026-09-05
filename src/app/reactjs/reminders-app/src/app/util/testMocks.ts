import { ReminderActionStatus } from '../hooks';

const mockReminder = {
  id: 1,
  title: 'Test Title 1',
  description: 'Test Description 1',
  limitDate: '',
  limitDateFormatted: '2023-01-01',
  isDone: true,
  isDoneFormatted: 'Yes',
};

const mockReminders = [
  { ...mockReminder },
  {
    id: 2,
    title: 'Test Title 2',
    description: 'Test Description 2',
    limitDateFormatted: '2023-01-02',
    isDone: false,
    isDoneFormatted: 'No',
  },
];

const mockUpdateMutateAsync = jest.fn().mockResolvedValue({});

const mockCreateMutateAsync = jest.fn().mockResolvedValue({});

const mockDeleteMutateAsync = jest.fn().mockResolvedValue({});

const mockQueryClient = {
  getQueryData: jest.fn().mockReturnValue(mockReminders),
  setQueryData: jest.fn(),
  invalidateQueries: jest.fn(),
};

const jestObjectsMock = {
  'next/navigation': {
    useRouter: jest.fn().mockReturnValue({
      push: jest.fn(),
    }),
    useSearchParams: jest.fn().mockImplementation(
      () => new URLSearchParams({ id: String(mockReminder.id) }),
    ),
  },
  '@/app/api': {
    useReminders: jest.fn().mockImplementation(() => ({
      data: mockReminders,
    })),
    useReminder: jest.fn().mockImplementation(() => ({
      data: mockReminder,
    })),
    REMINDERS_QUERY_KEY: ['reminders'],
    useUpdateReminder: jest.fn().mockReturnValue({
      mutateAsync: mockUpdateMutateAsync,
    }),
    useCreateReminder: jest.fn().mockReturnValue({
      mutateAsync: mockCreateMutateAsync,
    }),
    useDeleteReminder: jest.fn().mockReturnValue({
      mutateAsync: mockDeleteMutateAsync,
    }),
    createReminder: jest.fn(),
    deleteReminder: jest.fn(),
    updateReminder: jest.fn(),
    useReminderActions: jest.fn().mockReturnValue({
      createReminder: { mutateAsync: jest.fn() },
      updateReminder: { mutateAsync: jest.fn() },
      deleteReminder: { mutateAsync: jest.fn() },
    }),
  },
  '@/app/hooks': {
    useRemindersClearContext: jest.fn().mockReturnValue(jest.fn),
    useRemindersQueryClient: jest.fn().mockReturnValue(mockQueryClient),
    useRemindersContext: jest.fn().mockReturnValue({
      reminder: mockReminder,
      errors: {},
      dispatch: jest.fn(),
      onCreateReminder: jest
        .fn()
        .mockImplementation(() => ReminderActionStatus.Success),
      onUpdateReminder: jest
        .fn()
        .mockImplementation(() => ReminderActionStatus.Success),
      onDeleteReminder: jest
        .fn()
        .mockImplementation(() => ReminderActionStatus.Success),
    }),
    useMutation: jest.fn(),
    useQuery: jest.fn(),
    ReminderActionStatus: ReminderActionStatus,
  },
};

const jestFunctionsMock = {
  'next/navigation': () => jestObjectsMock['next/navigation'],
  '@/app/api': () => jestObjectsMock['@/app/api'],
  '@/app/hooks': () => jestObjectsMock['@/app/hooks'],
};

export {
  jestFunctionsMock,
  jestObjectsMock,
  mockReminders,
  mockReminder,
  mockQueryClient,
  mockUpdateMutateAsync,
  mockCreateMutateAsync,
  mockDeleteMutateAsync,
};
