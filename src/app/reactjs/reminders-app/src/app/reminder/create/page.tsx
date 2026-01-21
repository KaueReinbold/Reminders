'use client';

import { useRouter } from 'next/navigation';

import { Button, Container, Stack } from '@mui/material';

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
        <Stack spacing={5}>
          <ReminderForm />

          <Stack direction="row" spacing={2}>
            <Button
              type="submit"
              variant="contained"
              color="success"
              onClick={async () => {
                const status = await onCreateReminder();
                if (status === ReminderActionStatus.Success) handleBack();
              }}
            >
              Create
            </Button>
            <Button variant="contained" color="info" onClick={handleBack}>
              Back
            </Button>
          </Stack>
        </Stack>
      </form>
    </Container>
  );
}
