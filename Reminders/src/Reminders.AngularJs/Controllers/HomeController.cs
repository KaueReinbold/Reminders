using Microsoft.AspNetCore.Mvc;

namespace Reminders.AngularJs.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
