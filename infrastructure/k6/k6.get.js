import http from 'k6/http';
import { sleep } from 'k6';

export default function () {
  let url = 'http://localhost:5001/api/Reminders';
  let headers = {
    'content-type': 'application/json',
  };
  http.get(url, {
    headers: headers,
  });
  sleep(3);
}
