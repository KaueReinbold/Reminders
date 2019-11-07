using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Reminders.Application.Mapper.Extensions
{
    public static class AutoMapperConfiguration
    {
        public static IServiceCollection RegisterAutoMapper(
            this IServiceCollection services) =>
            services
                .AddSingleton(new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new DomainModelToViewModelProfile());
                    cfg.AddProfile(new ViewModelToDomainModelProfile());
                }).CreateMapper())
            ;
    }
}
