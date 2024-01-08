'use client';

import { Suspense, useState } from 'react';
import { useRouter } from 'next/navigation';

import { Button, Container, Grid, CircularProgress } from '@mui/material';

import { ReminderDeleteModal, ReminderForm } from '@/app/components';
import { ReminderActionStatus, useRemindersContext } from '@/app/hooks';

export default function Edit() {
  const router = useRouter();

  const { onUpdateReminder, onDeleteReminder } = useRemindersContext();

  const [openDelete, setOpenDelete] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const status = await onUpdateReminder();

    if (status === ReminderActionStatus.Success) {
      handleBack();
    }
  };

  const handleDelete = async (e: React.FormEvent) => {
    e.preventDefault();

    const status = await onDeleteReminder();

    if (status === ReminderActionStatus.Success) {
      handleBack();
    }
  };

  const toggleOpenDelete = () => {
    setOpenDelete(state => !state);
  };

  const handleBack = () => {
    router.push('/');
  };

  return (
    <Suspense fallback={<CircularProgress />}>
      <Container sx={{ margin: 3 }}>
        <form onSubmit={handleSubmit} noValidate>
          <Grid container direction="column" spacing={5}>
            <ReminderForm editing />

            <Grid item>
              <Button type="submit" variant="contained" color="success">
                Edit
              </Button>
              <Button
                variant="contained"
                color="error"
                onClick={toggleOpenDelete}
              >
                Delete
              </Button>
              <Button variant="contained" color="info" onClick={handleBack}>
                Back
              </Button>
            </Grid>
          </Grid>
        </form>

        <ReminderDeleteModal
          openDelete={openDelete}
          toggleOpenDelete={toggleOpenDelete}
          onDelete={handleDelete}
        />
      </Container>
    </Suspense>
  );
}
