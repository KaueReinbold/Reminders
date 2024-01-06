'use client'

import { Suspense, useEffect, useReducer, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { TextField, Button, Container, Grid, Checkbox, FormControlLabel, Modal, Box, Typography, CircularProgress } from '@mui/material';
import { Errors, Reminder, ValidationError, useDeleteReminder, useReminder, useUpdateReminder } from '@/app/api';

const style = {
  modalContent: {
    position: 'absolute' as 'absolute',
    top: '20%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    bgcolor: 'background.paper',
    boxShadow: 24,
    pt: 2,
    px: 4,
    pb: 3,
  },
  modalElement: {
    marginBottom: '2rem'
  }
};

type ReminderAction =
  | { type: 'SET_REMINDER'; payload: Reminder }
  | { type: 'UPDATE_REMINDER'; payload: Partial<Reminder> };

function reminderReducer(state: Reminder | null, action: ReminderAction): Reminder | null {
  switch (action.type) {
    case 'SET_REMINDER':
      return action.payload;
    case 'UPDATE_REMINDER':
      return { ...state, ...action.payload } as Reminder | null;
    default:
      throw new Error();
  }
}

export default function Edit() {
  const { id } = useParams<{ id: string }>()
  const router = useRouter();

  const { data: reminderData } = useReminder(id);
  const updateReminder = useUpdateReminder();
  const deleteReminder = useDeleteReminder();

  const [reminder, dispatch] = useReducer(reminderReducer, null);
  const [openDelete, setOpenDelete] = useState(false);
  const [errors, setErrors] = useState<Errors>()

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      if (reminder) {
        await updateReminder.mutateAsync(reminder);

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

  const handleDelete = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      await deleteReminder.mutateAsync(id);
      handleBack();
    } catch (error) {
      console.error(error);
    }
  };

  const handleOpenDelete = () => {
    setOpenDelete(true);
  };
  const handleCloseDelete = () => {
    setOpenDelete(false);
  };

  const handleBack = () => {
    router.push('/');
  }

  useEffect(() => {
    if (reminderData) {
      dispatch({ type: 'SET_REMINDER', payload: reminderData });
    }
  }, [reminderData]);

  return (
    <Suspense fallback={<CircularProgress />}>
      <Container sx={{ margin: 3 }}>
        <form onSubmit={handleSubmit} noValidate>
          <Grid container direction="column" spacing={5}>
            <Grid item>
              <TextField
                label="Id"
                defaultValue={reminder?.id}
                disabled
                fullWidth
                inputProps={
                  { readOnly: true }
                }
                InputLabelProps={{ shrink: true }}
              />
            </Grid>

            <Grid item>
              <TextField
                label="Title"
                defaultValue={reminder?.title}
                onChange={(e) => dispatch({ type: 'UPDATE_REMINDER', payload: { title: e.target.value } })}
                required
                fullWidth
                InputLabelProps={{ shrink: true }}
                error={Boolean(errors?.Title)}
                helperText={errors?.Title}
              />
            </Grid>

            <Grid item>
              <TextField
                label="Description"
                defaultValue={reminder?.description}
                onChange={(e) => dispatch({ type: 'UPDATE_REMINDER', payload: { description: e.target.value } })}
                required
                fullWidth
                InputLabelProps={{ shrink: true }}
                error={Boolean(errors?.Description)}
                helperText={errors?.Description}
              />
            </Grid>

            <Grid item>
              <TextField
                label="Limit Date"
                defaultValue={reminder?.limitDateFormatted || ''}
                onChange={(e) => dispatch({ type: 'UPDATE_REMINDER', payload: { limitDate: e.target.value } })}
                required
                fullWidth
                type='date'
                InputLabelProps={{ shrink: true }}
                error={Boolean(errors?.['LimitDate.Date'])}
                helperText={errors?.['LimitDate.Date']}
              />
            </Grid>

            <Grid item>
              <FormControlLabel
                label="Done"
                control={
                  <Checkbox
                    checked={reminder?.isDone ?? false}
                    onChange={(e) => dispatch({ type: 'UPDATE_REMINDER', payload: { isDone: e.target.checked } })}
                  />
                }
              />
            </Grid>

            <Grid item>
              <Button type="submit" variant="contained" color="success">
                Edit
              </Button>
              <Button variant="contained" color="error" onClick={handleOpenDelete}>
                Delete
              </Button>
              <Button variant="contained" color="info" onClick={handleBack}>
                Back
              </Button>
            </Grid>
          </Grid>
        </form>

        <Modal
          open={openDelete}
          onClose={handleCloseDelete}
          aria-labelledby="child-modal-title"
          aria-describedby="child-modal-description"
        >
          <Box sx={{ ...style.modalContent }}>
            <Box sx={{ ...style.modalElement }}>
              <Typography variant="h4">
                Delete reminder
              </Typography>
            </Box>

            <Box sx={{ ...style.modalElement }}>
              <Typography variant="body1">
                Are you sure you want to delete this reminder?
              </Typography>
            </Box>

            <Button variant="contained" color="error" onClick={handleDelete}>Delete</Button>

            <Button variant="contained" color="info" onClick={handleCloseDelete}>Close</Button>
          </Box>
        </Modal>
      </Container>
    </Suspense>
  );
}