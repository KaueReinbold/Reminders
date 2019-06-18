using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Domain.AutoMapperProfile;

namespace Reminders.Domain.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Extensions to add auto mapper profile
        /// </summary>
        /// <param name="services"></param>
        public static void AddAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);
        }
    }
}
