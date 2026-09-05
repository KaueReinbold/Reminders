import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:reminders_app/api/api_client.dart';
import 'package:reminders_app/main.dart';
import 'package:reminders_app/theme/tokens.dart';

void main() {
  testWidgets('boots into the list screen with the design theme', (
    tester,
  ) async {
    final api = RemindersApi(
      client: MockClient((_) async => http.Response('[]', 200)),
    );

    await tester.pumpWidget(RemindersApp(api: api));
    await tester.pumpAndSettle();

    expect(find.text('Reminders'), findsOneWidget);

    final context = tester.element(find.text('Reminders'));
    expect(Theme.of(context).scaffoldBackgroundColor, AppColors.canvas);
  });
}
