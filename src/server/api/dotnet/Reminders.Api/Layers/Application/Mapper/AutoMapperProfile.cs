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