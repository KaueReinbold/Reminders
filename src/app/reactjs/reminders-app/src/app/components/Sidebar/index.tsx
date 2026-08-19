import React from 'react';

import {
  VIEW_ORDER,
  ViewName,
  WeekProgress,
} from '@/app/util/reminderGroups';

import styles from './index.module.css';

interface Props {
  view: ViewName;
  counts: Record<ViewName, number>;
  onSelectView: (view: ViewName) => void;
  progress: WeekProgress;
}

export function Sidebar({
  view,
  counts,
  onSelectView,
  progress,
}: Props): React.ReactElement {
  return (
    <aside className={styles.sidebar}>
      <nav className={styles.nav}>
        {VIEW_ORDER.map(name => {
          const active = name === view;
          return (
            <button
              key={name}
              type="button"
              aria-current={active || undefined}
              className={active ? styles.navItemActive : styles.navItem}
              onClick={() => onSelectView(name)}
            >
              <span className={active ? styles.dotActive : styles.dot} />
              <span className={styles.navLabel}>{name}</span>
              <span className={styles.navCount}>{counts[name]}</span>
            </button>
          );
        })}
      </nav>

      <div className={styles.progress}>
        <div className={styles.progressLabel}>This week</div>
        <div className={styles.progressStat}>{progress.pct}%</div>
        <div className={styles.progressTrack}>
          <div
            className={styles.progressFill}
            style={{ width: `${progress.pct}%` }}
          />
        </div>
        <div className={styles.progressCaption}>{progress.caption}</div>
      </div>
    </aside>
  );
}
