using Microsoft.AspNetCore.Mvc;
using Reminders.App.Models;
using Reminders.App.BusinessContract;

namespace Reminders.App.Controllers
{
    public class ReminderController : Controller
    {
        private IBusinessReminder _business;

        public ReminderController(IBusinessReminder helper)
        {
            _business = helper;
        }

        // GET: Reminder
        [Route("")]
        [Route("Reminder")]
        [Route("Reminder/Index")]
        public IActionResult Index()
        {
            var remindersViewModel = _business.GetAll();

            if (remindersViewModel == null)
                return RedirectToAction("Index", new { Message = "Ocorreu um erro ao executar a ação." });

            return View(remindersViewModel);
        }

        // GET: Reminder/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reminder/Create
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReminderViewModel reminderViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _business.Insert(reminderViewModel);

                if (result)
                    return RedirectToAction("");
                else
                    return RedirectToAction("Create", new { Message = "Ocorreu um erro ao executar a ação." });
            }

            return View(reminderViewModel);
        }

        // GET: Reminder/Edit/[id}
        [Route("Edit")]
        public IActionResult Edit(int id)
        {
            var reminderViewModel = _business.Find(id);

            if (reminderViewModel == null)
                return RedirectToAction("", new { Message = "Ocorreu um erro ao executar a ação." });

            return View(reminderViewModel);
        }

        // POST: Reminder/Edit/{id}
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ReminderViewModel reminderViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _business.Update(reminderViewModel);

                if (result)
                    return RedirectToAction("");
                else
                    return RedirectToAction("Edit", new { Message = "Ocorreu um erro ao executar a ação." });
            }

            return View(reminderViewModel);
        }

        // GET: Reminder/Delete/{id}
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var reminderViewModel = _business.Find(id);

            if (reminderViewModel == null)
                return RedirectToAction("", new { Message = "Ocorreu um erro ao executar a ação." });

            return View(reminderViewModel);
        }

        // POST: Reminder/Delete/{id}
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = _business.Delete(id);

            if (result)
                return RedirectToAction("");
            else
                return RedirectToAction("Delete", new { Message = "Ocorreu um erro ao executar a ação." });
        }

        // GET: Reminder/Details/{id}
        [Route("Details")]
        public IActionResult Details(int id)
        {
            var reminderViewModel = _business.Find(id);

            if (reminderViewModel == null)
                return RedirectToAction("", new { Message = "Ocorreu um erro ao executar a ação." });

            return View(reminderViewModel);
        }
    }
}
