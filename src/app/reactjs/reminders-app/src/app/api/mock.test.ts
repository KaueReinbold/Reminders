import {
  createReminder,
  deleteReminder,
  getReminder,
  getReminders,
  resetReminders,
  updateReminder,
} from './mock';

describe('mock API', () => {
  const valid = {
    title: 'Water the plants',
    description: 'The ones on the balcony',
    limitDate: '2030-01-01',
    isDone: false,
  };

  beforeEach(() => {
    resetReminders();
  });

  it('seeds reminders with formatted fields', async () => {
    const reminders = await getReminders();

    expect(reminders).toHaveLength(5);
    expect(reminders[0].limitDateFormatted).toMatch(/^\d{4}-\d{2}-\d{2}$/);
    expect(reminders[0].isDoneFormatted).toBe('No');
  });

  it('returns a single reminder by id', async () => {
    expect((await getReminder('2')).id).toBe('2');
  });

  it('returns an empty reminder for an unknown id', async () => {
    expect(await getReminder('nope')).toEqual({});
  });

  it('creates a reminder with a new id', async () => {
    const { result, errors } = await createReminder(valid);

    expect(errors).toBeUndefined();
    expect(result?.id).toBe('6');
    expect(result?.isDoneFormatted).toBe('No');
    expect(await getReminders()).toHaveLength(6);
  });

  it('rejects an invalid reminder without storing it', async () => {
    const { result, errors } = await createReminder({
      ...valid,
      title: '',
      description: '',
    });

    expect(result).toBeUndefined();
    expect(errors?.Title).toEqual(['The field is Required']);
    expect(errors?.Description).toEqual(['The field is Required']);
    expect(await getReminders()).toHaveLength(5);
  });

  it('updates an existing reminder', async () => {
    const { result } = await updateReminder({ ...valid, id: '1', isDone: true });

    expect(result?.title).toBe('Water the plants');
    expect(result?.isDoneFormatted).toBe('Yes');
    expect((await getReminder('1')).title).toBe('Water the plants');
  });

  it('reports an update for an unknown reminder', async () => {
    const { errors } = await updateReminder({ ...valid, id: 'nope' });

    expect(errors?.BadRequest).toContain('nope');
  });

  it('deletes a reminder', async () => {
    expect(await deleteReminder('1')).toEqual({ result: '1' });
    expect(await getReminders()).toHaveLength(4);
  });

  it('reports a delete for an unknown reminder', async () => {
    const { errors } = await deleteReminder('nope');

    expect(errors?.BadRequest).toContain('nope');
  });

  it('starts from the seed data again after a reset', async () => {
    await deleteReminder('1');
    resetReminders();

    expect(await getReminders()).toHaveLength(5);
  });
});
