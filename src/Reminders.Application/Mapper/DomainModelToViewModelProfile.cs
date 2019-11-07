using AutoMapper;
using Reminders.Application.ViewModels;
using Reminders.Domain.Models;

namespace Reminders.Application.Mapper
{
    public class DomainModelToViewModelProfile
        : Profile
    {
        public DomainModelToViewModelProfile()
        {
            CreateMap<Reminder, ReminderViewModel>();
        }
    }
}
