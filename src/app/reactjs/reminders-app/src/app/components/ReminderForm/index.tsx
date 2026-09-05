import { Box, Checkbox, FormControlLabel, TextField } from '@mui/material';
import React, { useState } from 'react';
import type { InputHTMLAttributes } from 'react';
import { AlertError } from '..';
import { fieldError, unmappedError } from '@/app/api/errors';
import { useRemindersContext } from '@/app/hooks';

interface Props {
  editing?: boolean;
}

export function ReminderForm({ editing = false }: Props) {
  const { reminder, errors, dispatch, clearFieldError } = useRemindersContext();
  const [localErrors, setLocalErrors] = useState<Record<string, string>>({});

  // Local form state to allow immediate input feedback in tests
  const [formState, setFormState] = useState({
    title: reminder?.title ?? '',
    description: reminder?.description ?? '',
    limitDate: reminder?.limitDate ?? reminder?.limitDateFormatted ?? '',
    isDone: reminder?.isDone ?? false,
  });

  // Sync local form state when context reminder changes
  React.useEffect(() => {
    // Normalize ISO limitDate (e.g. 2026-01-22T00:00:00+00:00) to YYYY-MM-DD
    const formatLimitDate = (d?: string | null) => {
      if (!d) return '';
      // If already in YYYY-MM-DD form, return directly
      if (/^\d{4}-\d{2}-\d{2}$/.test(d)) return d;
      const date = new Date(d);
      if (Number.isNaN(date.getTime())) return '';
      const year = date.getUTCFullYear();
      const month = ('0' + (date.getUTCMonth() + 1)).slice(-2);
      const day = ('0' + date.getUTCDate()).slice(-2);
      return `${year}-${month}-${day}`;
    };

    setFormState({
      title: reminder?.title ?? '',
      description: reminder?.description ?? '',
      limitDate: reminder?.limitDateFormatted
        ? reminder.limitDateFormatted
        : formatLimitDate(reminder?.limitDate ?? ''),
      isDone: reminder?.isDone ?? false,
    });
  }, [reminder]);

  const handleChange = (key: string, value: string | boolean) => {
    // update local form state for immediate feedback
    setFormState(prev => ({ ...prev, [key]: value } as any));

    dispatch({ type: 'UPDATE_REMINDER', payload: { [key]: value } });

    // Error keys are the JSON field names (ADR-0011), so the form key is the
    // error key. Clear the field error when the user edits it.
    if (typeof clearFieldError === 'function') clearFieldError(key);

    // When there is no context-provided `errors` (component used in isolation in tests),
    // apply local validation so the component remains self-contained.
    if (!errors) {
      const emptyMessages: Record<string, string> = {
        title: 'Title cannot be empty',
        description: 'Description cannot be empty',
        limitDate: 'Limit Date cannot be empty',
      };

      if (emptyMessages[key]) {
        setLocalErrors(prev => {
          const next = { ...prev };

          if (!value || String(value).trim() === '') {
            next[key] = emptyMessages[key];
          } else {
            delete next[key];
          }

          return next;
        });
      }
    }
  };

  return (
    <>
      <AlertError error={unmappedError(errors)} />

      {editing && (
        <Box sx={{ mb: 2 }}>
          <TextField
            label="Id"
            value={reminder?.id || ''}
            disabled
            fullWidth
            InputLabelProps={{ shrink: true }}
            inputProps={{ readOnly: true, 'data-testid': 'reminderId' }}
          />
        </Box>
      )}

      <Box sx={{ mb: 2 }}>
        <TextField
          label="Title"
          placeholder="Enter title"
          value={formState.title}
          onChange={e => handleChange('title', e.target.value)}
          required
          fullWidth
          error={Boolean(fieldError(errors, 'title') ?? localErrors.title)}
          helperText={fieldError(errors, 'title') ?? localErrors.title}
          InputLabelProps={{ shrink: true }}
          inputProps={{ 'data-testid': 'title' }}
        />
      </Box>

      <Box sx={{ mb: 2 }}>
        <TextField
          label="Description"
          placeholder="Enter description"
          value={formState.description}
          onChange={e => handleChange('description', e.target.value)}
          required
          fullWidth
          InputLabelProps={{ shrink: true }}
          error={Boolean(fieldError(errors, 'description') ?? localErrors.description)}
          helperText={fieldError(errors, 'description') ?? localErrors.description}
          inputProps={{ 'data-testid': 'description' }}
        />
      </Box>

      <Box sx={{ mb: 2 }}>
        <TextField
          label="Limit Date"
          placeholder="Enter limit date"
          value={formState.limitDate}
          onChange={e => handleChange('limitDate', e.target.value)}
          required
          type="date"
          fullWidth
          InputLabelProps={{ shrink: true }}
          error={Boolean(fieldError(errors, 'limitDate') ?? localErrors.limitDate)}
          helperText={fieldError(errors, 'limitDate') ?? localErrors.limitDate}
          inputProps={{ 'data-testid': 'limitDate' }}
        />
      </Box>

      {editing && (
        <Box sx={{ mb: 2 }}>
          <FormControlLabel
            label="Done"
            control={
              <Checkbox
                checked={formState.isDone}
                onChange={e => handleChange('isDone', e.target.checked)}
                inputProps={{ 'data-testid': 'isDone' } as InputHTMLAttributes<HTMLInputElement>}
              />
            }
          />
        </Box>
      )}
    </>
  );
}
