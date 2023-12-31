namespace Reminders.Mvc.Controllers
{
    // FIXME: Do not redirect user to a different page when an error occurred. Show a friendly message.
    public class RemindersController : Controller
    {
        private ILogger<RemindersController> logger;
        private IRemindersService remindersService;

        public RemindersController(
            ILogger<RemindersController> logger,
            IRemindersService remindersService)
        {
            this.logger = logger;
            this.remindersService = remindersService;
        }

        // GET: Reminders
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                return View(await remindersService.GetRemindersAsync(cancellationToken));
            }
            catch (ApiException ex)
            {
                ConvertApiExceptionToModalStateErrors(ex);

                return View(new List<ReminderViewModel>());
            }
        }

        // GET: Reminders/Details/5
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
            => await GetReminder(id, cancellationToken);

        // GET: Reminders/Create
        public IActionResult Create() =>
            View();

        // POST: Reminders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ReminderViewModel reminder, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await remindersService.AddReminderAsync(reminder, cancellationToken);

                    return RedirectToAction(nameof(Index));
                }

                return View(reminder);
            }
            catch (ApiException ex)
            {
                ConvertApiExceptionToModalStateErrors(ex);

                return View();
            }
        }

        // GET: Reminders/Edit/5
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
            => await GetReminder(id, cancellationToken);

        // POST: Reminders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] ReminderViewModel reminder, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await remindersService.EditReminderAsync(id, reminder, cancellationToken);

                    return RedirectToAction(nameof(Index));
                }

                return View(reminder);
            }
            catch (ApiException ex)
            {
                ConvertApiExceptionToModalStateErrors(ex);

                return View();
            }

        }

        // GET: Reminders/Delete/5
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
            => await GetReminder(id, cancellationToken);

        // POST: Reminders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, [FromForm] ReminderViewModel reminder, CancellationToken cancellationToken)
        {
            await remindersService.DeleteReminderAsync(id, cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        private void ConvertApiExceptionToModalStateErrors(ApiException ex)
        {
            if (ex.ErrorResponse?.Errors is not null)
            {
                foreach (var (key, errors) in ex.ErrorResponse.Errors)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(key, error);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("Error", ApiException.TROUBLE_CONNECTING_SERVERS);
            }
        }

        private async Task<IActionResult> GetReminder(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                return View(await remindersService.GetReminderAsync(id, cancellationToken));
            }
            catch (ApiException ex)
            {
                ConvertApiExceptionToModalStateErrors(ex);

                return View();
            }
        }
    }
}