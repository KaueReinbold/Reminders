using Microsoft.AspNetCore.Mvc;
using Reminders.Domain.Models;
using Reminders.Domain.BusinessContract;
using Reminders.Domain.Enum;
using Reminders.Domain.Resource;

namespace Reminders.Api.Controllers
{
    [Route("api/[controller]")]
    public class ReminderController : Controller
    {
        private IBusinessReminder _business;

        public ReminderController(IBusinessReminder busines)
        {
            _business = busines;
        }

        // GET: api/Reminder
        [HttpGet]
        public IActionResult Get()
        {
            var remindersModel = _business.GetAll();

            if (remindersModel == null)
                return NotFound(new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });

            return Ok(remindersModel);
        }

        // GET api/Reminder/{id}
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var reminderModel = _business.Find(id);

            if (reminderModel == null)
                return NotFound(new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });

            return Ok(reminderModel);
        }

        // POST api/Reminder
        [HttpPost]
        public IActionResult Post([FromBody]ReminderModel reminderModel)
        {
            var result = _business.Insert(reminderModel);

            if (result)
               return Ok(new { Type = TypeMessage.Success, Message = Resource.ResourceManager.GetString("SuccessCreateMessage") });
            else
                return BadRequest(new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });
        }

        // PUT api/Reminder
        [HttpPut]
        public IActionResult Put([FromBody]ReminderModel reminderModel)
        {
           var result = _business.Update(reminderModel);

            if (result)
                return Ok(new { Type = TypeMessage.Success, Message = Resource.ResourceManager.GetString("SuccessEditMessage") });
            else
                return BadRequest(new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });
        }

        // DELETE api/Reminder/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _business.Delete(id);

            if (result)
                return Ok(new { Type = TypeMessage.Success, Message = Resource.ResourceManager.GetString("SuccessDeleteMessage") });
            else
                return BadRequest(new { Type = TypeMessage.Error, Message = Resource.ResourceManager.GetString("ErrorGenericMessage") });
        }
    }
}
