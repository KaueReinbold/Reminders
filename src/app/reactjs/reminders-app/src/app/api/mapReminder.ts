import { Reminder } from './types';

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  const year = date.getUTCFullYear();
  const month = ('0' + (date.getUTCMonth() + 1)).slice(-2);
  const day = ('0' + date.getUTCDate()).slice(-2);
  return `${year}-${month}-${day}`;
};

const mapReminder = (reminder: Reminder): Reminder =>
  ({
    ...reminder,
    limitDateFormatted: reminder.limitDate
      ? formatDate(reminder.limitDate)
      : '',
    isDoneFormatted: reminder.isDone ? 'Yes' : 'No',
  }) as Reminder;

export { formatDate, mapReminder };
