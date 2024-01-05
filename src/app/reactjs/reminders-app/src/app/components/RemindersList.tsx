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
  limitDateFormatted?: string;
  isDone: boolean;
  isDoneFormatted?: string;
};

export default function RemindersList() {
  const [reminders, setReminders] = useState<Reminder[]>([]);

  useEffect(() => {
    fetch(`${API_BASE_URL}/api/reminders`)
      .then(response => response.json())
      .then(data => {
        setReminders(data?.map((d: Reminder) => ({
          ...d,
          limitDateFormatted: d.limitDate ? new Date(d.limitDate).toLocaleDateString() : '',
          isDoneFormatted: d.isDone ? 'Yes' : 'No',
        } as Reminder)))
      });
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
              <TableCell>Limit Date</TableCell>
              <TableCell>Done</TableCell>
              <TableCell></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {reminders.map((reminder: Reminder) => (
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
    </>
  );
}