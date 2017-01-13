using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reminders.Domain.Contract;
using Reminders.Data.Entity;
using Reminders.App.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Reminders.App.Controllers
{
    public class ReminderController : Controller
    {
        private readonly IRepositoryReminders<ReminderEntity> _repository;
        public ReminderController(IRepositoryReminders<ReminderEntity> repository)
        {
            _repository = repository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var reminders = _repository.GetAll().ToList();
            var remindersViewModel = new List<ReminderViewModel>();

            reminders.ForEach(r =>
            {
                var reminder = new ReminderViewModel
                {
                    ID = r.ID,
                    Title = r.Title,
                    Description = r.Description,
                    LimitDate = r.LimitDate,
                    IsDone = r.IsDone
                };
                remindersViewModel.Add(reminder);
            });

            return View(remindersViewModel);
        }

        // GET: ReminderViewModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReminderViewModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReminderViewModel reminderViewModel)
        {
            if (ModelState.IsValid)
            {
                var reminder = new ReminderEntity
                {
                    Title = reminderViewModel.Title,
                    Description = reminderViewModel.Description,
                    LimitDate = reminderViewModel.LimitDate,
                    IsDone = reminderViewModel.IsDone
                };

                _repository.Insert(reminder);

                return RedirectToAction("Index");
            }

            return View(reminderViewModel);
        }

        // GET: ReminderViewModels/Delete/{id}
        public IActionResult Delete(int id)
        {
            var reminder = _repository.Find(id);

            var reminderViewModel = new ReminderViewModel
            {
                ID = reminder.ID,
                Title = reminder.Title,
                Description = reminder.Description,
                LimitDate = reminder.LimitDate,
                IsDone = reminder.IsDone
            };

            return View(reminderViewModel);
        }

        // POST: ReminderViewModels/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var reminder = _repository.Find(id);
            _repository.Delete(reminder);
            return RedirectToAction("Index");
        }

        // GET: ReminderViewModels/Edit/[id}
        public IActionResult Edit(int id)
        {
            var reminder = _repository.Find(id);

            var reminderViewModel = new ReminderViewModel
            {
                ID = reminder.ID,
                Title = reminder.Title,
                Description = reminder.Description,
                LimitDate = reminder.LimitDate,
                IsDone = reminder.IsDone
            };

            return View(reminderViewModel);
        }

        // POST: ReminderViewModels/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ReminderViewModel reminderViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var reminder = new ReminderEntity
                    {
                        ID = id,
                        Title = reminderViewModel.Title,
                        Description = reminderViewModel.Description,
                        LimitDate = reminderViewModel.LimitDate,
                        IsDone = reminderViewModel.IsDone
                    };

                    _repository.Update(reminder);
                }
                catch (Exception)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(reminderViewModel);
        }

        // GET: ReminderViewModels/Details/5
        public IActionResult Details(int id)
        {
            var reminder = _repository.Find(id);

            var reminderViewModel = new ReminderViewModel
            {
                ID = reminder.ID,
                Title = reminder.Title,
                Description = reminder.Description,
                LimitDate = reminder.LimitDate,
                IsDone = reminder.IsDone
            };

            return View(reminderViewModel);
        }
    }
}
