import http from 'k6/http';
import { check } from 'k6';

import { baseUrl } from './index.js';

export default function () {
  const url = `${baseUrl}/api/Reminders`;
  const headers = {
    'content-type': 'application/json',
  };
  const data = {
    title: 'Title',
    description: 'Description',
    isDone: false,
    limitDate: new Date(new Date().getFullYear() + 1, 1, 1),
  };

  const response = http.post(url, JSON.stringify(data), {
    headers,
  });

  check(response, { "status is 200": (r) => r.status === 200 });
}

export * from './index.js'