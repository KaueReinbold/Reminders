'use client';

import React, { useState } from 'react';

import { Reminder } from '@/app/api/types';
import { useEscapeKey } from '@/app/hooks/useEscapeKey';
import { limitDateOnly } from '@/app/util/reminderGroups';

import styles from './index.module.css';

interface Props {
  reminder?: Reminder;
  error?: string;
  onClose: () => void;
  onSave: (reminder: Reminder) => void;
  onDelete?: () => void;
}

// New reminders default to tomorrow, matching the design prototype.
const tomorrow = (): string => {
  const date = new Date();
  date.setDate(date.getDate() + 1);
  const month = ('0' + (date.getMonth() + 1)).slice(-2);
  const day = ('0' + date.getDate()).slice(-2);
  return `${date.getFullYear()}-${month}-${day}`;
};

export function ReminderSheet({
  reminder,
  error,
  onClose,
  onSave,
  onDelete,
}: Props): React.ReactElement {
  const editing = Boolean(reminder);

  const [draft, setDraft] = useState<Reminder>({
    ...reminder,
    title: reminder?.title ?? '',
    description: reminder?.description ?? '',
    limitDate: reminder ? limitDateOnly(reminder) : tomorrow(),
    isDone: reminder?.isDone ?? false,
  });

  useEscapeKey(onClose);

  const update = (values: Partial<Reminder>) =>
    setDraft(current => ({ ...current, ...values }));

  return (
    <div
      className={styles.scrim}
      onClick={onClose}
      data-testid="reminder-sheet-scrim"
    >
      <div
        role="dialog"
        aria-modal="true"
        aria-label={editing ? 'Edit reminder' : 'New reminder'}
        className={styles.sheet}
        onClick={event => event.stopPropagation()}
      >
        <div className={styles.header}>
          <span className={styles.title}>
            {editing ? 'Edit reminder' : 'New reminder'}
          </span>
          <button
            type="button"
            className={styles.closeButton}
            onClick={onClose}
          >
            Close
          </button>
        </div>

        <div className={styles.fields}>
          <label className={styles.field}>
            <span className={styles.label}>Title</span>
            <input
              type="text"
              className={styles.input}
              placeholder="Call the notary"
              value={draft.title}
              onChange={event => update({ title: event.target.value })}
              data-testid="title"
            />
          </label>

          <label className={styles.field}>
            <span className={styles.label}>Description</span>
            <textarea
              rows={3}
              className={styles.textarea}
              placeholder="Optional detail"
              value={draft.description}
              onChange={event => update({ description: event.target.value })}
              data-testid="description"
            />
          </label>

          <div className={styles.row}>
            <label className={styles.rowField}>
              <span className={styles.label}>Limit date</span>
              <input
                type="date"
                className={styles.dateInput}
                value={draft.limitDate}
                onChange={event => update({ limitDate: event.target.value })}
                data-testid="limitDate"
              />
            </label>

            <div className={styles.rowField}>
              <span className={styles.label}>Status</span>
              <button
                type="button"
                className={styles.statusButton}
                aria-pressed={draft.isDone}
                onClick={() => update({ isDone: !draft.isDone })}
                data-testid="isDone"
              >
                <span className={styles.statusCheck}>
                  {draft.isDone && (
                    <svg
                      width="12"
                      height="12"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="3.2"
                    >
                      <path d="M5 13l4.5 4.5L19 7" />
                    </svg>
                  )}
                </span>
                <span>{draft.isDone ? 'Done' : 'Not done yet'}</span>
              </button>
            </div>
          </div>
        </div>

        {error && <p className={styles.error}>{error}</p>}

        <div className={styles.footer}>
          <button
            type="button"
            className={styles.saveButton}
            disabled={!draft.title.trim()}
            onClick={() => onSave(draft)}
            data-testid="save-button"
          >
            {editing ? 'Save changes' : 'Create reminder'}
          </button>

          <button
            type="button"
            className={styles.cancelButton}
            onClick={onClose}
          >
            Cancel
          </button>

          {editing && onDelete && (
            <button
              type="button"
              className={styles.deleteButton}
              onClick={onDelete}
            >
              Delete reminder
            </button>
          )}
        </div>
      </div>
    </div>
  );
}
