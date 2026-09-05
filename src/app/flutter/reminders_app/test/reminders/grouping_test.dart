import 'package:flutter_test/flutter_test.dart';
import 'package:reminders_app/api/reminder.dart';
import 'package:reminders_app/reminders/grouping.dart';

final today = DateTime.utc(2026, 9, 5);

Reminder at(int offsetDays, {bool isDone = false, String title = 'Task'}) =>
    Reminder(
      id: '$title$offsetDays',
      title: title,
      description: '',
      limitDate: today.add(Duration(days: offsetDays)),
      isDone: isDone,
    );

void main() {
  group('bucketOf', () {
    test('buckets by due date and done flag', () {
      expect(bucketOf(at(-1), today), ReminderGroupName.overdue);
      expect(bucketOf(at(0), today), ReminderGroupName.today);
      expect(bucketOf(at(3), today), ReminderGroupName.upcoming);
      expect(bucketOf(at(-1, isDone: true), today), ReminderGroupName.done);
    });
  });

  group('dateLabel', () {
    test('uses relative wording near today and a short date otherwise', () {
      expect(dateLabel(at(0), today), 'Today');
      expect(dateLabel(at(1), today), 'Tomorrow');
      expect(dateLabel(at(-1), today), 'Yesterday');
      expect(dateLabel(at(-3), today), '3 days late');
      expect(dateLabel(at(9), today), 'Sep 14');
    });
  });

  test('shortDateLabel formats the header date', () {
    expect(shortDateLabel(today), 'Sat, Sep 5');
  });

  group('filterReminders', () {
    final items = [
      at(0, title: 'Today task'),
      at(2, title: 'Later task'),
      at(-1, title: 'Old task', isDone: true),
    ];

    test('filters by view', () {
      expect(
        filterReminders(items, ReminderView.today, '', today).single.title,
        'Today task',
      );
      expect(
        filterReminders(items, ReminderView.done, '', today).single.title,
        'Old task',
      );
      expect(filterReminders(items, ReminderView.all, '', today), hasLength(3));
    });

    test('searches title and description case-insensitively', () {
      expect(
        filterReminders(items, ReminderView.all, 'later', today).single.title,
        'Later task',
      );
      expect(filterReminders(items, ReminderView.all, 'zzz', today), isEmpty);
    });
  });

  test('groupReminders orders sections and drops empty ones', () {
    final groups = groupReminders([at(2), at(-1), at(0)], today);
    expect(
      groups.map((g) => g.name),
      [
        ReminderGroupName.overdue,
        ReminderGroupName.today,
        ReminderGroupName.upcoming,
      ],
    );
  });

  test('viewCounts counts every chip', () {
    final counts = viewCounts([at(0), at(2), at(-1, isDone: true)], today);
    expect(counts[ReminderView.all], 3);
    expect(counts[ReminderView.today], 1);
    expect(counts[ReminderView.upcoming], 1);
    expect(counts[ReminderView.done], 1);
  });

  group('weekProgress', () {
    test('is the done share of the next 7 days', () {
      final progress = weekProgress([at(1), at(2, isDone: true)], today);
      expect(progress.percent, 50);
      expect(progress.caption, '1 of 2 done in the next 7 days');
    });

    test('ignores reminders past the week window', () {
      expect(weekProgress([at(30)], today).percent, 0);
      expect(
        weekProgress([at(30)], today).caption,
        '0 of 0 done in the next 7 days',
      );
    });

    test('caption surfaces overdue reminders first', () {
      expect(weekProgress([at(-1)], today).caption, '1 reminder is overdue');
      expect(
        weekProgress([at(-1), at(-2)], today).caption,
        '2 reminders are overdue',
      );
    });
  });

  test('emptyState prefers the search copy', () {
    expect(emptyState(ReminderView.all, 'rent').title, 'No matches');
    expect(emptyState(ReminderView.all, '').title, 'Nothing on the list');
    expect(emptyState(ReminderView.done, '').title, 'Nothing completed yet');
  });
}
