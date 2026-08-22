namespace Reminders.Mvc.ViewModels;

public enum ReminderBucket { Overdue, Today, Upcoming, Done }

public record ReminderGroup(ReminderBucket Bucket, IReadOnlyList<ReminderViewModel> Items);

public record WeekProgress(int Percent, string Caption);

public enum ModalMode { Create, Edit, Delete }

public record ReminderModal(ModalMode Mode, ReminderViewModel Reminder);

public class RemindersIndexViewModel
{
    public static readonly string[] Views = { "All", "Today", "Upcoming", "Done" };

    public string View { get; init; } = "All";
    public string Query { get; init; } = "";
    public DateTime Today { get; init; }
    public IReadOnlyList<ReminderGroup> Groups { get; init; } = Array.Empty<ReminderGroup>();
    public IReadOnlyDictionary<string, int> Counts { get; init; } = new Dictionary<string, int>();
    public WeekProgress Progress { get; init; } = new(0, "");
    public ReminderModal? Modal { get; set; }

    public string? RouteView => View == "All" ? null : View;
    public string? RouteQuery => Query.Length == 0 ? null : Query;

    // Mirrors the React reminderGroups util: groups, counts and week progress
    // are computed from the full list; only the groups narrow by view + query.
    public static RemindersIndexViewModel Build(
        IEnumerable<ReminderViewModel> reminders, string? view, string? query, DateTime today)
    {
        var all = reminders.ToList();
        var activeView = Views.FirstOrDefault(v => v.Equals(view, StringComparison.OrdinalIgnoreCase)) ?? "All";
        var q = (query ?? "").Trim();

        var visible = all
            .Where(r => activeView == "All" || ReminderGroups.BucketOf(r, today).ToString() == activeView)
            .Where(r => q.Length == 0 || Matches(r.Title, q) || Matches(r.Description, q))
            .OrderBy(r => r.LimitDate.Date)
            .ToList();

        var groups = Enum.GetValues<ReminderBucket>()
            .Select(b => new ReminderGroup(b, visible.Where(r => ReminderGroups.BucketOf(r, today) == b).ToList()))
            .Where(g => g.Items.Count > 0)
            .ToList();

        var counts = new Dictionary<string, int>
        {
            ["All"] = all.Count,
            ["Today"] = all.Count(r => ReminderGroups.BucketOf(r, today) == ReminderBucket.Today),
            ["Upcoming"] = all.Count(r => ReminderGroups.BucketOf(r, today) == ReminderBucket.Upcoming),
            ["Done"] = all.Count(r => r.IsDone),
        };

        return new RemindersIndexViewModel
        {
            View = activeView,
            Query = q,
            Today = today,
            Groups = groups,
            Counts = counts,
            Progress = ReminderGroups.WeekProgress(all, today),
        };
    }

    private static bool Matches(string? text, string q) =>
        text?.Contains(q, StringComparison.OrdinalIgnoreCase) == true;
}
