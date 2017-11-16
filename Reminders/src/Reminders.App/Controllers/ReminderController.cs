using Microsoft.AspNetCore.Mvc;
using Reminders.Domain.Models;
using Reminders.Domain.BusinessContract;
using Reminders.Domain.Resource;
using Reminders.Domain.Enum;
using Reminders.Domain.Framework;
using Newtonsoft.Json;

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
            {
                Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
                {
                    type_message = TypeMessage.Error,
                    text_message = Resource.ResourceManager.GetString("ErrorGenericMessage")
                }));

                return RedirectToAction("");
            }

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

                Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
                {
                    type_message = result ? TypeMessage.Success : TypeMessage.Error,
                    text_message = result ? Resource.ResourceManager.GetString("SuccessCreateMessage") : Resource.ResourceManager.GetString("ErrorGenericMessage")
                }));

                return RedirectToAction("");
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
                Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
                {
                    type_message = TypeMessage.Error,
                    text_message = Resource.ResourceManager.GetString("ErrorGenericMessage")
                }));

                return Index();
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

                Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
                {
                    type_message = result ? TypeMessage.Success : TypeMessage.Error,
                    text_message = result ? Resource.ResourceManager.GetString("SuccessEditMessage") : Resource.ResourceManager.GetString("ErrorGenericMessage")
                }));

                return RedirectToAction("");
            }

            return View(reminderModel);
        }

        // GET: Reminder/Details/{id}
        [Route("Details")]
        public IActionResult Details(int id)
        {
            var ReminderModel = _business.Find(id);

            if (ReminderModel == null)
            {
                Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
                {
                    type_message = TypeMessage.Error,
                    text_message = Resource.ResourceManager.GetString("ErrorGenericMessage")
                }));

                return RedirectToAction("");
            }

            return View(ReminderModel);
        }

        // POST: Reminder/Delete/{id}
        [HttpPost]
        public void Delete(int id)
        {
            var result = _business.Delete(id);

            Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
            {
                type_message = result ? TypeMessage.Success : TypeMessage.Error,
                text_message = result ? Resource.ResourceManager.GetString("SuccessDeleteMessage") : Resource.ResourceManager.GetString("ErrorGenericMessage")
            }));
        }

        // POST Reminder/DoneReminder/{id}
        [HttpPost]
        public void DoneReminder(int id, bool isDone)
        {
            var reminder = _business.Find(id);

            if (reminder == null)
                Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
                {
                    type_message = TypeMessage.Error,
                    text_message = Resource.ResourceManager.GetString("ErrorGenericMessage")
                }));

            reminder.is_done = isDone;

            var result = _business.Update(reminder);

            Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
            {
                type_message = result ? TypeMessage.Success : TypeMessage.Error,
                text_message = result ? Resource.ResourceManager.GetString("SuccessDeleteMessage") : Resource.ResourceManager.GetString("ErrorGenericMessage")
            }));

            if (isDone && result)
                Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
                {
                    type_message = TypeMessage.Success,
                    text_message = Resource.ResourceManager.GetString("SuccessDoneMessage")
                }));
            else if (!isDone && result)
                Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
                {
                    type_message = TypeMessage.Success,
                    text_message = Resource.ResourceManager.GetString("SuccessEnableMessage")
                }));
            else
                Response.Cookies.Append("StatusMessage", JsonConvert.SerializeObject(new
                {
                    type_message = TypeMessage.Error,
                    text_message = Resource.ResourceManager.GetString("ErrorGenericMessage")
                }));
        }
    }
}
