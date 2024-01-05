"use client"

import { useEffect, useState } from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Button } from '@material-ui/core';
import Link from 'next/link';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

export type Reminder = {
  id?: string;
  title: string;
  description: string;
  limitDate: string;
  isDone: boolean;
};

export default function RemindersList() {
  const [reminders, setReminders] = useState<Reminder[]>([]);

  useEffect(() => {
    fetch(`${API_BASE_URL}/api/reminders`)
      .then(response => response.json())
      .then(data => setReminders(data));
  }, []);

  return (
    <>
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
            </TableRow>
          </TableHead>
          <TableBody>
            {reminders.map((reminder: Reminder) => (
              <TableRow key={reminder.id}>
                <TableCell>{reminder.id}</TableCell>
                <TableCell>{reminder.title}</TableCell>
                <TableCell>{reminder.description}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );
}