using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reminders.Infrastructure.CrossCutting.Configuration;

namespace Reminders.Infrastructure.Data.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static void ConfigureSqlServer(
            this DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = IConfigurationHelper.GetConfiguration();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }
}