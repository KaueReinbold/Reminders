'use client'

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { TextField, Button, Container, Grid, Checkbox, FormControlLabel, Modal, Box, Typography } from '@material-ui/core';

import { Reminder } from '@/app/components/RemindersList';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

export function formatDate(dateString: string) {
  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = ('0' + (date.getMonth() + 1)).slice(-2); // Months are 0-indexed in JavaScript
  const day = ('0' + date.getDate()).slice(-2);
  return `${year}-${month}-${day}`;
}

const style = {
  modalContent: {
    position: 'absolute' as 'absolute',
    top: '20%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    bgcolor: 'background.paper',
    borderRadius: 20,
    boxShadow: 24,
    pt: 2,
    px: 4,
    pb: 3,
  },
  modalElement: {
    marginBottom: '2rem'
  }
};


export default function Edit() {
  const { id } = useParams<{ id: string }>()
  const router = useRouter();

  const [reminder, setReminder] = useState<Reminder>({ id, title: '', description: '', limitDate: '', isDone: false });
  const [openDelete, setOpenDelete] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const body = JSON.stringify(reminder);
    const response = await fetch(`${API_BASE_URL}/api/reminders/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body,
    });

    if (response.ok) {
      handleBack();
    }
  };

  const handleDelete = async (e: React.FormEvent) => {
    e.preventDefault();

    const response = await fetch(`${API_BASE_URL}/api/reminders/${id}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (response.ok) {
      handleBack();
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
    fetch(`${API_BASE_URL}/api/reminders/${id}`)
      .then(response => response.json())
      .then(data => {
        setReminder(({
          ...data,
          limitDateFormatted: formatDate(data?.limitDate),
          isDoneFormatted: data.isDone ? 'Yes' : 'No',
        } as Reminder));
      });
  }, []);


  return (
    <Container>
      <form onSubmit={handleSubmit}>
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
              onChange={(e) => setReminder(state => ({ ...state, title: e.target.value }))}
              required
              fullWidth
              InputLabelProps={{ shrink: true, required: true }}
            />
          </Grid>

          <Grid item>
            <TextField
              label="Description"
              defaultValue={reminder?.description}
              onChange={(e) => setReminder(state => ({ ...state, description: e.target.value }))}
              required
              fullWidth
              InputLabelProps={{ shrink: true, required: true }}
            />
          </Grid>

          <Grid item>
            <TextField
              label="Limit Date"
              defaultValue={reminder?.limitDateFormatted}
              onChange={(e) => setReminder(state => ({ ...state, limitDate: e.target.value }))}
              required
              type='date'
              fullWidth
              InputLabelProps={{ shrink: true, required: true }}
            />
          </Grid>

          <Grid item>
            <FormControlLabel
              label="Done"
              control={
                <Checkbox
                  defaultChecked={reminder?.isDone}
                  onChange={(e) => setReminder(state => ({ ...state, isDone: e.target.checked }))}
                  required
                />
              }
            />
          </Grid>

          <Grid item>
            <Button type="submit" variant="contained" color="primary">
              Edit
            </Button>
            <Button variant="contained" color="secondary" onClick={handleOpenDelete}>
              Delete
            </Button>
            <Button variant="contained" onClick={handleBack}>
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

          <Button variant="contained" color="secondary" onClick={handleDelete}>Delete</Button>

          <Button variant="contained" onClick={handleCloseDelete}>Close</Button>
        </Box>
      </Modal>
    </Container>
  );
}