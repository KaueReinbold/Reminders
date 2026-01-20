using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Reminders.Api.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Reminders.Api.Test.Reminders;

[TestClass]
public class EnsureDatabaseAvailableTests
{
    [TestMethod]
    public void EnsureDatabaseAvailable_WithoutRemindersContext_LogsError()
    {
        var builder = WebApplication.CreateBuilder(new string[] { });

        // configure retry to be quick
        builder.Configuration["DatabaseRetry:MaxAttempts"] = "1";
        builder.Configuration["DatabaseRetry:BaseSeconds"] = "0";

        var testLoggerProvider = new TestLoggerProvider();
        var loggerFactory = LoggerFactory.Create(lb => lb.AddProvider(testLoggerProvider));
        builder.Services.AddSingleton<ILoggerFactory>(loggerFactory);

        var app = builder.Build();

        // Do NOT register RemindersContext to simulate missing DB

        app.EnsureDatabaseAvailable();

        // Expect at least one error log about connectivity
        var hasError = testLoggerProvider.Logs.Any(l => l.LogLevel == LogLevel.Error && l.Message.Contains("Database connectivity could not be established"));

        Assert.IsTrue(hasError, "Expected an error log indicating database connectivity failure.");
    }
}

internal class TestLoggerProvider : ILoggerProvider
{
    public List<LogEntry> Logs { get; } = new();

    public ILogger CreateLogger(string categoryName) => new TestLogger(Logs, categoryName);

    public void Dispose() { }

    private class TestLogger : ILogger
    {
        private readonly List<LogEntry> _logs;
        private readonly string _category;

        public TestLogger(List<LogEntry> logs, string category)
        {
            _logs = logs;
            _category = category;
        }

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            _logs.Add(new LogEntry { LogLevel = logLevel, Message = message, Exception = exception, Category = _category });
        }
    }

    internal class LogEntry
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }

}
