"use client"

import { useEffect, useState } from 'react';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

type Reminder = {
    id: number;
    title: string;
    description: string;
};

export default function RemindersList() {
    const [reminders, setReminders] = useState<Reminder[]>([]);

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/reminders`)
            .then(response => response.json())
            .then(data => setReminders(data));
    }, []);

    return (
        <div>
            {reminders.map((reminder: Reminder) => (
                <div key={reminder.id}>
                    <h2>{reminder.title}</h2>
                    <p>{reminder.description}</p>
                </div>
            ))}
        </div>
    );
}