import {
  getReminders,
  getReminder,
  createReminder,
  updateReminder,
  deleteReminder,
  Reminder,
  API_BASE_URL,
  Errors,
  getErrors,
} from './index';

describe('API functions', () => {
  const mockValidationFail = {
    type: 'https://tools.ietf.org/html/rfc9110#section-15.5.1',
    title: 'One or more validation errors occurred.',
    status: 400,
    errors: {
      Title: ["The field Title must be a text with a maximum length of '50'."],
      Description: [
        "The field Description must be a text with a maximum length of '200'.",
      ],
      'LimitDate.Date': ['The Limit Date should be later than Today.'],
    },
    traceId: '00-852bcb95dfb240075f3f8442bb60e22d-c4a9101dd7a52296-00',
  };

  const mockReminder: Reminder = {
    title: 'Test Reminder',
    description: 'This is a test reminder',
    limitDate: '2022-01-01',
    limitDateFormatted: '2021-12-31',
    isDone: false,
    isDoneFormatted: 'No',
  };

  beforeEach(() => {
    // Mock the fetch function
    global.fetch = jest.fn();
  });

  afterEach(() => {
    jest.resetAllMocks();
  });

  describe('getReminders', () => {
    it('should fetch reminders from the API', async () => {
      const mockResponse = {
        json: jest.fn().mockResolvedValueOnce([{ id: '1', ...mockReminder }]),
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      const reminders = await getReminders();

      expect(global.fetch).toHaveBeenCalledWith(
        `${API_BASE_URL}/api/reminders`,
      );
      expect(mockResponse.json).toHaveBeenCalled();
      expect(reminders).toEqual([{ id: '1', ...mockReminder }]);
    });
  });

  describe('getReminder', () => {
    it('should fetch a single reminder from the API', async () => {
      const mockResponse = {
        json: jest.fn().mockResolvedValueOnce({ id: '1', ...mockReminder }),
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      const reminder = await getReminder('1');

      expect(global.fetch).toHaveBeenCalledWith(
        `${API_BASE_URL}/api/reminders/1`,
      );
      expect(mockResponse.json).toHaveBeenCalled();
      expect(reminder).toEqual({ id: '1', ...mockReminder });
    });
  });

  describe('createReminder', () => {
    it('should create a new reminder', async () => {
      const mockResponse = {
        json: jest.fn().mockResolvedValueOnce({ id: '1', ...mockReminder }),
        ok: true,
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      const createdReminder = await createReminder(mockReminder);

      expect(global.fetch).toHaveBeenCalledWith(
        `${API_BASE_URL}/api/reminders`,
        {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(mockReminder),
        },
      );
      expect(mockResponse.json).toHaveBeenCalled();
      expect(createdReminder.result).toEqual({ id: '1', ...mockReminder });
    });

    it('should return validation error when trying to create a new reminder', async () => {
      const mockResponse = {
        json: jest.fn().mockResolvedValueOnce(mockValidationFail),
        ok: false,
        status: 400,
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      expect((await createReminder(mockReminder)).errors).toStrictEqual({
        Title: mockValidationFail.errors['Title'],
        Description: mockValidationFail.errors['Description'],
        'LimitDate.Date': mockValidationFail.errors['LimitDate.Date'],
      } as Errors);
    });
  });

  describe('updateReminder', () => {
    it('should update an existing reminder', async () => {
      const updatedReminder = { id: '1', ...mockReminder };
      const mockResponse = {
        json: jest.fn().mockResolvedValueOnce(updatedReminder),
        ok: true,
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      const reminder = await updateReminder(updatedReminder);

      expect(global.fetch).toHaveBeenCalledWith(
        `${API_BASE_URL}/api/reminders/1`,
        {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(updatedReminder),
        },
      );
      expect(mockResponse.json).toHaveBeenCalled();
      expect(reminder.result).toEqual(updatedReminder);
    });

    it('should return validation error when trying to update a new reminder', async () => {
      const mockResponse = {
        json: jest.fn().mockResolvedValueOnce(mockValidationFail),
        ok: false,
        status: 400,
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      expect((await updateReminder(mockReminder)).errors).toStrictEqual({
        Title: mockValidationFail.errors['Title'],
        Description: mockValidationFail.errors['Description'],
        'LimitDate.Date': mockValidationFail.errors['LimitDate.Date'],
      } as any);
    });
  });

  describe('deleteReminder', () => {
    it('should delete an existing reminder', async () => {
      const mockResponse = {
        ok: true,
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      const result = await deleteReminder('1');

      expect(global.fetch).toHaveBeenCalledWith(
        `${API_BASE_URL}/api/reminders/1`,
        {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
          },
        },
      );
      expect(result.result).toEqual('1');
    });

    it('should throw error if delete failed', async () => {
      const mockResponse = {
        json: jest
          .fn()
          .mockResolvedValueOnce({ ...mockValidationFail, errors: [] }),
        ok: false,
        status: 400,
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      expect((await deleteReminder('1')).errors).toStrictEqual({
        BadRequest: mockValidationFail.title,
      } as Errors);
    });
  });

  describe('mapReminder', () => {
    it('should limitDateFormatted be empty when no limit date is provided', async () => {
      const copy = {
        ...mockReminder,
        limitDate: null,
        limitDateFormatted: '',
      };
      const mockResponse = {
        json: jest.fn().mockResolvedValueOnce({ id: '1', ...copy }),
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      const reminder = await getReminder('1');

      expect(reminder).toEqual({ id: '1', ...copy });
    });

    it('should isDoneFormatted be Yes when isDone is true', async () => {
      const copy = {
        ...mockReminder,
        isDone: true,
        isDoneFormatted: 'Yes',
      };
      const mockResponse = {
        json: jest.fn().mockResolvedValueOnce({ id: '1', ...copy }),
      };
      (global.fetch as jest.Mock).mockResolvedValueOnce(mockResponse);

      const reminder = await getReminder('1');

      expect(reminder).toEqual({ id: '1', ...copy });
    });
  });

  describe('getErrors', () => {
    it('should return internal error when exception occurred', async () => {
      const consoleSpy = jest
        .spyOn(console, 'error')
        .mockImplementationOnce(jest.fn);
      const errorMessage = 'Error';
      const mockResponse = {
        json: jest.fn().mockRejectedValue(errorMessage),
      } as any;

      expect(await getErrors(mockResponse)).toStrictEqual({
        InternalServer: 'Failed to perform errors validation',
      } as Errors);
      expect(consoleSpy).toHaveBeenCalledWith(errorMessage);
    });
  });
});
