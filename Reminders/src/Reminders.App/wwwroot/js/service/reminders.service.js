/**
 * Created by Kaue Reinbold on 16/11/2017.
 */

'use strict';

let RemindersService = Object.create(null, {
    doneReminder: {
        value: (data) => {
            return RemindersRepository.doneReminder(data);
        }
    },
    deleteReminder: {
        value: (id) => {
            return RemindersRepository.deleteReminder(id);
        }
    }
});