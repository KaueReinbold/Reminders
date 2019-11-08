using AutoMapper;
using AutoMapper.QueryableExtensions;
using Reminders.Application.Contracts;
using Reminders.Application.ViewModels;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;
using System.Linq;

namespace Reminders.Application.Services
{
    public class RemindersService
        : IRemindersService
    {
        private readonly IMapper mapper;
        private readonly IRemindersRepository remindersRepository;
        private readonly IUnitOfWork unitOfWork;

        public RemindersService(
            IMapper mapper,
            IRemindersRepository remindersRepository,
            IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.remindersRepository = remindersRepository;
            this.unitOfWork = unitOfWork;
        }

        public void Insert(ReminderViewModel reminderViewModel)
        {
            remindersRepository.Add(mapper.Map<Reminder>(reminderViewModel));

            unitOfWork.Commit();
        }

        public void Edit(int id, ReminderViewModel reminderViewModel)
        {
            remindersRepository.Update(mapper.Map<Reminder>(reminderViewModel));

            unitOfWork.Commit();
        }

        public void Delete(int id)
        {
            remindersRepository.Remove(id);

            unitOfWork.Commit();
        }

        public IQueryable<ReminderViewModel> Get() =>
            remindersRepository.Get().ProjectTo<ReminderViewModel>(mapper.ConfigurationProvider);

        public ReminderViewModel Get(int id) =>
            mapper.Map<ReminderViewModel>(remindersRepository.Get(id));
    }
}
