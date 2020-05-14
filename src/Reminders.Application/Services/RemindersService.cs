﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Reminders.Application.Contracts;
using Reminders.Application.Validators.Reminders;
using Reminders.Application.Validators.Reminders.Exceptions;
using Reminders.Application.Validators.Reminders.Exceptions.Enumerables;
using Reminders.Application.Validators.Reminders.Resources;
using Reminders.Application.ViewModels;
using Reminders.Domain.Contracts;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;
using System;
using System.Linq;

namespace Reminders.Application.Services
{
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

            validator.ValidateAndThrow(reminderViewModel, ruleSet: "*");

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

            if (id != reminderViewModel.Id)
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

            reminderData.Delete();

            remindersRepository.Update(reminderData);

            unitOfWork.Commit();
        }

        public IQueryable<ReminderViewModel> Get() =>
            remindersRepository
                .Get()
                .Where(reminder => !reminder.IsDeleted)
                .ProjectTo<ReminderViewModel>(mapper.ConfigurationProvider);

        public ReminderViewModel Get(Guid id) =>
            mapper.Map<ReminderViewModel>(
                remindersRepository
                .Get()
                .FirstOrDefault(reminder =>
                    reminder.Id == id &&
                    !reminder.IsDeleted));

        public IQueryable<ReminderViewModel> GetInactive() =>
            remindersRepository
                .Get()
                .Where(reminder => reminder.IsDeleted)
                .ProjectTo<ReminderViewModel>(mapper.ConfigurationProvider);

        public ReminderViewModel GetInactive(Guid id) =>
            mapper.Map<ReminderViewModel>(
                remindersRepository
                .Get()
                .FirstOrDefault(reminder =>
                    reminder.Id == id &&
                    reminder.IsDeleted));
    }
}
