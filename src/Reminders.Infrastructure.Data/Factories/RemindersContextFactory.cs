using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;
using System;

namespace Reminders.Infrastructure.Data.Factories
{
    public class RemindersContextFactory
        : IDesignTimeDbContextFactory<RemindersContext>
    {
        public RemindersContext CreateDbContext(string[] args) =>
            new RemindersContext(new DbContextOptionsBuilder<RemindersContext>()
                .UseSqlServer(new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build().GetConnectionString("DefaultConnection"))
                .Options);
    }
}
