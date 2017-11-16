/**
    * Created by Kaue Reinbold on 16/11/2017.
*/

'use strict';

let ButtonEventsService = Object.create(null, {
    create: {
        value: () => {

            if (typeof (deleteReminderAction) !== "undefined")
                $(deleteReminderAction).click(ButtonEventsService.openModalDeleteReminder);

            if (typeof (saveDoneReminder) !== "undefined")
                $(saveDoneReminder).click(ButtonEventsService.doneReminder);

            if (typeof (deleteReminder) !== "undefined")
                $(deleteReminder).click(ButtonEventsService.deleteReminder);

        }
    },
    openModalDeleteReminder: {
        value: (event) => {
            actionDelete.value = event.target.getAttribute("idReminder");
            event.preventDefault();
            event.stopPropagation();
            $(modalDelete).modal();
        }
    },
    doneReminder: {
        value: () => {
            RemindersService.doneReminder({ id: idReminderForDone.value, isDone: idReminderDone.value })
                .then(() => location.href = location.origin)
                .catch((error) => console.error(error));
        }
    },
    deleteReminder: {
        value: () => {
            RemindersService.deleteReminder({ id: actionDelete.value })
                .then(() => location.href = location.origin)
                .catch((error) => console.error(error));
        }
    }
});