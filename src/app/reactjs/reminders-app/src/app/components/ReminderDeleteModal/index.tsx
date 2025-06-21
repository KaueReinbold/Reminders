import { Box, Button, Modal, Typography } from '@mui/material';

const style = {
  modalContent: {
    position: 'absolute',
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
    marginBottom: '2rem',
  },
};

interface Props {
  openDelete: boolean;
  toggleOpenDelete: () => void;
  onDelete: (e: React.FormEvent) => void;
}

export function ReminderDeleteModal({
  openDelete,
  toggleOpenDelete,
  onDelete,
}: Props): JSX.Element {
  return (
    <Modal
      open={openDelete}
      onClose={toggleOpenDelete}
      aria-labelledby="child-modal-title"
      aria-describedby="child-modal-description"
    >
      <Box sx={{ ...style.modalContent }}>
        <Box sx={{ ...style.modalElement }}>
          <Typography variant="h4">Delete reminder</Typography>
        </Box>

        <Box sx={{ ...style.modalElement }}>
          <Typography variant="body1">
            Are you sure you want to delete this reminder?
          </Typography>
        </Box>

        <Button variant="contained" color="error" onClick={onDelete} data-testid="delete-button">
          Delete
        </Button>

        <Button variant="contained" color="info" onClick={toggleOpenDelete} data-testid="close-button">
          Close
        </Button>
      </Box>
    </Modal>
  );
}
