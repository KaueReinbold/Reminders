import { Checkbox, FormControlLabel, Grid, TextField } from '@mui/material';
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
      <AlertError error={errors?.ServerError} />

      {editing && (
        <Grid item>
          <TextField
            label="Id"
            defaultValue={reminder?.id}
            disabled
            fullWidth
            inputProps={{ readOnly: true }}
            InputLabelProps={{ shrink: true }}
          />
        </Grid>
      )}

      <Grid item>
        <TextField
          label="Title"
          defaultValue={reminder?.title}
          onChange={e => handleChange('title', e.target.value)}
          required
          fullWidth
          InputLabelProps={{ shrink: true }}
          error={Boolean(errors?.Title)}
          helperText={errors?.Title}
        />
      </Grid>

      <Grid item>
        <TextField
          label="Description"
          defaultValue={reminder?.description}
          onChange={e => handleChange('description', e.target.value)}
          required
          fullWidth
          InputLabelProps={{ shrink: true }}
          error={Boolean(errors?.Description)}
          helperText={errors?.Description}
        />
      </Grid>

      <Grid item>
        <TextField
          label="Limit Date"
          defaultValue={reminder?.limitDateFormatted}
          onChange={e => handleChange('limitDate', e.target.value)}
          required
          type="date"
          fullWidth
          InputLabelProps={{ shrink: true }}
          error={Boolean(errors?.['LimitDate.Date'])}
          helperText={errors?.['LimitDate.Date']}
        />
      </Grid>

      {editing && (
        <Grid item>
          <FormControlLabel
            label="Done"
            control={
              <Checkbox
                checked={reminder?.isDone ?? false}
                onChange={e => handleChange('isDone', e.target.checked)}
              />
            }
          />
        </Grid>
      )}
    </>
  );
}
