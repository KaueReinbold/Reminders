import 'package:flutter/material.dart';

import 'api/api_client.dart';
import 'reminders/list_screen.dart';
import 'theme/tokens.dart';

void main() {
  runApp(RemindersApp(api: RemindersApi()));
}

class RemindersApp extends StatelessWidget {
  const RemindersApp({super.key, required this.api});

  final RemindersApi api;

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Reminders',
      theme: buildAppTheme(),
      home: RemindersListScreen(api: api),
    );
  }
}
