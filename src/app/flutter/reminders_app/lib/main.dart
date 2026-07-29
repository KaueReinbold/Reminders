import 'package:flutter/material.dart';

import 'theme/tokens.dart';

void main() {
  runApp(const RemindersApp());
}

class RemindersApp extends StatelessWidget {
  const RemindersApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Reminders',
      theme: buildAppTheme(),
      home: const _Placeholder(),
    );
  }
}

class _Placeholder extends StatelessWidget {
  const _Placeholder();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Text(
          'Reminders',
          style: Theme.of(context).textTheme.headlineLarge,
        ),
      ),
    );
  }
}
