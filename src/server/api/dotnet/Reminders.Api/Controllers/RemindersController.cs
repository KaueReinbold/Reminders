namespace Reminders.Api.Controllers;

[Route("api/reminders")]
[ApiController]
[IgnoreAntiforgeryToken] // REST API - uses CORS instead of anti-forgery tokens
public class RemindersController
    : ControllerBase
{
    private readonly IRemindersService remindersService;

    public RemindersController(IRemindersService remindersService) =>
        this.remindersService = remindersService;

    // GET: api/Reminders
    [HttpGet]
    public IActionResult Get() =>
        Ok(remindersService.Get());
    
    // GET: api/Reminders/Count
    [HttpGet("count")]
    public IActionResult Count() =>
        Ok(remindersService.Get().Count());

    // GET: api/Reminders/5
    [HttpGet("{id}", Name = "Get")]
    public async Task<IActionResult> Get(Guid id) =>
        Ok(await remindersService.GetAsync(id));

    // POST: api/Reminders
    [HttpPost]
    public Task<ReminderViewModel> Post([FromBody] ReminderViewModel reminderViewModel) =>
        remindersService.InsertAsync(reminderViewModel);

    // PUT: api/Reminders/5
    [HttpPut("{id}")]
    public Task<ReminderViewModel> Put(Guid id, [FromBody] ReminderViewModel reminderViewModel) =>
        remindersService.EditAsync(id, reminderViewModel);

    // DELETE: api/Reminders/5
    [HttpDelete("{id}")]
    public Task Delete(Guid id) =>
        remindersService.DeleteAsync(id);
}
