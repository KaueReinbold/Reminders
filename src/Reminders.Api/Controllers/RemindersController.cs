using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reminders.Application.Contracts;
using Reminders.Application.ViewModels;

namespace Reminders.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        private readonly ILogger<RemindersController> logger;
        private readonly IRemindersService remindersService;

        public RemindersController(
            ILogger<RemindersController> logger,
            IRemindersService remindersService)
        {
            this.logger = logger;
            this.remindersService = remindersService;
        }

        // GET: api/Reminders
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(remindersService.Get());
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);

                return null;
            }
        }

        // GET: api/Reminders/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(remindersService.Get(id));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);

                return null;
            }
        }

        // POST: api/Reminders
        [HttpPost]
        public void Post([FromBody] ReminderViewModel reminderViewModel)
        {
            try
            {
                remindersService.Insert(reminderViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        // PUT: api/Reminders/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] ReminderViewModel reminderViewModel)
        {
            try
            {
                remindersService.Edit(id, reminderViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            try
            {
                remindersService.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }
    }
}
