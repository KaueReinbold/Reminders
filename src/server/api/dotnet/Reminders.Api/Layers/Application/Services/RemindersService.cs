using Microsoft.Extensions.Options;

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
    private readonly BlockchainSettings blockchainSettings;

    public RemindersService(
        ILogger<RemindersService> logger,
        IMapper mapper,
        IRemindersRepository remindersRepository,
        IRemindersBlockchainService remindersBlockchainService,
        IUnitOfWork unitOfWork,
        IOptions<BlockchainSettings> blockchainSettings)
    {
        validator = new ReminderViewModelValidator();

        this.mapper = mapper;
        this.logger = logger;
        this.remindersRepository = remindersRepository;
        this.remindersBlockchainService = remindersBlockchainService;
        this.unitOfWork = unitOfWork;
        this.blockchainSettings = blockchainSettings.Value;
    }

    public async Task<ReminderViewModel> InsertAsync(ReminderViewModel reminderViewModel)
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
            try
            {
                var chainId = blockchainSettings.ChainId;
                var transactionHash = await remindersBlockchainService.CreateReminderAsync(reminder.Title);
                var output = await remindersBlockchainService.GetReminderAsync(chainId);

                this.logger.LogInformation($"Blockchain: {output.Text} - {output.Owner} - {transactionHash}");
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Blockchain create failed, reminder saved to database only");
            }
        }

        unitOfWork.Commit();


        return mapper.Map<ReminderViewModel>(reminder);
    }

    public async Task<ReminderViewModel> EditAsync(
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
            try
            {
                var chainId = blockchainSettings.ChainId;
                var transactionHash = await remindersBlockchainService.UpdateReminderAsync(chainId, reminder.Title);
                var output = await remindersBlockchainService.GetReminderAsync(chainId);

                this.logger.LogInformation($"Blockchain: {output.Text} - {output.Owner} - {transactionHash}");
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Blockchain update failed, reminder saved to database only");
            }
        }

        unitOfWork.Commit();

        return mapper.Map<ReminderViewModel>(reminder);
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!remindersRepository.Exists(id))
            throw new RemindersApplicationException(ValidationStatus.NotFound, RemindersResources.NotFound);

        var reminderData = remindersRepository.Get(id);

        if (reminderData is not null)
        {
            reminderData.Delete();

            try
            {
                var chainId = blockchainSettings.ChainId;
                var output = await remindersBlockchainService.GetReminderAsync(chainId);
                await remindersBlockchainService.DeleteReminderAsync(chainId);

                this.logger.LogInformation($"Blockchain: {output.Text} - {output.Owner}");
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Blockchain delete failed, reminder deleted from database only");
            }

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

    public async Task<ReminderViewModel> GetAsync(Guid id)
    {
        var reminder = remindersRepository
            .Get()
            .FirstOrDefault(reminder => reminder.Id == id && !reminder.IsDeleted);

        // A missing reminder is a 404, not an empty 204 (ADR-0011).
        if (reminder is null)
            throw new RemindersApplicationException(ValidationStatus.NotFound, RemindersResources.NotFound);

        try
        {
            var chainId = blockchainSettings.ChainId;
            var output = await remindersBlockchainService.GetReminderAsync(chainId);

            this.logger.LogInformation($"Blockchain: {output.Text} - {output.Owner}");
        }
        catch (Exception exception)
        {
            this.logger.LogError(exception, "Blockchain read failed, returning database data only");
        }

        var reminderViewModel = mapper.Map<ReminderViewModel>(reminder);

        return reminderViewModel;
    }
}