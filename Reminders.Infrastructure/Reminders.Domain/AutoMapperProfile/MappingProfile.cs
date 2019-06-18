using AutoMapper;
using Reminders.Domain.Entities;
using Reminders.Domain.Models;

namespace Reminders.Domain.AutoMapperProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ReminderModel, ReminderEntity>();
            CreateMap<ReminderEntity, ReminderModel>();
        }
    }
}
