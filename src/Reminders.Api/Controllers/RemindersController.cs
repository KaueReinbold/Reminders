using System;
using Microsoft.AspNetCore.Mvc;
using Reminders.Application.Contracts;
using Reminders.Application.ViewModels;

namespace Reminders.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        private readonly IRemindersService remindersService;

        public RemindersController(IRemindersService remindersService) => this.remindersService = remindersService;

        // GET: api/Reminders
        [HttpGet]
        public IActionResult Get() => Ok(remindersService.Get());

        // GET: api/Reminders/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(Guid id) => Ok(remindersService.Get(id));

        // POST: api/Reminders
        [HttpPost]
        public void Post([FromBody] ReminderViewModel reminderViewModel) => remindersService.Insert(reminderViewModel);

        // PUT: api/Reminders/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] ReminderViewModel reminderViewModel) => remindersService.Edit(id, reminderViewModel);

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(Guid id) => remindersService.Delete(id);
    }
}
