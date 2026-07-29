import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:reminders_app/main.dart';
import 'package:reminders_app/theme/tokens.dart';

void main() {
  testWidgets('renders placeholder with design theme', (tester) async {
    await tester.pumpWidget(const RemindersApp());

    expect(find.text('Reminders'), findsOneWidget);

    final context = tester.element(find.text('Reminders'));
    expect(Theme.of(context).scaffoldBackgroundColor, AppColors.canvas);
  });
}
