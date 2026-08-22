import 'package:flutter_test/flutter_test.dart';
import 'package:reminders_app/api/reminder.dart';

void main() {
  group('Reminder.fromJson', () {
    test('parses the API shape with a Z date', () {
      final reminder = Reminder.fromJson({
        'id': 'abc',
        'title': 'Pay rent',
        'description': 'Transfer',
        'limitDate': '2026-09-01T00:00:00Z',
        'isDone': false,
      });

      expect(reminder.id, 'abc');
      expect(reminder.title, 'Pay rent');
      expect(reminder.description, 'Transfer');
      expect(reminder.limitDate, DateTime.utc(2026, 9, 1));
      expect(reminder.isDone, isFalse);
    });

    test('keeps the calendar day for offset dates (.NET shape)', () {
      final reminder = Reminder.fromJson({
        'id': 'abc',
        'title': 't',
        'description': 'd',
        'limitDate': '2026-09-01T00:00:00+00:00',
        'isDone': true,
      });

      expect(reminder.limitDate, DateTime.utc(2026, 9, 1));
      expect(reminder.isDone, isTrue);
    });

    test('tolerates missing optional fields', () {
      final reminder = Reminder.fromJson({'limitDate': '2026-09-01T00:00:00Z'});

      expect(reminder.id, isNull);
      expect(reminder.title, '');
      expect(reminder.description, '');
      expect(reminder.isDone, isFalse);
    });

    test('throws on a missing date', () {
      expect(() => Reminder.fromJson({'title': 't'}), throwsFormatException);
    });
  });

  group('Reminder.toJson', () {
    test('serializes a date-only UTC limitDate and omits a null id', () {
      final json =
          Reminder(
            title: 'Pay rent',
            description: 'Transfer',
            limitDate: DateTime(2026, 9, 1, 15, 30),
          ).toJson();

      expect(json, {
        'title': 'Pay rent',
        'description': 'Transfer',
        'limitDate': '2026-09-01T00:00:00Z',
        'isDone': false,
      });
    });

    test('includes the id when present and round-trips', () {
      final original = Reminder(
        id: 'abc',
        title: 'Pay rent',
        description: 'Transfer',
        limitDate: DateTime.utc(2026, 9, 1),
        isDone: true,
      );

      expect(Reminder.fromJson(original.toJson()), original);
    });
  });

  test('copyWith overrides only the given fields', () {
    final original = Reminder(
      id: 'abc',
      title: 'Pay rent',
      description: 'Transfer',
      limitDate: DateTime.utc(2026, 9, 1),
    );

    final done = original.copyWith(isDone: true);

    expect(done.isDone, isTrue);
    expect(done.id, original.id);
    expect(done.title, original.title);
    expect(done.limitDate, original.limitDate);
    expect(done, isNot(equals(original)));
  });
}
