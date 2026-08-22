import 'dart:convert';
import 'dart:io';

import 'package:http/http.dart' as http;

import 'reminder.dart';

/// Base URL of the nginx load balancer in front of the APIs.
/// Override at build time: flutter run --dart-define=API_BASE_URL=http://192.168.1.10:9999
/// The default reaches the host machine from the Android emulator.
const String defaultApiBaseUrl = String.fromEnvironment(
  'API_BASE_URL',
  defaultValue: 'http://10.0.2.2:9999',
);

/// Error returned by the API or raised while reaching it.
///
/// [statusCode] is 0 when the server could not be reached. [fieldErrors]
/// carries validation messages keyed by field when the API provides them
/// (the .NET API returns a problem-details `errors` map).
class ApiException implements Exception {
  const ApiException(
    this.statusCode,
    this.message, {
    this.fieldErrors = const {},
  });

  final int statusCode;
  final String message;
  final Map<String, List<String>> fieldErrors;

  bool get isNotFound => statusCode == 404;
  bool get isValidation => statusCode == 400 && fieldErrors.isNotEmpty;
  bool get isNetwork => statusCode == 0;

  @override
  String toString() => 'ApiException($statusCode): $message';
}

/// REST client for /api/reminders.
class RemindersApi {
  RemindersApi({http.Client? client, String baseUrl = defaultApiBaseUrl})
    : _client = client ?? http.Client(),
      _baseUrl =
          baseUrl.endsWith('/')
              ? baseUrl.substring(0, baseUrl.length - 1)
              : baseUrl;

  final http.Client _client;
  final String _baseUrl;

  static const _headers = {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  };

  Uri _uri([String path = '']) => Uri.parse('$_baseUrl/api/reminders$path');

  Future<List<Reminder>> fetchAll() async {
    final body = await _send(() => _client.get(_uri(), headers: _headers));
    final list = body as List<dynamic>;
    return list
        .map((e) => Reminder.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<Reminder> fetchOne(String id) async {
    final body = await _send(
      () => _client.get(_uri('/$id'), headers: _headers),
    );
    return Reminder.fromJson(body as Map<String, dynamic>);
  }

  Future<Reminder> create(Reminder reminder) async {
    final body = await _send(
      () => _client.post(
        _uri(),
        headers: _headers,
        body: jsonEncode(reminder.toJson()),
      ),
    );
    return Reminder.fromJson(body as Map<String, dynamic>);
  }

  Future<Reminder> update(Reminder reminder) async {
    final id = reminder.id;
    if (id == null || id.isEmpty) {
      throw ArgumentError('update requires a reminder with an id');
    }
    final body = await _send(
      () => _client.put(
        _uri('/$id'),
        headers: _headers,
        body: jsonEncode(reminder.toJson()),
      ),
    );
    return Reminder.fromJson(body as Map<String, dynamic>);
  }

  Future<void> delete(String id) async {
    await _send(() => _client.delete(_uri('/$id'), headers: _headers));
  }

  Future<void> toggleDone(Reminder reminder) =>
      update(reminder.copyWith(isDone: !reminder.isDone));

  void close() => _client.close();

  Future<dynamic> _send(Future<http.Response> Function() request) async {
    http.Response response;
    try {
      response = await request();
    } on SocketException catch (e) {
      throw ApiException(0, 'Could not reach the server: ${e.message}');
    } on http.ClientException catch (e) {
      throw ApiException(0, 'Could not reach the server: ${e.message}');
    }

    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw _errorFrom(response);
    }
    if (response.body.isEmpty) return null;
    try {
      return jsonDecode(response.body);
    } on FormatException catch (e) {
      throw ApiException(
        response.statusCode,
        'Invalid response body: ${e.message}',
      );
    }
  }

  static ApiException _errorFrom(http.Response response) {
    String message = 'Request failed (${response.statusCode})';
    final fieldErrors = <String, List<String>>{};
    try {
      final decoded = jsonDecode(response.body);
      if (decoded is Map<String, dynamic>) {
        // .NET problem details: {title, errors: {Field: [msg]}}; Go and C++: {message}
        final errors = decoded['errors'];
        if (errors is Map<String, dynamic>) {
          errors.forEach((key, value) {
            if (value is List) {
              fieldErrors[key] = value.map((e) => e.toString()).toList();
            }
          });
        }
        final title = decoded['message'] ?? decoded['title'];
        if (title is String && title.isNotEmpty) message = title;
      }
    } on FormatException {
      // Non-JSON body: keep the generic message.
    }
    if (fieldErrors.isNotEmpty) {
      message = fieldErrors.values.expand((e) => e).join(' ');
    }
    return ApiException(response.statusCode, message, fieldErrors: fieldErrors);
  }
}
