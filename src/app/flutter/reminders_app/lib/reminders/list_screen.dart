import 'package:flutter/material.dart';

import '../api/api_client.dart';
import '../api/reminder.dart';
import '../theme/tokens.dart';
import 'grouping.dart';

/// Mobile reminders list: sticky header with search and filter chips,
/// progress strip, grouped cards, empty states and a create FAB.
class RemindersListScreen extends StatefulWidget {
  const RemindersListScreen({super.key, required this.api});

  final RemindersApi api;

  @override
  State<RemindersListScreen> createState() => _RemindersListScreenState();
}

class _RemindersListScreenState extends State<RemindersListScreen> {
  List<Reminder> _items = const [];
  ReminderView _view = ReminderView.all;
  String _query = '';
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final items = await widget.api.fetchAll();
      if (!mounted) return;
      setState(() {
        _items = items;
        _loading = false;
      });
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.message;
        _loading = false;
      });
    }
  }

  /// Optimistic toggle: the card moves group immediately, reverted on failure.
  Future<void> _toggle(Reminder reminder) async {
    final toggled = reminder.copyWith(isDone: !reminder.isDone);
    setState(() => _items = _replace(_items, toggled));
    try {
      await widget.api.toggleDone(reminder);
      if (!mounted || _error == null) return;
      setState(() => _error = null);
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() {
        _items = _replace(_items, reminder);
        _error = e.message;
      });
    }
  }

  static List<Reminder> _replace(List<Reminder> items, Reminder updated) =>
      items.map((i) => i.id == updated.id ? updated : i).toList();

  @override
  Widget build(BuildContext context) {
    final today = startOfToday();
    final counts = viewCounts(_items, today);
    final progress = weekProgress(_items, today);
    final visible = filterReminders(_items, _view, _query, today);
    final groups = groupReminders(visible, today);

    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            _Header(
              today: today,
              query: _query,
              view: _view,
              counts: counts,
              onQuery: (value) => setState(() => _query = value),
              onView: (value) => setState(() => _view = value),
            ),
            _ProgressStrip(progress: progress),
            if (_error != null) _ErrorBanner(message: _error!, onRetry: _load),
            Expanded(
              child:
                  _loading
                      ? const Center(child: CircularProgressIndicator())
                      : RefreshIndicator(
                        onRefresh: _load,
                        child: _List(
                          groups: groups,
                          today: today,
                          view: _view,
                          query: _query,
                          onToggle: _toggle,
                        ),
                      ),
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        // The create sheet arrives with issue #339.
        onPressed: () {},
        backgroundColor: AppColors.ink,
        foregroundColor: AppColors.inputFill,
        icon: const Icon(Icons.add, size: 16),
        label: const Text('New'),
        shape: const StadiumBorder(),
      ),
    );
  }
}

class _Header extends StatelessWidget {
  const _Header({
    required this.today,
    required this.query,
    required this.view,
    required this.counts,
    required this.onQuery,
    required this.onView,
  });

  final DateTime today;
  final String query;
  final ReminderView view;
  final Map<ReminderView, int> counts;
  final ValueChanged<String> onQuery;
  final ValueChanged<ReminderView> onView;

  @override
  Widget build(BuildContext context) {
    final text = Theme.of(context).textTheme;
    return Container(
      decoration: const BoxDecoration(
        color: AppColors.canvas,
        border: Border(bottom: BorderSide(color: AppColors.borderPage)),
      ),
      padding: const EdgeInsets.fromLTRB(18, 16, 18, 12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.baseline,
            textBaseline: TextBaseline.alphabetic,
            children: [
              Text('Reminders', style: text.headlineMedium),
              const Spacer(),
              Text(
                shortDateLabel(today),
                style: text.headlineMedium?.copyWith(
                  fontSize: 15,
                  fontStyle: FontStyle.italic,
                  color: AppColors.faint,
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          TextField(
            onChanged: onQuery,
            style: const TextStyle(fontSize: 15),
            decoration: InputDecoration(
              hintText: 'Search reminders',
              hintStyle: const TextStyle(color: AppColors.faint, fontSize: 15),
              prefixIcon: const Icon(
                Icons.search,
                size: 18,
                color: AppColors.faint,
              ),
              filled: true,
              fillColor: AppColors.surface,
              isDense: true,
              contentPadding: const EdgeInsets.symmetric(vertical: 12),
              border: _pillBorder(AppColors.borderPage),
              enabledBorder: _pillBorder(AppColors.borderPage),
              focusedBorder: _pillBorder(AppColors.accent),
            ),
          ),
          const SizedBox(height: 12),
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: Row(
              children: [
                for (final option in ReminderView.values)
                  Padding(
                    padding: const EdgeInsets.only(right: 7),
                    child: _Chip(
                      label: option.label,
                      count: counts[option] ?? 0,
                      selected: option == view,
                      onTap: () => onView(option),
                    ),
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  static OutlineInputBorder _pillBorder(Color color) => OutlineInputBorder(
    borderRadius: BorderRadius.circular(AppRadii.pill),
    borderSide: BorderSide(color: color),
  );
}

class _Chip extends StatelessWidget {
  const _Chip({
    required this.label,
    required this.count,
    required this.selected,
    required this.onTap,
  });

  final String label;
  final int count;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppRadii.pill),
      child: Container(
        constraints: const BoxConstraints(minHeight: 40),
        padding: const EdgeInsets.symmetric(horizontal: 15),
        decoration: BoxDecoration(
          color: selected ? AppColors.ink : AppColors.surface,
          borderRadius: BorderRadius.circular(AppRadii.pill),
          border: Border.all(
            color: selected ? AppColors.ink : AppColors.borderPage,
          ),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              label,
              style: TextStyle(
                fontSize: 14,
                fontWeight: selected ? FontWeight.w500 : FontWeight.w400,
                color: selected ? AppColors.inputFill : AppColors.bodyMuted,
              ),
            ),
            const SizedBox(width: 7),
            Text(
              '$count',
              style: TextStyle(
                fontSize: 12.5,
                color: selected ? AppColors.borderHover : AppColors.faint,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ProgressStrip extends StatelessWidget {
  const _ProgressStrip({required this.progress});

  final WeekProgress progress;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 14),
      decoration: const BoxDecoration(
        color: AppColors.panel,
        border: Border(bottom: BorderSide(color: AppColors.borderPage)),
      ),
      child: Row(
        children: [
          Text(
            '${progress.percent}%',
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(width: 12),
          Expanded(
            child: ClipRRect(
              borderRadius: BorderRadius.circular(AppRadii.pill),
              child: LinearProgressIndicator(
                value: progress.percent / 100,
                minHeight: 5,
                backgroundColor: AppColors.borderPage,
                valueColor: const AlwaysStoppedAnimation(AppColors.accent),
              ),
            ),
          ),
          const SizedBox(width: 12),
          ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 140),
            child: Text(
              progress.caption,
              textAlign: TextAlign.right,
              style: const TextStyle(fontSize: 12.5, color: AppColors.bodyMuted),
            ),
          ),
        ],
      ),
    );
  }
}

class _ErrorBanner extends StatelessWidget {
  const _ErrorBanner({required this.message, required this.onRetry});

  final String message;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    return Container(
      color: AppColors.accentTint,
      padding: const EdgeInsets.fromLTRB(18, 10, 8, 10),
      child: Row(
        children: [
          Expanded(
            child: Text(
              message,
              style: const TextStyle(fontSize: 13.5, color: AppColors.accent),
            ),
          ),
          TextButton(onPressed: onRetry, child: const Text('Retry')),
        ],
      ),
    );
  }
}

class _List extends StatelessWidget {
  const _List({
    required this.groups,
    required this.today,
    required this.view,
    required this.query,
    required this.onToggle,
  });

  final List<ReminderGroup> groups;
  final DateTime today;
  final ReminderView view;
  final String query;
  final ValueChanged<Reminder> onToggle;

  @override
  Widget build(BuildContext context) {
    const padding = EdgeInsets.fromLTRB(18, 18, 18, 120);
    if (groups.isEmpty) {
      return ListView(
        padding: padding,
        children: [_EmptyState(state: emptyState(view, query))],
      );
    }
    return ListView.separated(
      padding: padding,
      itemCount: groups.length,
      separatorBuilder: (_, _) => const SizedBox(height: AppSpace.gapSections),
      itemBuilder: (context, index) {
        final group = groups[index];
        return Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            _SectionHeader(label: group.name.label, count: group.items.length),
            for (final reminder in group.items)
              Padding(
                padding: const EdgeInsets.only(top: AppSpace.gapCards),
                child: _ReminderCard(
                  reminder: reminder,
                  today: today,
                  onToggle: () => onToggle(reminder),
                ),
              ),
          ],
        );
      },
    );
  }
}

class _SectionHeader extends StatelessWidget {
  const _SectionHeader({required this.label, required this.count});

  final String label;
  final int count;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Text(label, style: Theme.of(context).textTheme.headlineMedium),
        const SizedBox(width: 12),
        Text(
          '$count',
          style: const TextStyle(fontSize: 13, color: AppColors.faint),
        ),
        const SizedBox(width: 12),
        const Expanded(child: Divider(color: AppColors.borderPage, height: 1)),
      ],
    );
  }
}

class _ReminderCard extends StatelessWidget {
  const _ReminderCard({
    required this.reminder,
    required this.today,
    required this.onToggle,
  });

  final Reminder reminder;
  final DateTime today;
  final VoidCallback onToggle;

  @override
  Widget build(BuildContext context) {
    final overdue = isOverdue(reminder, today);
    return Container(
      padding: AppSpace.padCard,
      decoration: BoxDecoration(
        color: AppColors.surface,
        border: Border.all(color: AppColors.borderCard),
        borderRadius: BorderRadius.circular(AppRadii.card),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          _Checkbox(
            key: ValueKey('toggle-${reminder.id}'),
            done: reminder.isDone,
            onTap: onToggle,
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  reminder.title,
                  style: TextStyle(
                    fontSize: 15.5,
                    fontWeight: FontWeight.w500,
                    color: reminder.isDone ? AppColors.disabled : AppColors.ink,
                    decoration:
                        reminder.isDone ? TextDecoration.lineThrough : null,
                  ),
                ),
                if (reminder.description.isNotEmpty) ...[
                  const SizedBox(height: 5),
                  Text(
                    reminder.description,
                    style: const TextStyle(
                      fontSize: 13.8,
                      color: AppColors.bodyMuted,
                    ),
                  ),
                ],
              ],
            ),
          ),
          const SizedBox(width: 10),
          _DatePill(label: dateLabel(reminder, today), overdue: overdue),
        ],
      ),
    );
  }
}

class _Checkbox extends StatelessWidget {
  const _Checkbox({super.key, required this.done, required this.onTap});

  final bool done;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Semantics(
      button: true,
      label: done ? 'Mark not done' : 'Mark done',
      child: InkWell(
        onTap: onTap,
        customBorder: const CircleBorder(),
        child: Container(
          width: 24,
          height: 24,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: done ? AppColors.success : Colors.transparent,
            border:
                done
                    ? null
                    : Border.all(color: AppColors.borderHoverStrong, width: 1.6),
          ),
          child:
              done
                  ? const Icon(Icons.check, size: 14, color: AppColors.surface)
                  : null,
        ),
      ),
    );
  }
}

class _DatePill extends StatelessWidget {
  const _DatePill({required this.label, required this.overdue});

  final String label;
  final bool overdue;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 11, vertical: 6),
      decoration: BoxDecoration(
        color: overdue ? AppColors.accentTint : AppColors.pillNeutralBg,
        borderRadius: BorderRadius.circular(AppRadii.pill),
      ),
      child: Text(
        label,
        style: TextStyle(
          fontSize: 12.5,
          fontWeight: overdue ? FontWeight.w500 : FontWeight.w400,
          color: overdue ? AppColors.accent : AppColors.bodyMuted,
        ),
      ),
    );
  }
}

class _EmptyState extends StatelessWidget {
  const _EmptyState({required this.state});

  final ({String title, String body}) state;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 64),
      decoration: BoxDecoration(
        color: AppColors.surface,
        border: Border.all(color: AppColors.borderHover),
        borderRadius: BorderRadius.circular(AppRadii.modal),
      ),
      child: Column(
        children: [
          Text(
            state.title,
            textAlign: TextAlign.center,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 14),
          Text(
            state.body,
            textAlign: TextAlign.center,
            style: const TextStyle(fontSize: 15, color: AppColors.bodyMuted),
          ),
        ],
      ),
    );
  }
}
