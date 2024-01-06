'use client'

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { TextField, Button, Container, Grid } from '@material-ui/core';
import { Reminder } from '@/app/components/RemindersList';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

export default function Create() {
  const router = useRouter();

  const [reminder, setReminder] = useState<Reminder>({ title: '', description: '', limitDate: '', isDone: false });

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
      handleBack();
    }
  };

  const handleBack = () => {
    router.push('/');
  }

  return (
    <Container>
      <form onSubmit={handleSubmit}>
        <Grid container direction="column" spacing={5}>
          <Grid item>
            <TextField
              label="Title"
              value={reminder?.title}
              onChange={(e) => setReminder((state) => ({ ...state, title: e.target.value }))}
              required
              fullWidth
            />
          </Grid>

          <Grid item>
            <TextField
              label="Description"
              value={reminder?.description}
              onChange={(e) => setReminder((state) => ({ ...state, description: e.target.value }))}
              required
              fullWidth
            />
          </Grid>

          <Grid item>
            <TextField
              label="Limit Date"
              value={reminder?.limitDate}
              onChange={(e) => setReminder((state) => ({ ...state, limitDate: e.target.value }))}
              required
              type='date'
              fullWidth
              InputLabelProps={{ shrink: true, required: true }}
            />
          </Grid>

          <Grid item>
            <Button type="submit" variant="contained" color="primary">
              Create
            </Button>
            <Button variant="contained" onClick={handleBack}>
              Back
            </Button>
          </Grid>
        </Grid>
      </form>
    </Container>
  );
}