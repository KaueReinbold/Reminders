using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Reminders.Infrastructure.Data.EntityFramework.Extensions
{
  public static class DbContextOptionsBuilderExtensions
  {
    public static void ConfigureSqlServer(
        this DbContextOptionsBuilder optionsBuilder)
    {
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

      var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables()
          .Build();

      optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }
  }
}