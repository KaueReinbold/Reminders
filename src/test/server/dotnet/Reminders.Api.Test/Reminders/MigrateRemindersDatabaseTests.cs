using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Reminders.Infrastructure.Data.EntityFramework.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Api.Extensions;
using Reminders.Application.Extensions;

namespace Reminders.Api.Test.Reminders;

[TestClass]
public class MigrateRemindersDatabaseTests
{
    [TestMethod]
    public void MigrateRemindersDatabase_WhenMigratorThrows_LogsError()
    {
        var builder = WebApplication.CreateBuilder(new string[] { });

        // configure retry to be quick
        builder.Configuration["DatabaseRetry:MaxAttempts"] = "1";
        builder.Configuration["DatabaseRetry:BaseSeconds"] = "0";

        var testLoggerProvider = new TestLoggerProvider();
        var loggerFactory = LoggerFactory.Create(lb => lb.AddProvider(testLoggerProvider));
        builder.Services.AddSingleton<ILoggerFactory>(loggerFactory);

        // create a service provider that contains a throwing IMigrator and attach it to the DbContext
        var internalServices = new ServiceCollection();
        internalServices.AddSingleton<IMigrator>(new ThrowingMigrator());
        var internalProvider = internalServices.BuildServiceProvider();

        var ctx = new FakeContext(internalProvider);
        builder.Services.AddSingleton<RemindersContext>(ctx);

        var app = builder.Build();

        app.MigrateRemindersDatabase();

        var hasError = testLoggerProvider.Logs.Any(l => l.LogLevel == LogLevel.Error && l.Message.Contains("Failed to apply database migrations"));

        Assert.IsTrue(hasError, "Expected an error log indicating migrations failed after retries.");
    }

    private class ThrowingMigrator : IMigrator
    {
        public string GenerateScript(string fromMigration, string toMigration, MigrationsSqlGenerationOptions options) => string.Empty;

        public void Migrate(string targetMigration)
        {
            throw new InvalidOperationException("simulated migration failure");
        }
        public System.Threading.Tasks.Task MigrateAsync(string targetMigration, System.Threading.CancellationToken cancellationToken = default) => throw new InvalidOperationException("simulated migration failure");
    }

    private class FakeContext : RemindersContext, Microsoft.EntityFrameworkCore.Infrastructure.IInfrastructure<IServiceProvider>
    {
        private readonly IServiceProvider _svc;
        public FakeContext(IServiceProvider svc) { _svc = svc; }

        public IServiceProvider GetInfrastructure() => _svc;
    }
}
