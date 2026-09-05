import '../api/reminder.dart';

/// Grouping, filtering and progress rules for the reminders list.
///
/// Mirrors the React implementation (`src/app/util/reminderGroups.ts`) so both
/// clients bucket and label reminders the same way.

/// Sections of the list, in render order. Empty groups are hidden.
enum ReminderGroupName { overdue, today, upcoming, done }

/// Filter chips, in render order.
enum ReminderView { all, today, upcoming, done }

extension ReminderGroupLabel on ReminderGroupName {
  String get label => switch (this) {
    ReminderGroupName.overdue => 'Overdue',
    ReminderGroupName.today => 'Today',
    ReminderGroupName.upcoming => 'Upcoming',
    ReminderGroupName.done => 'Done',
  };
}

extension ReminderViewLabel on ReminderView {
  String get label => switch (this) {
    ReminderView.all => 'All',
    ReminderView.today => 'Today',
    ReminderView.upcoming => 'Upcoming',
    ReminderView.done => 'Done',
  };
}

class ReminderGroup {
  const ReminderGroup(this.name, this.items);

  final ReminderGroupName name;
  final List<Reminder> items;
}

class WeekProgress {
  const WeekProgress(this.percent, this.caption);

  final int percent;
  final String caption;
}

const _months = [
  'Jan',
  'Feb',
  'Mar',
  'Apr',
  'May',
  'Jun',
  'Jul',
  'Aug',
  'Sep',
  'Oct',
  'Nov',
  'Dec',
];

const _weekdays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

/// Today as a UTC midnight date, matching how [Reminder.limitDate] is stored.
DateTime startOfToday() {
  final now = DateTime.now();
  return DateTime.utc(now.year, now.month, now.day);
}

/// Whole days between a reminder due date and [today]; negative when past.
int dayDiff(Reminder reminder, DateTime today) =>
    reminder.limitDate.difference(today).inDays;

ReminderGroupName bucketOf(Reminder reminder, DateTime today) {
  if (reminder.isDone) return ReminderGroupName.done;
  final diff = dayDiff(reminder, today);
  if (diff < 0) return ReminderGroupName.overdue;
  if (diff == 0) return ReminderGroupName.today;
  return ReminderGroupName.upcoming;
}

bool isOverdue(Reminder reminder, DateTime today) =>
    bucketOf(reminder, today) == ReminderGroupName.overdue;

/// Date pill text: Today, Tomorrow, Yesterday, "3 days late" or "Aug 4".
String dateLabel(Reminder reminder, DateTime today) {
  final diff = dayDiff(reminder, today);
  if (diff == 0) return 'Today';
  if (diff == 1) return 'Tomorrow';
  if (diff == -1) return 'Yesterday';
  if (diff < 0) return '${-diff} days late';
  final date = reminder.limitDate;
  return '${_months[date.month - 1]} ${date.day}';
}

/// Header date, e.g. "Fri, Sep 5".
String shortDateLabel(DateTime today) =>
    '${_weekdays[today.weekday - 1]}, ${_months[today.month - 1]} ${today.day}';

/// Applies the chip filter plus live search over title and description.
/// Counts and progress stay unfiltered; only the list narrows.
List<Reminder> filterReminders(
  List<Reminder> reminders,
  ReminderView view,
  String query,
  DateTime today,
) {
  final q = query.trim().toLowerCase();
  return reminders.where((reminder) {
    final matchesView =
        view == ReminderView.all ||
        bucketOf(reminder, today) == _groupFor(view);
    final matchesQuery =
        q.isEmpty ||
        reminder.title.toLowerCase().contains(q) ||
        reminder.description.toLowerCase().contains(q);
    return matchesView && matchesQuery;
  }).toList();
}

ReminderGroupName? _groupFor(ReminderView view) => switch (view) {
  ReminderView.all => null,
  ReminderView.today => ReminderGroupName.today,
  ReminderView.upcoming => ReminderGroupName.upcoming,
  ReminderView.done => ReminderGroupName.done,
};

/// Groups in section order, sorted by due date, dropping empty groups.
List<ReminderGroup> groupReminders(List<Reminder> reminders, DateTime today) {
  final sorted =
      reminders.toList()..sort((a, b) => a.limitDate.compareTo(b.limitDate));
  return ReminderGroupName.values
      .map(
        (name) => ReminderGroup(
          name,
          sorted.where((r) => bucketOf(r, today) == name).toList(),
        ),
      )
      .where((group) => group.items.isNotEmpty)
      .toList();
}

Map<ReminderView, int> viewCounts(List<Reminder> reminders, DateTime today) => {
  ReminderView.all: reminders.length,
  ReminderView.today:
      reminders.where((r) => bucketOf(r, today) == ReminderGroupName.today).length,
  ReminderView.upcoming:
      reminders
          .where((r) => bucketOf(r, today) == ReminderGroupName.upcoming)
          .length,
  ReminderView.done: reminders.where((r) => r.isDone).length,
};

/// Percent done of reminders due within the next 7 days (overdue included,
/// matching the design prototype). The caption surfaces overdue count first.
WeekProgress weekProgress(List<Reminder> reminders, DateTime today) {
  final week = reminders.where((r) => dayDiff(r, today) <= 7).toList();
  final done = week.where((r) => r.isDone).length;
  final percent = week.isEmpty ? 0 : (done * 100 / week.length).round();
  final overdue = reminders.where((r) => isOverdue(r, today)).length;
  final caption =
      overdue > 0
          ? '$overdue ${overdue == 1 ? 'reminder is' : 'reminders are'} overdue'
          : '$done of ${week.length} done in the next 7 days';
  return WeekProgress(percent, caption);
}

/// Empty-state copy: search first, then the active chip.
({String title, String body}) emptyState(ReminderView view, String query) {
  if (query.trim().isNotEmpty) {
    return (title: 'No matches', body: 'No reminder matches "$query".');
  }
  return switch (view) {
    ReminderView.all => (
      title: 'Nothing on the list',
      body:
          'Everything is clear. Add a reminder when something needs to come back to you.',
    ),
    ReminderView.today => (
      title: 'Today is clear',
      body: 'No reminders due today, check Upcoming to get ahead.',
    ),
    ReminderView.upcoming => (
      title: 'Nothing scheduled',
      body: 'No future reminders yet.',
    ),
    ReminderView.done => (
      title: 'Nothing completed yet',
      body: 'Tick something off and it will land here.',
    ),
  };
}
