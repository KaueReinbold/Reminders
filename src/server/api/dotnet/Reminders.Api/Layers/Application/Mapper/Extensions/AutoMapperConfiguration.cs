namespace Reminders.Application.Mapper.Extensions;

public static class AutoMapperConfiguration
{
    public static IMapper CreateMapper() =>
        new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperProfile());
        }).CreateMapper();
}