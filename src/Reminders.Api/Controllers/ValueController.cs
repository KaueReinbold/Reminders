using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reminders.Application.Contracts;

namespace Reminders.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValueController : ControllerBase
    {
        private readonly ILogger<ValueController> logger;
        private readonly IRemindersService remindersService;

        public ValueController(
            ILogger<ValueController> logger,
            IRemindersService remindersService)
        {
            this.logger = logger;
            this.remindersService = remindersService;
        }

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
    }
}
