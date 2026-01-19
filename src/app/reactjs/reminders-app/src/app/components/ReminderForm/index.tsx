import { Box, Checkbox, FormControlLabel, TextField } from '@mui/material';
import type { InputHTMLAttributes } from 'react';
import { AlertError } from '..';
import { useRemindersContext } from '@/app/hooks';

interface Props {
  editing?: boolean;
}

export function ReminderForm({ editing = false }: Props) {
  const { reminder, errors, dispatch } = useRemindersContext();

  const handleChange = (key: string, value: string | boolean) => {
    dispatch({ type: 'UPDATE_REMINDER', payload: { [key]: value } });
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
          value={reminder?.title || ''}
          onChange={e => handleChange('title', e.target.value)}
          required
          fullWidth
          error={Boolean(errors?.Title)}
          helperText={errors?.Title}
          InputLabelProps={{ shrink: true }}
          inputProps={{ 'data-testid': 'title' }}
        />
      </Box>

      <Box sx={{ mb: 2 }}>
        <TextField
          label="Description"
          placeholder="Enter description"
          value={reminder?.description || ''}
          onChange={e => handleChange('description', e.target.value)}
          required
          fullWidth
          InputLabelProps={{ shrink: true }}
          error={Boolean(errors?.Description)}
          helperText={errors?.Description}
          inputProps={{ 'data-testid': 'description' }}
        />
      </Box>

      <Box sx={{ mb: 2 }}>
        <TextField
          label="Limit Date"
          placeholder="Enter limit date"
          value={reminder?.limitDate || reminder?.limitDateFormatted || ''}
          onChange={e => handleChange('limitDate', e.target.value)}
          required
          type="date"
          fullWidth
          InputLabelProps={{ shrink: true }}
          error={Boolean(errors?.['LimitDate.Date'])}
          helperText={errors?.['LimitDate.Date']}
          inputProps={{ 'data-testid': 'limitDate' }}
        />
      </Box>

      {editing && (
        <Box sx={{ mb: 2 }}>
          <FormControlLabel
            label="Done"
            control={
              <Checkbox
                checked={reminder?.isDone || false}
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
