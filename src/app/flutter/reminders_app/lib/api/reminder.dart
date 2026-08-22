/// Reminder as exposed by the REST API (same shape across the .NET, Go and
/// C++ implementations): {id, title, description, limitDate, isDone}.
class Reminder {
  const Reminder({
    this.id,
    required this.title,
    required this.description,
    required this.limitDate,
    this.isDone = false,
  });

  final String? id;
  final String title;
  final String description;

  /// Due date. Only the calendar day matters; stored as UTC midnight.
  final DateTime limitDate;
  final bool isDone;

  factory Reminder.fromJson(Map<String, dynamic> json) {
    return Reminder(
      id: json['id'] as String?,
      title: json['title'] as String? ?? '',
      description: json['description'] as String? ?? '',
      limitDate: _parseDate(json['limitDate'] as String?),
      isDone: json['isDone'] as bool? ?? false,
    );
  }

  Map<String, dynamic> toJson() => {
    if (id != null) 'id': id,
    'title': title,
    'description': description,
    'limitDate': formatDate(limitDate),
    'isDone': isDone,
  };

  Reminder copyWith({
    String? id,
    String? title,
    String? description,
    DateTime? limitDate,
    bool? isDone,
  }) {
    return Reminder(
      id: id ?? this.id,
      title: title ?? this.title,
      description: description ?? this.description,
      limitDate: limitDate ?? this.limitDate,
      isDone: isDone ?? this.isDone,
    );
  }

  /// Date-only value the APIs agree on: yyyy-MM-ddT00:00:00Z.
  static String formatDate(DateTime date) {
    final utc = DateTime.utc(date.year, date.month, date.day);
    return '${utc.toIso8601String().substring(0, 10)}T00:00:00Z';
  }

  static DateTime _parseDate(String? raw) {
    if (raw == null || raw.isEmpty) {
      throw const FormatException('limitDate is missing');
    }
    final parsed = DateTime.parse(raw).toUtc();
    return DateTime.utc(parsed.year, parsed.month, parsed.day);
  }

  @override
  bool operator ==(Object other) =>
      other is Reminder &&
      other.id == id &&
      other.title == title &&
      other.description == description &&
      other.limitDate == limitDate &&
      other.isDone == isDone;

  @override
  int get hashCode => Object.hash(id, title, description, limitDate, isDone);

  @override
  String toString() =>
      'Reminder(id: $id, title: $title, limitDate: ${formatDate(limitDate)}, isDone: $isDone)';
}
