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
  }),
  '@/app/api': () => ({
    useReminders: jest.fn().mockImplementation(() => ({
      data: mockReminders,
    })),
  }),
  '@/app/hooks': () => ({
    useRemindersClearContext: jest.fn().mockReturnValue(jest.fn),
    useRemindersContext: jest.fn().mockReturnValue({
      reminder: mockReminder,
      errors: {},
      dispatch: jest.fn(),
      onCreateReminder: jest.fn(),
      onUpdateReminder: jest.fn(),
      onDeleteReminder: jest.fn(),
    }),
  }),
};

export { jestMocks, mockReminders, mockReminder };
