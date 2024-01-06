'use client'

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { TextField, Button, Container, Grid } from '@mui/material';
import { Errors, Reminder, ValidationError, useCreateReminder } from '@/app/api';

export default function Create() {
  const router = useRouter();

  const createReminder = useCreateReminder();

  const [reminder, setReminder] = useState<Reminder>({ title: '', description: '', limitDate: '', isDone: false });
  const [errors, setErrors] = useState<Errors>()

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      if (reminder) {
        await createReminder.mutateAsync(reminder);
        handleBack();
      }
    } catch (error) {
      if (error instanceof ValidationError) {
        setErrors(error.errors);
      } else {
        console.error(error);
      }
    }
  };

  const handleBack = () => {
    router.push('/');
  }

  return (
    <Container sx={{ margin: 3 }}>
      <form onSubmit={handleSubmit}>
        <Grid container direction="column" spacing={5}>
          <Grid item>
            <TextField
              label="Title"
              value={reminder?.title}
              onChange={(e) => setReminder((state) => ({ ...state, title: e.target.value }))}
              required
              fullWidth
              error={Boolean(errors?.Title)}
              helperText={errors?.Title}
            />
          </Grid>

          <Grid item>
            <TextField
              label="Description"
              value={reminder?.description}
              onChange={(e) => setReminder((state) => ({ ...state, description: e.target.value }))}
              required
              fullWidth
              error={Boolean(errors?.Description)}
              helperText={errors?.Description}
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
              InputLabelProps={{ shrink: true }}
              error={Boolean(errors?.['LimitDate.Date'])}
              helperText={errors?.['LimitDate.Date']}
            />
          </Grid>

          <Grid item>
            <Button type="submit" variant="contained" color="success">
              Create
            </Button>
            <Button variant="contained" color="info" onClick={handleBack}>
              Back
            </Button>
          </Grid>
        </Grid>
      </form>
    </Container>
  );
}