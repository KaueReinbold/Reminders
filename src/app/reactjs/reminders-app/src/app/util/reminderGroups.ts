import { Reminder } from '@/app/api/types';

export type GroupName = 'Overdue' | 'Today' | 'Upcoming' | 'Done';

export type ViewName = 'All' | 'Today' | 'Upcoming' | 'Done';

export const VIEW_ORDER: ViewName[] = ['All', 'Today', 'Upcoming', 'Done'];

export type WeekProgress = {
  pct: number;
  caption: string;
};

export type ReminderGroup = {
  label: GroupName;
  items: Reminder[];
};

const GROUP_ORDER: GroupName[] = ['Overdue', 'Today', 'Upcoming', 'Done'];

const MS_PER_DAY = 86400000;

const startOfToday = (): Date => {
  const now = new Date();
  return new Date(now.getFullYear(), now.getMonth(), now.getDate());
};

// Reminder dates are date-only (YYYY-MM-DD); parse in local time to avoid
// UTC shifting the day.
const parseDateOnly = (dateStr: string): Date => {
  const [year, month, day] = dateStr.split('-').map(Number);
  return new Date(year, month - 1, day);
};

const limitDateOnly = (reminder: Reminder): string =>
  reminder.limitDateFormatted || reminder.limitDate?.slice(0, 10) || '';

const dayDiff = (dateStr: string, today: Date = startOfToday()): number =>
  Math.round((parseDateOnly(dateStr).getTime() - today.getTime()) / MS_PER_DAY);

const dateLabel = (dateStr: string, today: Date = startOfToday()): string => {
  const diff = dayDiff(dateStr, today);
  if (diff === 0) return 'Today';
  if (diff === 1) return 'Tomorrow';
  if (diff === -1) return 'Yesterday';
  if (diff < 0) return `${Math.abs(diff)} days late`;
  return parseDateOnly(dateStr).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
  });
};

const isOverdue = (reminder: Reminder, today: Date = startOfToday()): boolean =>
  !reminder.isDone && dayDiff(limitDateOnly(reminder), today) < 0;

const bucketOf = (reminder: Reminder, today: Date = startOfToday()): GroupName => {
  if (reminder.isDone) return 'Done';
  const diff = dayDiff(limitDateOnly(reminder), today);
  if (diff < 0) return 'Overdue';
  if (diff === 0) return 'Today';
  return 'Upcoming';
};

const matchesView = (
  reminder: Reminder,
  view: ViewName,
  today: Date = startOfToday(),
): boolean => view === 'All' || bucketOf(reminder, today) === view;

const matchesQuery = (reminder: Reminder, query: string): boolean => {
  const q = query.trim().toLowerCase();
  return (
    !q ||
    reminder.title.toLowerCase().includes(q) ||
    (reminder.description || '').toLowerCase().includes(q)
  );
};

// Live search on title + description combined with the sidebar view filter.
// Counts and week progress stay unfiltered; only the list narrows.
const filterReminders = (
  reminders: Reminder[],
  view: ViewName,
  query: string,
  today: Date = startOfToday(),
): Reminder[] =>
  reminders.filter(
    reminder => matchesView(reminder, view, today) && matchesQuery(reminder, query),
  );

const groupReminders = (
  reminders: Reminder[],
  today: Date = startOfToday(),
): ReminderGroup[] => {
  const sorted = reminders
    .slice()
    .sort((a, b) => limitDateOnly(a).localeCompare(limitDateOnly(b)));

  return GROUP_ORDER.map(label => ({
    label,
    items: sorted.filter(reminder => bucketOf(reminder, today) === label),
  })).filter(group => group.items.length > 0);
};

const viewCounts = (
  reminders: Reminder[],
  today: Date = startOfToday(),
): Record<ViewName, number> => ({
  All: reminders.length,
  Today: reminders.filter(r => bucketOf(r, today) === 'Today').length,
  Upcoming: reminders.filter(r => bucketOf(r, today) === 'Upcoming').length,
  Done: reminders.filter(r => r.isDone).length,
});

// Week progress: % done of reminders due within the next 7 days (overdue
// included, matching the design prototype). Caption surfaces overdue count
// when any exist.
const weekProgress = (
  reminders: Reminder[],
  today: Date = startOfToday(),
): WeekProgress => {
  const week = reminders.filter(r => dayDiff(limitDateOnly(r), today) <= 7);
  const done = week.filter(r => r.isDone).length;
  const pct = week.length ? Math.round((done / week.length) * 100) : 0;
  const overdue = reminders.filter(r => isOverdue(r, today)).length;

  const caption = overdue
    ? `${overdue} ${overdue === 1 ? 'reminder is' : 'reminders are'} overdue`
    : `${done} of ${week.length} done in the next 7 days`;

  return { pct, caption };
};

export {
  bucketOf,
  dateLabel,
  dayDiff,
  filterReminders,
  groupReminders,
  isOverdue,
  limitDateOnly,
  viewCounts,
  weekProgress,
};
