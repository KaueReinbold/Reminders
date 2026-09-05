import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:reminders_app/api/api_client.dart';
import 'package:reminders_app/api/reminder.dart';
import 'package:reminders_app/reminders/list_screen.dart';
import 'package:reminders_app/theme/tokens.dart';

/// Dates are relative to the real today: the screen groups against it.
DateTime dayFromToday(int offset) {
  final now = DateTime.now();
  return DateTime.utc(now.year, now.month, now.day).add(Duration(days: offset));
}

Map<String, dynamic> reminderJson({
  required String id,
  required String title,
  required int dueInDays,
  String description = '',
  bool isDone = false,
}) => {
  'id': id,
  'title': title,
  'description': description,
  'limitDate': Reminder.formatDate(dayFromToday(dueInDays)),
  'isDone': isDone,
};

final overdue = reminderJson(id: '1', title: 'Pay rent', dueInDays: -2);
final dueToday = reminderJson(
  id: '2',
  title: 'Call the notary',
  dueInDays: 0,
  description: 'Bring the contract',
);
final upcoming = reminderJson(id: '3', title: 'Book flights', dueInDays: 4);
final done = reminderJson(
  id: '4',
  title: 'Renew passport',
  dueInDays: -5,
  isDone: true,
);

Future<void> pumpScreen(
  WidgetTester tester,
  Future<http.Response> Function(http.Request) handler,
) async {
  final api = RemindersApi(
    client: MockClient(handler),
    baseUrl: 'http://localhost:9999',
  );
  await tester.pumpWidget(
    MaterialApp(theme: buildAppTheme(), home: RemindersListScreen(api: api)),
  );
  await tester.pumpAndSettle();
}

Future<void> pumpWith(WidgetTester tester, List<Map<String, dynamic>> items) =>
    pumpScreen(tester, (_) async => http.Response(jsonEncode(items), 200));

void main() {
  testWidgets('renders sections, cards and date pills', (tester) async {
    await pumpWith(tester, [overdue, dueToday, upcoming, done]);

    expect(find.text('Overdue'), findsOneWidget);
    expect(find.text('Today'), findsWidgets);
    expect(find.text('Upcoming'), findsWidgets);
    expect(find.text('Done'), findsWidgets);

    expect(find.text('Pay rent'), findsOneWidget);
    expect(find.text('Bring the contract'), findsOneWidget);
    expect(find.text('2 days late'), findsOneWidget);
  });

  testWidgets('shows a chip per view and the week progress', (tester) async {
    await pumpWith(tester, [overdue, dueToday, upcoming, done]);

    for (final label in ['All', 'Today', 'Upcoming', 'Done']) {
      expect(find.text(label), findsWidgets);
    }

    // 1 of the 4 reminders due within the week is done.
    expect(find.text('25%'), findsOneWidget);
    expect(find.text('1 reminder is overdue'), findsOneWidget);
  });

  testWidgets('filter chip narrows the list', (tester) async {
    await pumpWith(tester, [overdue, dueToday, upcoming, done]);

    await tester.tap(find.text('Upcoming').first);
    await tester.pumpAndSettle();

    expect(find.text('Book flights'), findsOneWidget);
    expect(find.text('Pay rent'), findsNothing);
  });

  testWidgets('search filters title and description', (tester) async {
    await pumpWith(tester, [overdue, dueToday, upcoming, done]);

    await tester.enterText(find.byType(TextField), 'contract');
    await tester.pumpAndSettle();

    expect(find.text('Call the notary'), findsOneWidget);
    expect(find.text('Pay rent'), findsNothing);
  });

  testWidgets('empty state depends on search and view', (tester) async {
    await pumpWith(tester, []);
    expect(find.text('Nothing on the list'), findsOneWidget);

    await tester.enterText(find.byType(TextField), 'zzz');
    await tester.pumpAndSettle();
    expect(find.text('No matches'), findsOneWidget);
  });

  testWidgets('checkbox toggles done and moves the card', (tester) async {
    final requests = <http.Request>[];
    await pumpScreen(tester, (request) async {
      requests.add(request);
      if (request.method == 'GET') {
        return http.Response(jsonEncode([overdue]), 200);
      }
      return http.Response(
        jsonEncode({...overdue, 'isDone': true}),
        200,
      );
    });

    expect(find.text('Overdue'), findsOneWidget);

    await tester.tap(find.byKey(const ValueKey('toggle-1')));
    await tester.pumpAndSettle();

    expect(requests.last.method, 'PUT');
    expect(jsonDecode(requests.last.body)['isDone'], true);
    expect(find.text('Overdue'), findsNothing);
    expect(find.text('Done'), findsWidgets);
  });

  testWidgets('surfaces API errors with a retry', (tester) async {
    var calls = 0;
    await pumpScreen(tester, (_) async {
      calls++;
      if (calls == 1) {
        return http.Response(jsonEncode({'message': 'Boom'}), 500);
      }
      return http.Response(jsonEncode([dueToday]), 200);
    });

    expect(find.text('Boom'), findsOneWidget);

    await tester.tap(find.text('Retry'));
    await tester.pumpAndSettle();

    expect(find.text('Boom'), findsNothing);
    expect(find.text('Call the notary'), findsOneWidget);
  });

  testWidgets('shows the create FAB', (tester) async {
    await pumpWith(tester, []);
    expect(find.widgetWithText(FloatingActionButton, 'New'), findsOneWidget);
  });
}
