namespace Reminders.Application.Services;

public class RemindersService
  : IRemindersService
{
    private readonly ReminderViewModelValidator validator;
    private readonly IMapper mapper;
    private readonly IRemindersRepository remindersRepository;
    private readonly IUnitOfWork unitOfWork;

    public RemindersService(
      IMapper mapper,
      IRemindersRepository remindersRepository,
      IUnitOfWork unitOfWork)
    {
        validator = new ReminderViewModelValidator();

        this.mapper = mapper;
        this.remindersRepository = remindersRepository;
        this.unitOfWork = unitOfWork;
    }

    public ReminderViewModel Insert(ReminderViewModel reminderViewModel)
    {
        reminderViewModel.IsDone = false;

        validator.Validate(reminderViewModel, options =>
        {
            options.ThrowOnFailures();
            options.IncludeRuleSets("*");
        });

        reminderViewModel.Id = Guid.Empty;

        var reminder = mapper.Map<Reminder>(reminderViewModel);

        reminder = remindersRepository.Add(reminder);

        unitOfWork.Commit();

        return mapper.Map<ReminderViewModel>(reminder);
    }

    public ReminderViewModel Edit(
      Guid id,
      ReminderViewModel reminderViewModel)
    {
        validator.ValidateAndThrow(reminderViewModel);

        if (reminderViewModel.Id.Equals(id) == false)
            throw new RemindersApplicationException(ValidationStatus.IdsDoNotMatch, RemindersResources.IdsDoNotMatch);

        if (!remindersRepository.Exists(id))
            throw new RemindersApplicationException(ValidationStatus.NotFound, RemindersResources.NotFound);

        var reminder = mapper.Map<Reminder>(reminderViewModel);

        reminder = remindersRepository.Update(reminder);

        unitOfWork.Commit();

        return mapper.Map<ReminderViewModel>(reminder);
    }

    public void Delete(Guid id)
    {
        if (!remindersRepository.Exists(id))
            throw new RemindersApplicationException(ValidationStatus.NotFound, RemindersResources.NotFound);

        var reminderData = remindersRepository.Get(id);

        if (reminderData is not null)
        {
            reminderData.Delete();

            remindersRepository.Update(reminderData);

            unitOfWork.Commit();
        }
        else
        {
            throw new RemindersApplicationException(ValidationStatus.NotFound, RemindersResources.NotFound);
        }
    }

    public IQueryable<ReminderViewModel> Get()
    {
        var reminders = remindersRepository
          .Get()
          .Where(reminder => !reminder.IsDeleted);

        var remindersViewModel = reminders
          .ProjectTo<ReminderViewModel>(mapper.ConfigurationProvider);

        return remindersViewModel;
    }
    public ReminderViewModel Get(Guid id)
    {
        var reminder = remindersRepository
            .Get()
            .FirstOrDefault(reminder => reminder.Id == id && !reminder.IsDeleted);

        var reminderViewModel = mapper.Map<ReminderViewModel>(reminder);

        return reminderViewModel;
    }
}