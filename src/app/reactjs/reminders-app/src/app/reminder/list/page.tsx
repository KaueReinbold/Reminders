'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';

import {
  REMINDERS_QUERY_KEY,
  Reminder,
  useReminders,
  useUpdateReminder,
} from '@/app/api';
import { AppHeader, ReminderCard, Sidebar } from '@/app/components';
import {
  useRemindersClearContext,
  useRemindersQueryClient,
} from '@/app/hooks';
import {
  ViewName,
  groupReminders,
  viewCounts,
  weekProgress,
} from '@/app/util/reminderGroups';

import styles from './list.module.css';

export default function RemindersList() {
  const router = useRouter();

  const { data: reminders } = useReminders();
  const clearReminder = useRemindersClearContext();
  const queryClient = useRemindersQueryClient();
  const updateReminder = useUpdateReminder();

  // Search and view filtering of the list itself lands with the filters
  // issue; the shell only renders the controls and active states.
  const [query, setQuery] = useState('');
  const [view, setView] = useState<ViewName>('All');

  const handleCreateClick = () => {
    clearReminder();
    router.push('/reminder/create');
  };

  const handleEditClick = (id?: string) => {
    clearReminder();
    router.push(`/reminder/edit?id=${id}`);
  };

  const handleToggle = async (reminder: Reminder) => {
    const toggled = { ...reminder, isDone: !reminder.isDone };
    const previous = queryClient.getQueryData<Reminder[]>(REMINDERS_QUERY_KEY);

    queryClient.setQueryData<Reminder[]>(REMINDERS_QUERY_KEY, current =>
      current?.map(item => (item.id === reminder.id ? toggled : item)),
    );

    const { errors } = await updateReminder.mutateAsync(toggled);

    if (errors) {
      queryClient.setQueryData(REMINDERS_QUERY_KEY, previous);
    }
  };

  const items = reminders ?? [];
  const groups = groupReminders(items);

  return (
    <>
      <AppHeader
        query={query}
        onQueryChange={setQuery}
        onCreate={handleCreateClick}
      />

      <div className={styles.body}>
        <Sidebar
          view={view}
          counts={viewCounts(items)}
          onSelectView={setView}
          progress={weekProgress(items)}
        />

        <main className={styles.list}>
          {groups.map(group => (
            <section key={group.label} className={styles.section}>
              <div className={styles.sectionHeader}>
                <h2 className={styles.sectionTitle}>{group.label}</h2>
                <span className={styles.sectionCount}>
                  {group.items.length}
                </span>
                <span className={styles.sectionRule} />
              </div>

              {group.items.map(reminder => (
                <ReminderCard
                  key={reminder.id}
                  reminder={reminder}
                  onToggle={handleToggle}
                  onEdit={handleEditClick}
                />
              ))}
            </section>
          ))}
        </main>
      </div>
    </>
  );
}
