import http from 'k6/http';
import { sleep } from 'k6';

export default function () {
  let url = 'http://localhost:5001/api/Reminders';
  let headers = {
    'content-type': 'application/json',
  };
  let data = {
    title: 'Title',
    description: 'Description',
    isDone: false,
    limitDate: new Date(2021, 1, 1),
  };
  http.post(url, JSON.stringify(data), {
    headers: headers,
  });
  sleep(3);
}
