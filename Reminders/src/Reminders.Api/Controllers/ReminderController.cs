using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Reminders.Domain.Models;
using Reminders.Domain.BusinessContract;

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

        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            var remindersModel = _business.GetAll();

            return Ok(remindersModel);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var reminderModel = _business.Find(id);

            return Ok(reminderModel);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]ReminderModel reminderModel)
        {
            var result = _business.Insert(reminderModel);

            if (result)
               return Ok(new { });
            else
                return BadRequest();
        }

        // PUT api/values/5
        [HttpPut]
        public IActionResult Put([FromBody]ReminderModel reminderModel)
        {
           var result = _business.Update(reminderModel);

            if (result)
                return Ok(new { });
            else
                return BadRequest();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _business.Delete(id);

            if (result)
                return Ok(new { });
            else
                return BadRequest();
        }
    }
}
