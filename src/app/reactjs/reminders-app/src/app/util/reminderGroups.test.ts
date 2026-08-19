import { Reminder } from '@/app/api/types';
import {
  bucketOf,
  dateLabel,
  dayDiff,
  groupReminders,
  isOverdue,
  limitDateOnly,
  viewCounts,
  weekProgress,
} from './reminderGroups';

const TODAY = new Date(2023, 0, 5);

const reminder = (overrides: Partial<Reminder>): Reminder => ({
  id: '1',
  title: 'Title',
  description: 'Description',
  limitDate: '2023-01-05T00:00:00',
  limitDateFormatted: '2023-01-05',
  isDone: false,
  ...overrides,
});

describe('dayDiff', () => {
  it('returns 0 for today, negative for past, positive for future', () => {
    expect(dayDiff('2023-01-05', TODAY)).toBe(0);
    expect(dayDiff('2023-01-02', TODAY)).toBe(-3);
    expect(dayDiff('2023-01-08', TODAY)).toBe(3);
  });
});

describe('dateLabel', () => {
  it('labels relative days', () => {
    expect(dateLabel('2023-01-05', TODAY)).toBe('Today');
    expect(dateLabel('2023-01-06', TODAY)).toBe('Tomorrow');
    expect(dateLabel('2023-01-04', TODAY)).toBe('Yesterday');
    expect(dateLabel('2023-01-01', TODAY)).toBe('4 days late');
  });

  it('labels far future dates as short date', () => {
    expect(dateLabel('2023-02-10', TODAY)).toBe('Feb 10');
  });
});

describe('limitDateOnly', () => {
  it('prefers limitDateFormatted and falls back to limitDate', () => {
    expect(limitDateOnly(reminder({}))).toBe('2023-01-05');
    expect(
      limitDateOnly(reminder({ limitDateFormatted: undefined })),
    ).toBe('2023-01-05');
  });
});

describe('bucketOf and isOverdue', () => {
  it('buckets done reminders as Done regardless of date', () => {
    const done = reminder({ isDone: true, limitDateFormatted: '2023-01-01' });
    expect(bucketOf(done, TODAY)).toBe('Done');
    expect(isOverdue(done, TODAY)).toBe(false);
  });

  it('buckets open reminders by date', () => {
    expect(bucketOf(reminder({ limitDateFormatted: '2023-01-01' }), TODAY)).toBe('Overdue');
    expect(bucketOf(reminder({ limitDateFormatted: '2023-01-05' }), TODAY)).toBe('Today');
    expect(bucketOf(reminder({ limitDateFormatted: '2023-01-09' }), TODAY)).toBe('Upcoming');
    expect(isOverdue(reminder({ limitDateFormatted: '2023-01-01' }), TODAY)).toBe(true);
  });
});

describe('groupReminders', () => {
  it('orders groups, hides empty ones, sorts items ascending by date', () => {
    const reminders = [
      reminder({ id: 'a', limitDateFormatted: '2023-01-09' }),
      reminder({ id: 'b', limitDateFormatted: '2023-01-02' }),
      reminder({ id: 'c', limitDateFormatted: '2023-01-01' }),
      reminder({ id: 'd', limitDateFormatted: '2023-01-06', isDone: true }),
    ];

    const groups = groupReminders(reminders, TODAY);

    expect(groups.map(group => group.label)).toEqual([
      'Overdue',
      'Upcoming',
      'Done',
    ]);
    expect(groups[0].items.map(item => item.id)).toEqual(['c', 'b']);
    expect(groups[1].items.map(item => item.id)).toEqual(['a']);
    expect(groups[2].items.map(item => item.id)).toEqual(['d']);
  });

  it('returns no groups for an empty list', () => {
    expect(groupReminders([], TODAY)).toEqual([]);
  });
});

describe('viewCounts', () => {
  it('counts reminders per view', () => {
    const reminders = [
      reminder({ id: 'a', limitDateFormatted: '2023-01-01' }),
      reminder({ id: 'b', limitDateFormatted: '2023-01-05' }),
      reminder({ id: 'c', limitDateFormatted: '2023-01-09' }),
      reminder({ id: 'd', limitDateFormatted: '2023-01-09', isDone: true }),
    ];

    expect(viewCounts(reminders, TODAY)).toEqual({
      All: 4,
      Today: 1,
      Upcoming: 1,
      Done: 1,
    });
  });
});

describe('weekProgress', () => {
  it('computes percent done of reminders due within 7 days', () => {
    const reminders = [
      reminder({ id: 'a', limitDateFormatted: '2023-01-06', isDone: true }),
      reminder({ id: 'b', limitDateFormatted: '2023-01-08', isDone: true }),
      reminder({ id: 'c', limitDateFormatted: '2023-01-10' }),
      // Beyond the 7-day window: excluded from the percentage.
      reminder({ id: 'd', limitDateFormatted: '2023-02-01' }),
    ];

    expect(weekProgress(reminders, TODAY)).toEqual({
      pct: 67,
      caption: '2 of 3 done in the next 7 days',
    });
  });

  it('surfaces overdue count in the caption', () => {
    const reminders = [
      reminder({ id: 'a', limitDateFormatted: '2023-01-01' }),
      reminder({ id: 'b', limitDateFormatted: '2023-01-06', isDone: true }),
    ];

    expect(weekProgress(reminders, TODAY)).toEqual({
      pct: 50,
      caption: '1 reminder is overdue',
    });
  });

  it('returns 0 percent for an empty week', () => {
    expect(weekProgress([], TODAY)).toEqual({
      pct: 0,
      caption: '0 of 0 done in the next 7 days',
    });
  });
});
