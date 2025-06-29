'use client';

import { Button, Container, Typography, Box } from '@mui/material';
import { useRouter } from 'next/navigation';

export default function NotFoundReminder() {
  const router = useRouter();

  const handleBackToHome = () => {
    router.push('/');
  };

  return (
    <Container sx={{ margin: 3, textAlign: 'center' }}>
      <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 3 }}>
        <Typography variant="h4" color="error">
          404 - Reminder Not Found
        </Typography>
        <Typography variant="body1" color="text.secondary">
          The reminder you&apos;re looking for doesn&apos;t exist or may have been deleted.
        </Typography>
        <Button 
          variant="contained" 
          color="primary" 
          onClick={handleBackToHome}
          size="large"
        >
          Back to Home
        </Button>
      </Box>
    </Container>
  );
}