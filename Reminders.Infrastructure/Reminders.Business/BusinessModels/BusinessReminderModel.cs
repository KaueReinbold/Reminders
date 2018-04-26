using AutoMapper;
using Microsoft.Extensions.Logging;
using Reminders.Business.Contracts;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Reminders.Business.BusinessModels
{
    public class BusinessReminderModel : IBusinessModelGeneric<ReminderModel>
    {
        private readonly IRepositoryEntityGeneric<ReminderEntity> _repositoryRemindersEntity;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessReminderModel> _logger;

        public BusinessReminderModel(IMapper mapper, ILogger<BusinessReminderModel> logger, IRepositoryEntityGeneric<ReminderEntity> repositoryRemindersEntity)
        {
            _mapper = mapper;
            _logger = logger;
            _repositoryRemindersEntity = repositoryRemindersEntity;
        }

        public bool Delete(int key)
        {
            try
            {
                var reminder = _repositoryRemindersEntity.Find(key);

                var result = _repositoryRemindersEntity.Delete(reminder);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);

                return false;
            }
        }

        public ReminderModel Find(int key)
        {
            try
            {
                var reminder = _repositoryRemindersEntity.Find(key);

                return _mapper.Map<ReminderModel>(reminder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);

                return new ReminderModel();
            }
        }

        public List<ReminderModel> GetAll()
        {
            try
            {
                var reminders = _repositoryRemindersEntity.GetAll();

                return _mapper.Map<List<ReminderModel>>(reminders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);

                return new List<ReminderModel>();
            }
        }

        public List<ReminderModel> GetAll(Func<ReminderModel, bool> func)
        {
            try
            {
                Expression<Func<ReminderEntity, ReminderModel>> mapper = Mapper.Engine.CreateMapExpression<ReminderEntity, ReminderModel>();

                Expression<Func<ReminderEntity, bool>> mappedSelector = func.Compose(mapper);

                var reminders = _repositoryRemindersEntity.GetAll(mappedSelector);

                return _mapper.Map<List<ReminderModel>>(reminders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);

                return new List<ReminderModel>();
            }
        }

        public ReminderModel Insert(ReminderModel model)
        {
            try
            {
                var reminder = _mapper.Map<ReminderEntity>(model);

                reminder = _repositoryRemindersEntity.Insert(reminder);

                if (reminder.Id > 0)
                    return _mapper.Map<ReminderModel>(reminder);

                return new ReminderModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);

                return new ReminderModel();
            }
        }

        public bool Update(ReminderModel model)
        {
            try
            {
                var reminder = _mapper.Map<ReminderEntity>(model);

                var result = _repositoryRemindersEntity.Update(reminder);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);

                return false;
            }
        }

        public bool CompleteReminder(int key)
        {
            try
            {
                var reminder = _repositoryRemindersEntity.Find(key);

                if (!reminder.IsDone)
                {
                    reminder.IsDone = true;

                    _repositoryRemindersEntity.Update(reminder);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);

                return false;
            }
        }
    }
}
