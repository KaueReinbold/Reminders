import http from 'k6/http';
import { check } from 'k6';

import { baseUrl } from './index.js';

export default function () {
  const url = `${baseUrl}/api/Reminders`;
  const headers = {
    'content-type': 'application/json',
  };
  const response = http.get(url, {
    headers,
  });

  check(response, { "status is 200": (r) => r.status === 200 });
}

export * from './index.js'