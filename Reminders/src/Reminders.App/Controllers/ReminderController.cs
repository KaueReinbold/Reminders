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
                return RedirectToAction("Index", new { Type = TypeMessage.Error, Message = Resource.Resource.ResourceManager.GetString("ErrorGenericMessage") });

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
                    return RedirectToAction("", new { Type = TypeMessage.Success, Message = Resource.Resource.ResourceManager.GetString("SuccessCreateMessage") });
                else
                    return RedirectToAction("Create", new { Type = TypeMessage.Error, Message = Resource.Resource.ResourceManager.GetString("ErrorGenericMessage") });
            }

            return View(reminderViewModel);
        }

        // GET: Reminder/Edit/[id}
        [Route("Edit")]
        public IActionResult Edit(int id)
        {
            var reminderViewModel = _business.Find(id);

            if (reminderViewModel == null)
                return RedirectToAction("", new { Type = TypeMessage.Error, Message = Resource.Resource.ResourceManager.GetString("ErrorGenericMessage") });

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
                    return RedirectToAction("", new { Type = TypeMessage.Success, Message = Resource.Resource.ResourceManager.GetString("SuccessEditMessage") });
                else
                    return RedirectToAction("Edit", new { Type = TypeMessage.Error, Message = Resource.Resource.ResourceManager.GetString("ErrorGenericMessage") });
            }

            return View(reminderViewModel);
        }

        // GET: Reminder/Delete/{id}
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var reminderViewModel = _business.Find(id);

            if (reminderViewModel == null)
                return RedirectToAction("", new { Type = TypeMessage.Error, Message = Resource.Resource.ResourceManager.GetString("ErrorGenericMessage") });

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
                return RedirectToAction("", new { Type = TypeMessage.Success, Message = Resource.Resource.ResourceManager.GetString("SuccessDeleteMessage") });
            else
                return RedirectToAction("Delete", new { Type = TypeMessage.Error, Message = Resource.Resource.ResourceManager.GetString("ErrorGenericMessage") });
        }

        // GET: Reminder/Details/{id}
        [Route("Details")]
        public IActionResult Details(int id)
        {
            var reminderViewModel = _business.Find(id);

            if (reminderViewModel == null)
                return RedirectToAction("", new { Type = TypeMessage.Error, Message = Resource.Resource.ResourceManager.GetString("ErrorGenericMessage") });

            return View(reminderViewModel);
        }

        // POST Reminder/DoneReminder/{id}
        [HttpPost]
        public bool DoneReminder(int? id, bool isDone)
        {
            if (id.HasValue)
            {
                var reminder = _business.Find(id.Value);

                reminder.IsDone = isDone;

                return _business.Update(reminder);
            }
            else
                return false;
        }
    }
}
