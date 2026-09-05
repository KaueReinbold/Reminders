'use client';

import { useRef, useState } from 'react';

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

// A rejected mutation is a transport failure, not a validation response, so
// there are no field errors to show. Keep the overlay open and say so.
const REQUEST_FAILED: Errors = {
  request: ['Something went wrong. Please try again.'],
};

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
  const [sheetErrors, setSheetErrors] = useState<Errors>();

  // Deleting from the sheet closes the sheet and the dialog and removes the
  // card that opened them, so there is no trigger left to return focus to.
  // The list itself is the nearest thing still on screen.
  const listRef = useRef<HTMLElement>(null);

  const items = reminders ?? [];
  const groups = groupReminders(filterReminders(items, view, query));

  const refresh = () =>
    queryClient.invalidateQueries({ queryKey: REMINDERS_QUERY_KEY });

  const closeSheet = () => {
    setSheet(null);
    setSheetErrors(undefined);
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
    setSheetErrors(undefined);
    setSheet({});
  };

  const handleEditClick = (id?: string) => {
    setSheetErrors(undefined);
    setSheet({ reminder: items.find(item => item.id === id) });
  };

  const handleSave = async (draft: Reminder) => {
    try {
      const { errors } = draft.id
        ? await updateReminder.mutateAsync(draft)
        : await createReminder.mutateAsync(draft);

      if (errors) {
        setSheetErrors(errors);
        return;
      }
    } catch {
      setSheetErrors(REQUEST_FAILED);
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
        setSheetErrors(errors);
        return;
      }
    } catch {
      setConfirmTarget(null);
      setSheetErrors(REQUEST_FAILED);
      return;
    }

    closeSheet();
    refresh();
    listRef.current?.focus();
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

        <main className={styles.list} ref={listRef} tabIndex={-1}>
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
          errors={sheetErrors}
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
