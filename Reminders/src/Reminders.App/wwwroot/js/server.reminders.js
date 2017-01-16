function PostDoneReminder(id, isDone, callBack) {
    $.post("Reminder/DoneReminder", { id: id, isDone: isDone }, callBack);
}

function PostDeleteReminder(id, callBack) {
    $.post("Reminder/Delete?id=" +id, null, callBack);
}