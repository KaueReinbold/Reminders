using System.Web;

namespace Reminders.Mvc.Controllers;

public class RemindersController : Controller
{
    private readonly IRemindersService remindersService;

    public RemindersController(IRemindersService remindersService)
    {
        this.remindersService = remindersService;
    }

    // GET: Reminders?view=Today&q=text
    public async Task<IActionResult> Index(string? view, string? q, CancellationToken cancellationToken)
    {
        if (TempData["Error"] is string error)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return await IndexView(view, q, null, cancellationToken);
    }

    // GET: Reminders/Create (list page with the create modal open)
    public Task<IActionResult> Create(string? view, string? q, CancellationToken cancellationToken) =>
        IndexView(view, q, new ReminderModal(ModalMode.Create, new ReminderViewModel { LimitDate = DateTime.Today }), cancellationToken);

    // POST: Reminders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] ReminderViewModel reminder, string? view, string? q, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await remindersService.AddReminderAsync(reminder, cancellationToken);

                return RedirectToAction(nameof(Index), new { view, q });
            }
            catch (ApiException ex)
            {
                AddApiErrors(ex);
            }
        }

        return await IndexView(view, q, new ReminderModal(ModalMode.Create, reminder), cancellationToken);
    }

    // GET: Reminders/Edit/5 (list page with the edit modal open)
    public Task<IActionResult> Edit(Guid id, string? view, string? q, CancellationToken cancellationToken) =>
        OpenModal(ModalMode.Edit, id, view, q, cancellationToken);

    // POST: Reminders/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromForm] ReminderViewModel reminder, string? view, string? q, CancellationToken cancellationToken)
    {
        reminder.Id = id;

        if (ModelState.IsValid)
        {
            try
            {
                await remindersService.EditReminderAsync(id, reminder, cancellationToken);

                return RedirectToAction(nameof(Index), new { view, q });
            }
            catch (ApiException ex)
            {
                AddApiErrors(ex);
            }
        }

        return await IndexView(view, q, new ReminderModal(ModalMode.Edit, reminder), cancellationToken);
    }

    // GET: Reminders/Delete/5 (list page with the delete confirmation open)
    public Task<IActionResult> Delete(Guid id, string? view, string? q, CancellationToken cancellationToken) =>
        OpenModal(ModalMode.Delete, id, view, q, cancellationToken);

    // POST: Reminders/Delete/5
    [HttpPost]
    [ActionName(nameof(Delete))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, string? view, string? q, CancellationToken cancellationToken)
    {
        try
        {
            await remindersService.DeleteReminderAsync(id, cancellationToken);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { view, q });
    }

    // POST: Reminders/Toggle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(Guid id, string? view, string? q, CancellationToken cancellationToken)
    {
        try
        {
            var reminder = await remindersService.GetReminderAsync(id, cancellationToken);

            if (reminder is not null)
            {
                reminder.IsDone = !reminder.IsDone;

                await remindersService.EditReminderAsync(id, reminder, cancellationToken);
            }
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.ErrorResponse?.Errors is null
                ? ApiException.TROUBLE_CONNECTING_SERVERS
                : HttpUtility.HtmlEncode(string.Join(" ", ex.ErrorResponse.Errors.SelectMany(e => e.Value)));
        }

        return RedirectToAction(nameof(Index), new { view, q });
    }

    private async Task<IActionResult> OpenModal(ModalMode mode, Guid id, string? view, string? q, CancellationToken cancellationToken)
    {
        ReminderModal? modal = null;

        try
        {
            var reminder = await remindersService.GetReminderAsync(id, cancellationToken);

            if (reminder is null)
            {
                ModelState.AddModelError(string.Empty, "Reminder not found.");
            }
            else
            {
                modal = new ReminderModal(mode, reminder);
            }
        }
        catch (ApiException ex)
        {
            AddApiErrors(ex);
        }

        return await IndexView(view, q, modal, cancellationToken);
    }

    private async Task<IActionResult> IndexView(string? view, string? q, ReminderModal? modal, CancellationToken cancellationToken)
    {
        IEnumerable<ReminderViewModel> reminders = Enumerable.Empty<ReminderViewModel>();

        try
        {
            reminders = await remindersService.GetRemindersAsync(cancellationToken) ?? reminders;
        }
        catch (ApiException ex)
        {
            AddApiErrors(ex);
        }

        var model = RemindersIndexViewModel.Build(reminders, view, q, DateTime.Today);
        model.Modal = modal;

        return View(nameof(Index), model);
    }

    private void AddApiErrors(ApiException ex)
    {
        if (ex.ErrorResponse?.Errors is not null)
        {
            foreach (var (key, errors) in ex.ErrorResponse.Errors)
            {
                // API keys like "LimitDate.Date" map to the form field "LimitDate"
                var field = key.Split('.')[0];

                foreach (var error in errors)
                {
                    // Encode the error message to prevent XSS attacks
                    ModelState.AddModelError(field, HttpUtility.HtmlEncode(error));
                }
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, ApiException.TROUBLE_CONNECTING_SERVERS);
        }
    }
}
