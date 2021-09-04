using AutoMapper;

namespace Reminders.Application.Mapper.Extensions
{
    public static class AutoMapperConfiguration
    {
        public static IMapper CreateMapper() =>
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainModelToViewModelProfile());
                cfg.AddProfile(new ViewModelToDomainModelProfile());
            }).CreateMapper();
    }
}