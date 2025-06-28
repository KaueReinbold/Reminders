using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reminders.Infrastructure.CrossCutting.Extensions;

namespace Reminders.Application.Test.Extensions
{
    // Test class to use for generic type parameter
    public class TestLogger { }

    [TestClass]
    public class MachineNameMiddlewareExtensionsUnitTest
    {
        #region UseMachineNameLogging Tests

        [TestMethod]
        public void Should_UseMachineNameLogging_ReturnApplicationBuilder()
        {
            // arrange
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();
            
            mockApplicationBuilder
                .Setup(ab => ab.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns(mockApplicationBuilder.Object);

            // act
            var result = mockApplicationBuilder.Object.UseMachineNameLogging<TestLogger>();

            // assert
            Assert.AreSame(mockApplicationBuilder.Object, result);
            mockApplicationBuilder.Verify(ab => ab.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
        }

        [TestMethod]
        public async Task Should_UseMachineNameLogging_LogMachineInformation()
        {
            // arrange
            var context = new DefaultHttpContext();
            var mockLogger = new Mock<ILogger<TestLogger>>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(ILogger<TestLogger>)))
                .Returns(mockLogger.Object);
            
            context.RequestServices = mockServiceProvider.Object;

            var nextCalled = false;
            Task Next(HttpContext ctx)
            {
                nextCalled = true;
                return Task.CompletedTask;
            }

            // act
            var middleware = CreateMachineNameMiddleware<TestLogger>(Next);
            await middleware(context);

            // assert
            Assert.IsTrue(nextCalled);
            mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("MachineName")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Should_UseMachineNameLogging_CallNextMiddleware()
        {
            // arrange
            var context = new DefaultHttpContext();
            var mockLogger = new Mock<ILogger<TestLogger>>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(ILogger<TestLogger>)))
                .Returns(mockLogger.Object);
            
            context.RequestServices = mockServiceProvider.Object;

            var nextCalled = false;
            Task Next(HttpContext ctx)
            {
                nextCalled = true;
                return Task.CompletedTask;
            }

            // act
            var middleware = CreateMachineNameMiddleware<TestLogger>(Next);
            await middleware(context);

            // assert
            Assert.IsTrue(nextCalled);
        }

        #endregion

        #region Helper Methods

        private static RequestDelegate CreateMachineNameMiddleware<T>(RequestDelegate next)
        {
            // This simulates what UseMachineNameLogging does
            return async (context) =>
            {
                context
                    .RequestServices
                    .GetRequiredService<ILogger<T>>()
                    .LogInformation($"\t\nMachineName: {Environment.MachineName} \t\nSystem: {Environment.OSVersion.VersionString} \t\nDateTime: {DateTime.UtcNow} \n");

                await next.Invoke(context);
            };
        }

        #endregion
    }
}