import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:reminders_app/api/api_client.dart';
import 'package:reminders_app/api/reminder.dart';

const baseUrl = 'http://localhost:9999';

final sampleJson = {
  'id': 'abc',
  'title': 'Pay rent',
  'description': 'Transfer',
  'limitDate': '2026-09-01T00:00:00Z',
  'isDone': false,
};

final sample = Reminder.fromJson(sampleJson);

/// Same reminder before the API assigned an id.
final sampleNew = Reminder(
  title: sample.title,
  description: sample.description,
  limitDate: sample.limitDate,
);

/// Builds an api whose client records the request and answers with [handler].
RemindersApi apiWith(Future<http.Response> Function(http.Request) handler) =>
    RemindersApi(client: MockClient(handler), baseUrl: baseUrl);

void main() {
  group('fetchAll', () {
    test('GETs /api/reminders and maps the list', () async {
      http.Request? seen;
      final api = apiWith((request) async {
        seen = request;
        return http.Response(jsonEncode([sampleJson]), 200);
      });

      final reminders = await api.fetchAll();

      expect(seen!.method, 'GET');
      expect(seen!.url.toString(), '$baseUrl/api/reminders');
      expect(seen!.headers['Accept'], 'application/json');
      expect(reminders, [sample]);
    });

    test('empty list', () async {
      final api = apiWith((_) async => http.Response('[]', 200));
      expect(await api.fetchAll(), isEmpty);
    });

    test('server error becomes ApiException with the API message', () async {
      final api = apiWith(
        (_) async => http.Response(
          jsonEncode({'message': 'Could not get reminders'}),
          500,
        ),
      );

      await expectLater(
        api.fetchAll,
        throwsA(
          isA<ApiException>()
              .having((e) => e.statusCode, 'statusCode', 500)
              .having((e) => e.message, 'message', 'Could not get reminders'),
        ),
      );
    });

    test('unreachable server becomes a network ApiException', () async {
      final api = apiWith(
        (_) async => throw http.ClientException('Connection refused'),
      );

      await expectLater(
        api.fetchAll,
        throwsA(
          isA<ApiException>().having((e) => e.isNetwork, 'isNetwork', isTrue),
        ),
      );
    });
  });

  group('fetchOne', () {
    test('GETs /api/reminders/{id}', () async {
      http.Request? seen;
      final api = apiWith((request) async {
        seen = request;
        return http.Response(jsonEncode(sampleJson), 200);
      });

      expect(await api.fetchOne('abc'), sample);
      expect(seen!.url.path, '/api/reminders/abc');
    });

    test('404 is flagged as not found', () async {
      final api = apiWith(
        (_) async =>
            http.Response(jsonEncode({'message': 'Reminder not found'}), 404),
      );

      await expectLater(
        () => api.fetchOne('missing'),
        throwsA(
          isA<ApiException>().having((e) => e.isNotFound, 'isNotFound', isTrue),
        ),
      );
    });
  });

  group('create', () {
    test('POSTs the JSON body and returns the created reminder', () async {
      http.Request? seen;
      final api = apiWith((request) async {
        seen = request;
        return http.Response(jsonEncode(sampleJson), 201);
      });

      final created = await api.create(sampleNew);

      expect(seen!.method, 'POST');
      expect(seen!.headers['Content-Type'], startsWith('application/json'));
      expect(jsonDecode(seen!.body), {
        'title': 'Pay rent',
        'description': 'Transfer',
        'limitDate': '2026-09-01T00:00:00Z',
        'isDone': false,
      });
      expect(created.id, 'abc');
    });

    test('.NET validation errors are mapped by field', () async {
      final api = apiWith(
        (_) async => http.Response(
          jsonEncode({
            'title': 'One or more validation errors occurred.',
            'status': 400,
            'errors': {
              'Title': ['The Title field is required.'],
              'LimitDate.Date': ['The Limit Date should be later than Today.'],
            },
          }),
          400,
        ),
      );

      await expectLater(
        () => api.create(sample),
        throwsA(
          isA<ApiException>()
              .having((e) => e.isValidation, 'isValidation', isTrue)
              .having((e) => e.fieldErrors['Title'], 'Title', [
                'The Title field is required.',
              ])
              .having(
                (e) => e.message,
                'message',
                contains('later than Today'),
              ),
        ),
      );
    });

    test('non-JSON error body keeps a generic message', () async {
      final api = apiWith(
        (_) async => http.Response('<html>Bad Gateway</html>', 502),
      );

      await expectLater(
        () => api.create(sample),
        throwsA(
          isA<ApiException>()
              .having((e) => e.statusCode, 'statusCode', 502)
              .having((e) => e.message, 'message', 'Request failed (502)'),
        ),
      );
    });
  });

  group('update', () {
    test('PUTs /api/reminders/{id} with the body', () async {
      http.Request? seen;
      final api = apiWith((request) async {
        seen = request;
        return http.Response(request.body, 200);
      });

      final updated = await api.update(sample.copyWith(isDone: true));

      expect(seen!.method, 'PUT');
      expect(seen!.url.path, '/api/reminders/abc');
      expect(jsonDecode(seen!.body)['isDone'], isTrue);
      expect(updated.isDone, isTrue);
    });

    test('requires an id', () async {
      final api = apiWith((_) async => http.Response('', 200));
      expect(() => api.update(sampleNew), throwsArgumentError);
    });

    test('toggleDone flips isDone', () async {
      http.Request? seen;
      final api = apiWith((request) async {
        seen = request;
        return http.Response(request.body, 200);
      });

      await api.toggleDone(sample);

      expect(jsonDecode(seen!.body)['isDone'], isTrue);
    });
  });

  group('delete', () {
    test('DELETEs /api/reminders/{id} and accepts an empty body', () async {
      http.Request? seen;
      final api = apiWith((request) async {
        seen = request;
        return http.Response('', 204);
      });

      await api.delete('abc');

      expect(seen!.method, 'DELETE');
      expect(seen!.url.path, '/api/reminders/abc');
    });

    test('error propagates', () async {
      final api = apiWith(
        (_) async => http.Response('{"message":"nope"}', 500),
      );
      await expectLater(() => api.delete('abc'), throwsA(isA<ApiException>()));
    });
  });

  test('base URL trailing slash is tolerated', () async {
    http.Request? seen;
    final api = RemindersApi(
      client: MockClient((request) async {
        seen = request;
        return http.Response('[]', 200);
      }),
      baseUrl: '$baseUrl/',
    );

    await api.fetchAll();

    expect(seen!.url.toString(), '$baseUrl/api/reminders');
  });
}
