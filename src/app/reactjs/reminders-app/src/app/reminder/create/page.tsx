'use client'

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { TextField, Button, Container, Typography } from '@material-ui/core';
import { Reminder } from '@/components/RemindersList';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

export type APIError = {
  type: string,
  title: string,
  status: number,
  errors: Errors,
  traceId: string,
}

export type Errors = {
  "LimitDate.Date": string[],
}


export default function Create() {
  const [reminder, setReminder] = useState<Reminder>({ title: '', description: '', limitDate: '2025-01-05T00:50:06.416Z', isDone: false });

  const [errors, setErrors] = useState<Errors>();

  const router = useRouter();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const body = JSON.stringify(reminder);
    const response = await fetch(`${API_BASE_URL}/api/reminders`, {
      method: 'POST',

      headers: {
        'Content-Type': 'application/json',
      },
      body,
    });
    const data = await response.json();

    if (response.ok) {
      router.back();
    } else {
      setErrors(data?.errors);
    }
  };

  return (
    <Container>
      <form onSubmit={handleSubmit}>
        <TextField
          label="Title"
          value={reminder?.title}
          onChange={(e) => setReminder(state => ({ ...state, title: e.target.value }))}
          required
        />
        <TextField
          label="Description"
          value={reminder?.description}
          onChange={(e) => setReminder(state => ({ ...state, description: e.target.value }))}
          required
        />
        <Button type="submit" variant="contained" color="primary">
          Create
        </Button>
      </form>
    </Container>
  );
}