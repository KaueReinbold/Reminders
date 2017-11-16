/**
 * Created by Kaue Reinbold on 16/11/2017.
 */

'use strict';

let RemindersRepository = Object.create(null, {
    doneReminder: {
        value: (data) => {
            return RequestApi.post('Reminder/DoneReminder', data);
        }
    },
    deleteReminder: {
        value: (id) => {
            return RequestApi.post('Reminder/Delete', id);
        }
    }
});