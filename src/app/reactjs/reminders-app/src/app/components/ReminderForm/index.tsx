import { Box, Checkbox, FormControlLabel, TextField } from '@mui/material';
import React, { useState } from 'react';
import type { InputHTMLAttributes } from 'react';
import { AlertError } from '..';
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

    // Clear client-side error for the field when user edits it
    let errorKey: string | undefined;
    if (key === 'title') errorKey = 'Title';
    else if (key === 'description') errorKey = 'Description';
    else if (key === 'limitDate') errorKey = 'LimitDate.Date';

    if (errorKey && typeof clearFieldError === 'function') clearFieldError(errorKey);
    // When there is no context-provided `errors` (component used in isolation in tests),
    // apply local validation so the component remains self-contained.
    if (!errors) {
      if (key === 'title') {
        if (!value || String(value).trim() === '') {
          setLocalErrors(prev => ({ ...prev, Title: 'Title cannot be empty' }));
        } else {
          setLocalErrors(prev => { const c = { ...prev }; delete c.Title; return c; });
        }
      }

      if (key === 'description') {
        if (!value || String(value).trim() === '') {
          setLocalErrors(prev => ({ ...prev, Description: 'Description cannot be empty' }));
        } else {
          setLocalErrors(prev => { const c = { ...prev }; delete c.Description; return c; });
        }
      }

      if (key === 'limitDate') {
        if (!value || String(value).trim() === '') {
          setLocalErrors(prev => ({ ...prev, 'LimitDate.Date': 'Limit Date cannot be empty' }));
        } else {
          setLocalErrors(prev => { const c = { ...prev }; delete c['LimitDate.Date']; return c; });
        }
      }
    }
  };

  return (
    <>
      <AlertError error={errors?.InternalServer} />

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
          error={Boolean(errors?.Title ?? localErrors.Title)}
          helperText={errors?.Title ?? localErrors.Title}
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
          error={Boolean(errors?.Description ?? localErrors.Description)}
          helperText={errors?.Description ?? localErrors.Description}
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
          error={Boolean(errors?.['LimitDate.Date'] ?? localErrors['LimitDate.Date'])}
          helperText={errors?.['LimitDate.Date'] ?? localErrors['LimitDate.Date']}
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
