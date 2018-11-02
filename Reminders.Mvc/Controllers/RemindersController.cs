using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Reminders.Business.Contracts;
using Reminders.Domain;
using Reminders.Domain.Enums;
using Reminders.Domain.Models;
using Reminders.Domain.Extensions;
using Reminders.Business.Contracts.Business;

namespace Reminders.Mvc.Controllers
{
    public class RemindersController : Controller
    {
        private readonly ILogger<RemindersController> _logger;
        private readonly IBusinessModelGeneric<ReminderModel> _businessReminderModel;

        public RemindersController(ILogger<RemindersController> logger, IBusinessModelGeneric<ReminderModel> businessReminderModel)
        {
            _logger = logger;
            _businessReminderModel = businessReminderModel;
        }

        // GET: Reminders
        public IActionResult Index()
        {
            var reminders = _businessReminderModel.GetAll();

            return View(reminders);
        }

        // GET: Reminders/Details/5
        public IActionResult Details(int id)
        {
            var reminderDetails = _businessReminderModel.Find(id);

            return View(reminderDetails);
        }

        // GET: Reminders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reminders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReminderModel reminder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    reminder.LimitDate = reminder.LimitDate.ToUniversalTime();

                    _businessReminderModel.Insert(reminder);

                    TempData["Message"] = new MessageModel { Type = EnumMessages.Success, Message = "Reminder created!" }.ToJson();

                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(reminder);

            }
            catch (Exception ex)
            {
                TempData["Message"] = new MessageModel { Type = EnumMessages.Error, Message = "Some error has happened!" }.ToJson();

                _logger.LogError(ex, string.Empty);

                return View();
            }
        }

        // GET: Reminders/Edit/5
        public IActionResult Edit(int id)
        {
            var reminderEdit = _businessReminderModel.Find(id);

            return View(reminderEdit);
        }

        // POST: Reminders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ReminderModel reminder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    reminder.LimitDate = reminder.LimitDate.ToUniversalTime();

                    _businessReminderModel.Update(reminder);

                    TempData["Message"] = new MessageModel { Type = EnumMessages.Success, Message = "Reminder updated!" }.ToJson();

                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(reminder);
            }
            catch (Exception ex)
            {
                TempData["Message"] = new MessageModel { Type = EnumMessages.Error, Message = "Some error has happened!" }.ToJson();

                _logger.LogError(ex, string.Empty);

                return View();
            }
        }

        // GET: Reminders/Delete/5
        public IActionResult Delete(int id)
        {
            var reminderDelete = _businessReminderModel.Find(id);

            return View(reminderDelete);
        }

        // POST: Reminders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(ReminderModel reminderModel)
        {
            try
            {
                _businessReminderModel.Delete(reminderModel.Id);

                TempData["Message"] = new MessageModel { Type = EnumMessages.Success, Message = "Reminder deleted!" }.ToJson();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Message"] = new MessageModel { Type = EnumMessages.Error, Message = "Some error has happened!" }.ToJson();

                _logger.LogError(ex, string.Empty);

                return View();
            }
        }
    }
}