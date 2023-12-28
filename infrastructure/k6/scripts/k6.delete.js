import http from 'k6/http';
import { check } from 'k6';

import { baseUrl } from './index.js';

export default function () {
  const url = `${baseUrl}/api/Reminders`;
  const headers = {
    'content-type': 'application/json',
  };

  let response = http.get(url, {
    headers,
  });

  const reminders = response.json();
  
  for (const reminder of reminders) {
    response = http.del(`${url}/${reminder.id}`);

    check(response, { "status is 200": (r) => r.status === 200 });
  }
}

export * from './index.js'