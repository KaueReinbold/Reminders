'use client';

import React from 'react';

import { useEscapeKey } from '@/app/hooks/useEscapeKey';
import { useReturnFocus } from '@/app/hooks/useReturnFocus';

import styles from './index.module.css';

interface Props {
  openDelete: boolean;
  reminderTitle?: string;
  toggleOpenDelete: () => void;
  onDelete: () => void;
}

export function ReminderDeleteModal({
  openDelete,
  reminderTitle,
  toggleOpenDelete,
  onDelete,
}: Props): React.ReactElement | null {
  useEscapeKey(toggleOpenDelete, openDelete);
  useReturnFocus(openDelete);

  if (!openDelete) {
    return null;
  }

  return (
    <div
      className={styles.scrim}
      onClick={toggleOpenDelete}
      data-testid="delete-modal-scrim"
    >
      <div
        role="dialog"
        aria-modal="true"
        aria-label="Delete this reminder?"
        className={styles.dialog}
        onClick={event => event.stopPropagation()}
      >
        <span className={styles.title}>Delete this reminder?</span>
        <span className={styles.body}>
          {reminderTitle
            ? `“${reminderTitle}” will be removed permanently.`
            : 'This reminder will be removed permanently.'}
        </span>

        <div className={styles.actions}>
          <button
            type="button"
            className={styles.deleteButton}
            onClick={onDelete}
            data-testid="delete-button"
          >
            Delete
          </button>
          <button
            type="button"
            className={styles.keepButton}
            onClick={toggleOpenDelete}
            data-testid="close-button"
            autoFocus
          >
            Keep it
          </button>
        </div>
      </div>
    </div>
  );
}
