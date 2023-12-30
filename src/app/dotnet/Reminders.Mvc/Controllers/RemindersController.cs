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
        public async Task<IActionResult> Index() =>
            View(await remindersService.GetRemindersAsync());

        // GET: Reminders/Details/5
        public async Task<IActionResult> Details(Guid id) =>
            View(await remindersService.GetReminderAsync(id));

        // GET: Reminders/Create
        public IActionResult Create() =>
            View();

        // POST: Reminders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ReminderViewModel reminder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await remindersService.AddReminderAsync(reminder);

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
        public async Task<IActionResult> Edit(Guid id) =>
            View(await remindersService.GetReminderAsync(id));

        // POST: Reminders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] ReminderViewModel reminder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await remindersService.EditReminderAsync(id, reminder);

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
        public async Task<IActionResult> Delete(Guid id) =>
            View(await remindersService.GetReminderAsync(id));

        // POST: Reminders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, [FromForm] ReminderViewModel reminder)
        {
            await remindersService.DeleteReminderAsync(id);

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
        }
    }
}