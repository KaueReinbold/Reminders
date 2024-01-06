"use client"

import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Button, CircularProgress } from '@mui/material';
import Link from 'next/link';
import { useReminders } from '@/app/api';
import { Suspense } from 'react';

export default function RemindersList() {
  const { data: reminders, isStale } = useReminders();

  return (
    <Suspense fallback={<CircularProgress />}>
      <Link href="/reminder/create">
        <Button variant="contained" color="primary">
          Create Reminder
        </Button>
      </Link>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Title</TableCell>
              <TableCell>Description</TableCell>
              <TableCell>Limit Date</TableCell>
              <TableCell>Done</TableCell>
              <TableCell></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {reminders?.map((reminder) => (
              <TableRow key={reminder.id}>
                <TableCell>{reminder.id}</TableCell>
                <TableCell>{reminder.title}</TableCell>
                <TableCell>{reminder.description}</TableCell>
                <TableCell>{reminder.limitDateFormatted}</TableCell>
                <TableCell>{reminder.isDoneFormatted}</TableCell>
                <TableCell>
                  <Link href={`/reminder/${reminder.id}`}>
                    <Button variant="contained" color="primary">
                      Edit
                    </Button>
                  </Link></TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Suspense>
  );
}