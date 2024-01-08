'use client';

import { useRouter } from 'next/navigation';

import { Button, Container, Grid } from '@mui/material';

import { ReminderForm } from '@/app/components';
import { ReminderActionStatus, useRemindersContext } from '@/app/hooks';

export default function Create() {
  const router = useRouter();

  const { onCreateReminder } = useRemindersContext();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const status = await onCreateReminder();

    if (status === ReminderActionStatus.Success) {
      handleBack();
    }
  };

  const handleBack = () => {
    router.push('/');
  };

  return (
    <Container sx={{ margin: 3 }}>
      <form onSubmit={handleSubmit}>
        <Grid container direction="column" spacing={5}>
          <ReminderForm />

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
