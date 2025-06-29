import React from 'react';
import { Alert, Stack } from '@mui/material';

interface Props {
  error?: string;
}

export function AlertError({ error }: Props): React.ReactElement | null {
  if (!error) {
    return null;
  }

  return (
    <Stack sx={{ width: '100%', marginTop: 5 }}>
      <Alert severity="error">{error}</Alert>
    </Stack>
  );
}
