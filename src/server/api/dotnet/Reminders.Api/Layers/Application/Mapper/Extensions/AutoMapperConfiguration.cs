using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Reminders.Application.Mapper.Extensions;

public static class AutoMapperConfiguration
{
    public static IMapper CreateMapper(ILoggerFactory? loggerFactory = null) =>
        new MapperConfiguration(
            (IMapperConfigurationExpression cfg) => cfg.AddProfile(new AutoMapperProfile()),
            loggerFactory ?? NullLoggerFactory.Instance
        ).CreateMapper();
}