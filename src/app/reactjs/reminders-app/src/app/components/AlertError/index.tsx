import { Alert, Stack } from '@mui/material';

interface Props {
  error?: string;
}

export function AlertError({ error }: Props) {
  return (
    error && (
      <Stack sx={{ width: '100%', marginTop: 5 }}>
        <Alert severity="error">{error}</Alert>
      </Stack>
    )
  );
}
