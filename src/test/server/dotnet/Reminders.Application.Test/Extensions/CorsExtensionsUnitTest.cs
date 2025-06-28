using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reminders.Api.Extensions;

namespace Reminders.Application.Test.Extensions
{
    [TestClass]
    public class CorsExtensionsUnitTest
    {
        #region AddRemindersCors Tests

        [TestMethod]
        public void Should_AddRemindersCors_ReturnServiceCollection()
        {
            // arrange
            var services = new ServiceCollection();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["CorsOrigins"]).Returns("http://localhost:3000,https://example.com");

            // act
            var result = services.AddRemindersCors(mockConfiguration.Object);

            // assert
            Assert.AreSame(services, result);
            Assert.IsTrue(services.Count > 0); // CORS services should be added
        }

        [TestMethod]
        public void Should_AddRemindersCors_WithNullOrigins()
        {
            // arrange
            var services = new ServiceCollection();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["CorsOrigins"]).Returns((string)null);

            // act
            var result = services.AddRemindersCors(mockConfiguration.Object);

            // assert
            Assert.AreSame(services, result);
            Assert.IsTrue(services.Count > 0); // CORS services should be added even with null origins
        }

        [TestMethod]
        public void Should_AddRemindersCors_WithEmptyOrigins()
        {
            // arrange
            var services = new ServiceCollection();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["CorsOrigins"]).Returns("");

            // act
            var result = services.AddRemindersCors(mockConfiguration.Object);

            // assert
            Assert.AreSame(services, result);
            Assert.IsTrue(services.Count > 0); // CORS services should be added
        }

        [TestMethod]
        public void Should_AddRemindersCors_WithMultipleOrigins()
        {
            // arrange
            var services = new ServiceCollection();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["CorsOrigins"]).Returns("http://localhost:3000,https://example.com,https://test.com");

            // act
            var result = services.AddRemindersCors(mockConfiguration.Object);

            // assert
            Assert.AreSame(services, result);
            Assert.IsTrue(services.Count > 0); // CORS services should be added
        }

        #endregion

        #region UseRemindersCors Tests

        [TestMethod]
        public void Should_UseRemindersCors_ReturnApplicationBuilder()
        {
            // arrange
            var mockApplicationBuilder = new Mock<IApplicationBuilder>();
            mockApplicationBuilder
                .Setup(ab => ab.Use(It.IsAny<System.Func<Microsoft.AspNetCore.Http.RequestDelegate, Microsoft.AspNetCore.Http.RequestDelegate>>()))
                .Returns(mockApplicationBuilder.Object);

            // act
            var result = CorsExtensions.UseRemindersCors(mockApplicationBuilder.Object);

            // assert
            Assert.AreSame(mockApplicationBuilder.Object, result);
        }

        #endregion

        #region Constants Tests

        [TestMethod]
        public void Should_RemindersCorsPolicyName_HaveCorrectValue()
        {
            // arrange & act
            var policyName = CorsExtensions.RemindersCorsPolicyName;

            // assert
            Assert.AreEqual("AllowSpecificOrigin", policyName);
        }

        #endregion
    }
}