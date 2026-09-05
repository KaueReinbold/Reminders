import React from 'react';

import { IS_MOCK_API } from '@/app/api';

import styles from './index.module.css';

interface Props {
  query: string;
  onQueryChange: (query: string) => void;
  onCreate: () => void;
}

const todayLabel = (): string =>
  new Date().toLocaleDateString('en-US', {
    weekday: 'long',
    month: 'long',
    day: 'numeric',
  });

export function AppHeader({
  query,
  onQueryChange,
  onCreate,
}: Props): React.ReactElement {
  return (
    <header className={styles.header}>
      <div className={styles.titleGroup}>
        <span className={styles.title}>Reminders</span>
        <span className={styles.date}>{todayLabel()}</span>
        {IS_MOCK_API && (
          <span
            className={styles.demoBadge}
            title="No backend: data lives in your browser and resets on reload"
          >
            Demo data
          </span>
        )}
      </div>

      <div className={styles.searchPill}>
        <svg
          width="15"
          height="15"
          viewBox="0 0 24 24"
          fill="none"
          stroke="#8c8577"
          strokeWidth="2.2"
          className={styles.searchIcon}
        >
          <circle cx="11" cy="11" r="7" />
          <path d="M20 20l-4.2-4.2" />
        </svg>
        <input
          type="text"
          placeholder="Search reminders"
          value={query}
          onChange={event => onQueryChange(event.target.value)}
          className={styles.searchInput}
        />
      </div>

      <button type="button" className={styles.createButton} onClick={onCreate}>
        <svg
          width="15"
          height="15"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2.4"
        >
          <path d="M12 5v14M5 12h14" />
        </svg>
        New reminder
      </button>
    </header>
  );
}
