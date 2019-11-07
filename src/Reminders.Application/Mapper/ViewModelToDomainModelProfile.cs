using AutoMapper;
using Reminders.Application.ViewModels;
using Reminders.Domain.Models;

namespace Reminders.Application.Mapper
{
    public class ViewModelToDomainModelProfile
        : Profile
    {
        public ViewModelToDomainModelProfile()
        {
            CreateMap<ReminderViewModel, Reminder>();
        }
    }
}
