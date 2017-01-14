
function DoneReminder(id, isDone, callBack) {
	$.post("Reminder/DoneReminder", { id: id, isDone: isDone }, callBack);
}
