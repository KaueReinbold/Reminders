using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reminders.Data.Context;
using Reminders.Domain;
using Reminders.Domain.Contract;
using Reminders.Data.Entity;

namespace Reminders.App.Controllers
{
    public class HomeController : Controller
    {
        private IRepositoryReminders<ReminderEntity> _repository;

        public HomeController(IRepositoryReminders<ReminderEntity> repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
