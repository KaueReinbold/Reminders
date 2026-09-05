'use client';

import { useState } from 'react';

import {
  Errors,
  REMINDERS_QUERY_KEY,
  Reminder,
  useCreateReminder,
  useDeleteReminder,
  useReminders,
  useUpdateReminder,
} from '@/app/api';
import {
  AppHeader,
  ReminderCard,
  ReminderDeleteModal,
  ReminderSheet,
  Sidebar,
} from '@/app/components';
import { useRemindersQueryClient } from '@/app/hooks';
import {
  ViewName,
  filterReminders,
  groupReminders,
  viewCounts,
  weekProgress,
} from '@/app/util/reminderGroups';

import styles from './list.module.css';

const errorMessage = (errors: Errors): string =>
  Object.values(errors).flat().join(' ');

// A rejected mutation is a transport failure, not a validation response, so
// there are no field errors to show. Keep the overlay open and say so.
const REQUEST_FAILED = 'Something went wrong. Please try again.';

export default function RemindersList() {
  const { data: reminders } = useReminders();
  const queryClient = useRemindersQueryClient();
  const createReminder = useCreateReminder();
  const updateReminder = useUpdateReminder();
  const deleteReminder = useDeleteReminder();

  const [query, setQuery] = useState('');
  const [view, setView] = useState<ViewName>('All');

  // `sheet` holds the create/edit modal state: null when closed, an object
  // carrying the reminder being edited (absent on create).
  const [sheet, setSheet] = useState<{ reminder?: Reminder } | null>(null);
  const [confirmTarget, setConfirmTarget] = useState<Reminder | null>(null);
  const [sheetError, setSheetError] = useState<string>();

  const items = reminders ?? [];
  const groups = groupReminders(filterReminders(items, view, query));

  const refresh = () =>
    queryClient.invalidateQueries({ queryKey: REMINDERS_QUERY_KEY });

  const closeSheet = () => {
    setSheet(null);
    setSheetError(undefined);
  };

  // Escape and scrim clicks reach both overlays: the confirmation on top of
  // the sheet closes first.
  const dismissSheet = () => {
    if (confirmTarget) {
      setConfirmTarget(null);
      return;
    }

    closeSheet();
  };

  const handleCreateClick = () => {
    setSheetError(undefined);
    setSheet({});
  };

  const handleEditClick = (id?: string) => {
    setSheetError(undefined);
    setSheet({ reminder: items.find(item => item.id === id) });
  };

  const handleSave = async (draft: Reminder) => {
    try {
      const { errors } = draft.id
        ? await updateReminder.mutateAsync(draft)
        : await createReminder.mutateAsync(draft);

      if (errors) {
        setSheetError(errorMessage(errors));
        return;
      }
    } catch {
      setSheetError(REQUEST_FAILED);
      return;
    }

    closeSheet();
    refresh();
  };

  const handleConfirmDelete = async () => {
    if (!confirmTarget?.id) return;

    try {
      const { errors } = await deleteReminder.mutateAsync(confirmTarget.id);

      setConfirmTarget(null);

      if (errors) {
        setSheetError(errorMessage(errors));
        return;
      }
    } catch {
      setConfirmTarget(null);
      setSheetError(REQUEST_FAILED);
      return;
    }

    closeSheet();
    refresh();
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

      {sheet && (
        <ReminderSheet
          reminder={sheet.reminder}
          error={sheetError}
          onClose={dismissSheet}
          onSave={handleSave}
          onDelete={
            sheet.reminder
              ? () => setConfirmTarget(sheet.reminder ?? null)
              : undefined
          }
        />
      )}

      <ReminderDeleteModal
        openDelete={Boolean(confirmTarget)}
        reminderTitle={confirmTarget?.title}
        toggleOpenDelete={() => setConfirmTarget(null)}
        onDelete={handleConfirmDelete}
      />
    </>
  );
}
