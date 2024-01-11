import { ReminderActionStatus } from '../hooks';

const mockReminder = {
  id: 1,
  title: 'Test Title 1',
  description: 'Test Description 1',
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

const jestMocks = {
  'next/navigation': () => ({
    useRouter: jest.fn().mockReturnValue({
      push: jest.fn(),
    }),
    useParams: jest.fn().mockImplementation(() => ({ id: 'someId' })),
  }),
  '@/app/api': () => ({
    useReminders: jest.fn().mockImplementation(() => ({
      data: mockReminders,
    })),
    useReminder: jest.fn().mockImplementation(() => ({
      data: mockReminder,
    })),
    createReminder: jest.fn(),
    deleteReminder: jest.fn(),
    getReminder: jest.fn(),
    getReminders: jest.fn(),
    updateReminder: jest.fn(),
    useReminderActions: jest.fn().mockImplementation(() => ({
      createReminder: { mutateAsync: jest.fn() },
      updateReminder: { mutateAsync: jest.fn() },
      deleteReminder: { mutateAsync: jest.fn() },
    })),
  }),
  '@/app/hooks': () => ({
    useRemindersClearContext: jest.fn().mockReturnValue(jest.fn),
    useRemindersContext: jest.fn().mockReturnValue({
      reminder: mockReminder,
      errors: {},
      dispatch: jest.fn(),
      onCreateReminder: jest
        .fn()
        .mockResolvedValue(ReminderActionStatus.Success),
      onUpdateReminder: jest
        .fn()
        .mockResolvedValue(ReminderActionStatus.Success),
      onDeleteReminder: jest
        .fn()
        .mockResolvedValue(ReminderActionStatus.Success),
    }),
    useMutation: jest.fn(),
    ReminderActionStatus: ReminderActionStatus,
  }),
};

const jestRemindersMocks = {
  'next/navigation': {
    useRouter: jest.fn().mockReturnValue({
      push: jest.fn(),
    }),
    useParams: jest.fn().mockImplementation(() => ({ id: mockReminder.id })),
  },
  '@/app/api': {
    useReminders: jest.fn().mockImplementation(() => ({
      data: mockReminders,
    })),
    useReminder: jest.fn().mockImplementation(() => ({
      data: mockReminder,
    })),
    createReminder: jest.fn(),
    deleteReminder: jest.fn(),
    getReminder: jest.fn(),
    getReminders: jest.fn(),
    updateReminder: jest.fn(),
    useReminderActions: jest.fn().mockReturnValue({
      createReminder: { mutateAsync: jest.fn() },
      updateReminder: { mutateAsync: jest.fn() },
      deleteReminder: { mutateAsync: jest.fn() },
    }),
    ValidationError: jest.fn().mockImplementation(() => ({
      errors: {
        Title: [
          "The field Title must be a text with a maximum length of '50'.",
        ],
        Description: [
          "The field Description must be a text with a maximum length of '200'.",
        ],
        'LimitDate.Date': ['The Limit Date should be later than Today.'],
      },
    })),
  },
  '@/app/hooks': {
    useRemindersClearContext: jest.fn().mockReturnValue(jest.fn),
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
    ReminderActionStatus: ReminderActionStatus,
  },
};

export { jestMocks, jestRemindersMocks, mockReminders, mockReminder };
