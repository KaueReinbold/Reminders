using Microsoft.AspNetCore.Mvc;
using Reminders.Domain.Models;
using Reminders.Domain.BusinessContract;
using Reminders.Domain.Resource;
using Reminders.Domain.Enum;

namespace Reminders.App.Controllers
{
    public class ReminderController : Controller
    {
        private IBusinessReminder _business;

        public ReminderController(IBusinessReminder business)
        {
            _business = business;
        }

        // GET: Reminder
        [Route("")]
        [Route("Reminder")]
        [Route("Reminder/Index")]
        public IActionResult Index()
        {
            var remindersViewModel = _business.GetAll();

            if (remindersViewModel == null)
                return RedirectToAction("Index", new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });

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
        public IActionResult Create(ReminderModel reminderModel)
        {
            if (ModelState.IsValid)
            {
                var result = _business.Insert(reminderModel);

                if (result)
                {
                    ViewBag.StatusMessage = Resource.ResourceManager.GetString("SuccessCreateMessage");
                    ViewBag.StatusMessageStatus = TypeMessage.Success;
                    return RedirectToAction("Create");
                }
                else
                {
                    ViewBag.StatusMessage = Resource.ResourceManager.GetString("ErrorGenericMessage");
                    ViewBag.StatusMessageStatus = TypeMessage.Error;
                }
            }

            return View(reminderModel);
        }

        // GET: Reminder/Edit/[id}
        [Route("Edit")]
        public IActionResult Edit(int id)
        {
            var ReminderModel = _business.Find(id);

            if (ReminderModel == null)
            {
                ViewData["StatusMessage"] = Resource.ResourceManager.GetString("ErrorGenericMessage");
                ViewData["StatusMessageStatus"] = TypeMessage.Error;
            }

            return View(ReminderModel);
        }

        // POST: Reminder/Edit/{id}
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ReminderModel reminderModel)
        {
            if (ModelState.IsValid)
            {
                var result = _business.Update(reminderModel);

                if (result)
                {
                    ViewData["StatusMessage"] = Resource.ResourceManager.GetString("SuccessEditMessage");
                    ViewData["StatusMessageStatus"] = TypeMessage.Success;
                }
                else
                {
                    ViewData["StatusMessage"] = Resource.ResourceManager.GetString("ErrorGenericMessage");
                    ViewData["StatusMessageStatus"] = TypeMessage.Error;
                }

                return View();
            }

            return View(reminderModel);
        }

        // GET: Reminder/Details/{id}
        [Route("Details")]
        public IActionResult Details(int id)
        {
            var ReminderModel = _business.Find(id);

            if (ReminderModel == null)
                return RedirectToAction("", new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });

            return View(ReminderModel);
        }

        // POST: Reminder/Delete/{id}
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var result = _business.Delete(id);

            if (result)
                return Json(new { Type = TypeMessage.Success, Message = Resource.ResourceManager.GetString("SuccessDeleteMessage") });
            else
                return Json(new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });
        }

        // POST Reminder/DoneReminder/{id}
        [HttpPost]
        public JsonResult DoneReminder(int id, bool isDone)
        {
            var reminder = _business.Find(id);

            if (reminder == null)
                return Json(new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });

            reminder.is_done = isDone;

            var result = _business.Update(reminder);

            if (isDone && result)
                return Json(new { Type = TypeMessage.Success, Message = Resource.ResourceManager.GetString("SuccessDoneMessage") });
            else if (!isDone && result)
                return Json(new { Type = TypeMessage.Success, Message = Resource.ResourceManager.GetString("SuccessEnableMessage") });
            else
                return Json(new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });
        }
    }
}
