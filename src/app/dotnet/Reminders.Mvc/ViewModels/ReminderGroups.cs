namespace Reminders.Mvc.ViewModels;

// Date bucketing helpers shared by the list view and its view model.
// Limit dates are compared as calendar days against the server's today.
public static class ReminderGroups
{
    public static int DayDiff(ReminderViewModel reminder, DateTime today) =>
        (reminder.LimitDate.Date - today.Date).Days;

    public static bool IsOverdue(ReminderViewModel reminder, DateTime today) =>
        !reminder.IsDone && DayDiff(reminder, today) < 0;

    public static ReminderBucket BucketOf(ReminderViewModel reminder, DateTime today)
    {
        if (reminder.IsDone) return ReminderBucket.Done;
        var diff = DayDiff(reminder, today);
        if (diff < 0) return ReminderBucket.Overdue;
        if (diff == 0) return ReminderBucket.Today;
        return ReminderBucket.Upcoming;
    }

    public static string DateLabel(ReminderViewModel reminder, DateTime today)
    {
        var diff = DayDiff(reminder, today);
        return diff switch
        {
            0 => "Today",
            1 => "Tomorrow",
            -1 => "Yesterday",
            < 0 => $"{-diff} days late",
            _ => reminder.LimitDate.ToString("MMM d", System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    // Percent done of reminders due within the next 7 days (overdue included,
    // matching the design prototype). Caption surfaces overdue count when any exist.
    public static WeekProgress WeekProgress(IReadOnlyCollection<ReminderViewModel> reminders, DateTime today)
    {
        var week = reminders.Where(r => DayDiff(r, today) <= 7).ToList();
        var done = week.Count(r => r.IsDone);
        var percent = week.Count == 0 ? 0 : (int)Math.Round(done * 100.0 / week.Count);
        var overdue = reminders.Count(r => IsOverdue(r, today));

        var caption = overdue > 0
            ? $"{overdue} {(overdue == 1 ? "reminder is" : "reminders are")} overdue"
            : $"{done} of {week.Count} done in the next 7 days";

        return new WeekProgress(percent, caption);
    }
}
