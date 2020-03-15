using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reminders.Application.Contracts;
using Reminders.Application.ViewModels;
using System;

namespace Reminders.Mvc.Controllers
{
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
        public ActionResult Index() =>
            View(remindersService.Get());

        // GET: Reminders/Details/5
        public ActionResult Details(Guid id) =>
            View(remindersService.Get(id));

        // GET: Reminders/Create
        public ActionResult Create() =>
            View();

        // POST: Reminders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] ReminderViewModel reminder)
        {
            remindersService.Insert(reminder);

            return RedirectToAction(nameof(Index));
        }

        // GET: Reminders/Edit/5
        public ActionResult Edit(Guid id) =>
            View(remindersService.Get(id));

        // POST: Reminders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, [FromForm] ReminderViewModel reminder)
        {
            remindersService.Edit(id, reminder);

            return RedirectToAction(nameof(Index));
        }

        // GET: Reminders/Delete/5
        public ActionResult Delete(Guid id) =>
            View(remindersService.Get(id));

        // POST: Reminders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, [FromForm] ReminderViewModel reminder)
        {
            remindersService.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}