using AutoMapper;
using Microsoft.Extensions.Logging;
using Reminders.Business.Contracts;
using Reminders.Business.Contracts.Business;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reminders.Business.BusinessModels
{
    public class BusinessReminderModel : IBusinessModelGeneric<ReminderModel>
    {
        private readonly IUnitOfWork _unityOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessReminderModel> _logger;

        public BusinessReminderModel(
            IMapper mapper, 
            ILogger<BusinessReminderModel> logger, 
            IUnitOfWork unityOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unityOfWork = unityOfWork;
        }
        
        public ReminderModel Find(int key)
        {
            try
            {
                var reminder = _unityOfWork.RemindersRepository.SingleOrDefault(r => r.Id == key);
                
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
                var reminders = _unityOfWork.RemindersRepository.GetAll();

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

                _unityOfWork.RemindersRepository.Add(reminder);

                _unityOfWork.Complete();

                var reminderInserted = _mapper.Map<ReminderModel>(reminder);

                return reminderInserted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return new ReminderModel();
            }
        }

        public bool Update(ReminderModel model)
        {
            try
            {
                var reminder = _unityOfWork.RemindersRepository.SingleOrDefault(r => r.Id == model.Id);

                reminder.Title = model.Title;
                reminder.Description = model.Description;
                reminder.IsDone = model.IsDone;
                reminder.LimitDate = model.LimitDate;

                return _unityOfWork.Complete() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public bool Delete(int key)
        {
            try
            {
                var reminder = _unityOfWork.RemindersRepository.SingleOrDefault(r => r.Id == key);

                _unityOfWork.RemindersRepository.Remove(reminder);

                return _unityOfWork.Complete() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public Task<List<ReminderModel>> GetAllAsync()
        {
            try
            {
                var reminders = _unityOfWork.RemindersRepository.GetAllAsync();
                
                return Task.Run(() => _mapper.Map<List<ReminderModel>>(reminders));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);

                return Task.Run(() => new List<ReminderModel>());
            }
        }
    }        
}
