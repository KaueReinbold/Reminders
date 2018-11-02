using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reminders.Business.Contracts;
using Reminders.Domain.Models;
using Reminders.Domain.Extensions;
using Reminders.Business.Contracts.Business;

namespace Reminders.Api.Controllers
{
    /// <summary>
    /// Controller of Reminders.
    /// </summary>
    [Route("api/[controller]")]
    public class RemindersController : Controller
    {
        private readonly ILogger<RemindersController> _logger;
        private readonly IBusinessModelGeneric<ReminderModel> _businessReminderModel;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="businessReminderModel"></param>
        public RemindersController(ILogger<RemindersController> logger, IBusinessModelGeneric<ReminderModel> businessReminderModel)
        {
            _logger = logger;
            _businessReminderModel = businessReminderModel;
        }

        /// <summary>
        /// Get all reminders.
        /// </summary>
        /// <returns> Return a json array with all reminders. </returns>
        // GET api/values
        [HttpGet]
        public string Get()
        {
            var reminders = _businessReminderModel.GetAll();

            return reminders.ToJson();
        }

        /// <summary>
        /// Get reminder by id.
        /// </summary>
        /// <param name="id"> Int variables that represents an Id of a reminder. </param>
        /// <returns> Return a json object with a reminder. </returns>
        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            var reminder = _businessReminderModel.Find(id);

            return reminder.ToJson();
        }

        /// <summary>
        /// Post that insert a reminder.
        /// </summary>
        /// <param name="reminder"> Object that have all the attributes of a reminder. </param>
        /// <returns> Returns the status of process. </returns>
        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]ReminderModel reminder)
        {
            reminder.LimitDate = reminder.LimitDate.ToUniversalTime();

            var reminderCreated = _businessReminderModel.Insert(reminder);

            if (reminderCreated.Id == 0)
                return BadRequest("Some error has happened!");

            return Ok("Reminder created!");
        }

        /// <summary>
        /// Put that update a reminder.
        /// </summary>
        /// <param name="reminder"> Object that have all the attributes of a reminder to update. </param>
        /// <returns> Returns the status of process. </returns>
        // PUT api/values
        [HttpPut]
        public IActionResult Put([FromBody]ReminderModel reminder)
        {
            reminder.LimitDate = reminder.LimitDate.ToUniversalTime();

            var reminderEdit = _businessReminderModel.Update(reminder);

            if (!reminderEdit)
                return BadRequest("Some error has happened!");

            return Ok("Reminder updated!");
        }

        /// <summary>
        /// Delete that remove a reminder.
        /// </summary>
        /// <param name="id"> Int variables that represents an Id of a reminder. </param>
        /// <returns></returns>
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var hasDeleted = _businessReminderModel.Delete(id);

            if (!hasDeleted)
                return BadRequest("Some error has happened!");

            return Ok("Reminder deleted!");
        }
    }
}
