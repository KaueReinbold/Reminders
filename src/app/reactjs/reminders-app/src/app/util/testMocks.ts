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
    useParams: jest.fn(() => ({ id: 'someId' })),
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
    useReminderActions: jest.fn().mockReturnValue({
      createReminder: jest.fn(),
      updateReminder: jest.fn(),
      deleteReminder: jest.fn(),
    }),
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

export { jestMocks, mockReminders, mockReminder };
