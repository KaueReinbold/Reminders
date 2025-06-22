namespace Reminders.Application.Services;

public class RemindersService
  : IRemindersService
{
    private readonly ReminderViewModelValidator validator;
    private readonly IMapper mapper;
    private readonly ILogger<RemindersService> logger;
    private readonly IRemindersRepository remindersRepository;
    private readonly IRemindersBlockchainService remindersBlockchainService;
    private readonly IUnitOfWork unitOfWork;

    public RemindersService(
        ILogger<RemindersService> logger,
        IMapper mapper,
        IRemindersRepository remindersRepository,
        IRemindersBlockchainService remindersBlockchainService,
        IUnitOfWork unitOfWork)
    {
        validator = new ReminderViewModelValidator();

        this.mapper = mapper;
        this.logger = logger;
        this.remindersRepository = remindersRepository;
        this.remindersBlockchainService = remindersBlockchainService;
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

        if (reminder.Title is not null)
        {
            var chainId = 0; // TODO: This will need to stored in a separated way.
            var transactionHash = remindersBlockchainService.CreateReminderAsync(reminder.Title).Result;
            var output = remindersBlockchainService.GetReminderAsync(chainId).Result;

            this.logger.LogInformation($"Blockchain: {output.Text} - {output.Owner} - {transactionHash}");
        }

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

        if (reminder.Title is not null)
        {
            var chainId = 0; // TODO: This will need to stored in a separated way.
            var transactionHash = remindersBlockchainService.UpdateReminderAsync(chainId, reminder.Title).Result;
            var output = remindersBlockchainService.GetReminderAsync(chainId).Result;

            this.logger.LogInformation($"Blockchain: {output.Text} - {output.Owner} - {transactionHash}");
        }

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

            var chainId = 0; // TODO: This will need to stored in a separated way.
            var output = remindersBlockchainService.GetReminderAsync(chainId).Result;
            remindersBlockchainService.DeleteReminderAsync(chainId);

            this.logger.LogInformation($"Blockchain: {output.Text} - {output.Owner}");

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

        var chainId = 0; // TODO: This will need to stored in a separated way.
        var output = remindersBlockchainService.GetReminderAsync(chainId).Result;

        this.logger.LogInformation($"Blockchain: {output.Text} - {output.Owner}");

        var reminderViewModel = mapper.Map<ReminderViewModel>(reminder);

        return reminderViewModel;
    }
}