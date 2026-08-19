import React from 'react';

import { Reminder } from '@/app/api/types';
import { dateLabel, isOverdue, limitDateOnly } from '@/app/util/reminderGroups';

import styles from './index.module.css';

interface Props {
  reminder: Reminder;
  onToggle: (reminder: Reminder) => void;
  onEdit: (id?: string) => void;
}

export function ReminderCard({
  reminder,
  onToggle,
  onEdit,
}: Props): React.ReactElement {
  const overdue = isOverdue(reminder);

  return (
    <article className={styles.card}>
      <button
        type="button"
        aria-label={reminder.isDone ? 'Mark not done' : 'Mark done'}
        className={reminder.isDone ? styles.checkboxDone : styles.checkbox}
        onClick={() => onToggle(reminder)}
      >
        {reminder.isDone && (
          <svg
            width="13"
            height="13"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="3.2"
          >
            <path d="M5 13l4.5 4.5L19 7" />
          </svg>
        )}
      </button>

      <div className={styles.body}>
        <span className={reminder.isDone ? styles.titleDone : styles.title}>
          {reminder.title}
        </span>
        {reminder.description && (
          <span className={styles.description}>{reminder.description}</span>
        )}
      </div>

      <div className={styles.meta}>
        <span className={overdue ? styles.datePillOverdue : styles.datePill}>
          {dateLabel(limitDateOnly(reminder))}
        </span>
        <button
          type="button"
          aria-label="Edit reminder"
          className={styles.editButton}
          onClick={() => onEdit(reminder.id)}
        >
          <svg
            width="15"
            height="15"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.9"
          >
            <path d="M4 20h4L19.5 8.5a2.1 2.1 0 0 0-3-3L5 17v3z" />
          </svg>
        </button>
      </div>
    </article>
  );
}
