using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reminders.Domain.Contract;
using Reminders.Data.Entity;
using Reminders.App.Models;
using Reminders.App.Helpers;

namespace Reminders.App.Controllers
{
    public class ReminderController : Controller
    {
        private HelperReminder _helper;

        public ReminderController(HelperReminder helper)
        {
            _helper = helper;
        }

        // GET: Reminder
        public IActionResult Index()
        {
            var remindersViewModel = _helper.GetAll();

            return View(remindersViewModel);
        }

        // GET: Reminder/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reminder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReminderViewModel reminderViewModel)
        {
            if (ModelState.IsValid)
            {
                _helper.Insert(reminderViewModel);

                return RedirectToAction("Index");
            }

            return View(reminderViewModel);
        }

        // GET: Reminder/Delete/{id}
        public IActionResult Delete(int id)
        {
            var reminderViewModel = _helper.Find(id);

            return View(reminderViewModel);
        }

        // POST: Reminder/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _helper.Delete(id);

            return RedirectToAction("Index");
        }

        // GET: Reminder/Edit/[id}
        public IActionResult Edit(int id)
        {
            var reminderViewModel = _helper.Find(id);

            return View(reminderViewModel);
        }

        // POST: Reminder/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ReminderViewModel reminderViewModel)
        {
            if (ModelState.IsValid)
            {
                _helper.Update(reminderViewModel);

                return RedirectToAction("Index");
            }

            return View(reminderViewModel);
        }

        // GET: Reminder/Details/{id}
        public IActionResult Details(int id)
        {
            var reminderViewModel = _helper.Find(id);

            return View(reminderViewModel);
        }
    }
}
