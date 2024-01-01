using AutoMapper;
using Reminders.Application.ViewModels;
using Reminders.Domain.Models;

namespace Reminders.Application.Mapper;

public class AutoMapperProfile
    : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Reminder, ReminderViewModel>()
            .ReverseMap();
    }
}