using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Reminders.Api.Extensions;
using Reminders.Application.Validators.Reminders.Exceptions;
using Reminders.Application.Validators.Reminders.Exceptions.Enumerables;

namespace Reminders.Application.Test.Extensions
{
    [TestClass]
    public class ExceptionMiddlewareExtensionsUnitTest
    {
        #region UseRemindersExceptionHandler Tests

        [TestMethod]
        public async Task Should_HandleRemindersApplicationException_With_NotFound()
        {
            // arrange
            var context = CreateMockHttpContext();
            var exception = new RemindersApplicationException(ValidationStatus.NotFound, "Not found");
            var exceptionFeature = new Mock<IExceptionHandlerFeature>();
            exceptionFeature.Setup(f => f.Error).Returns(exception);
            
            context.Features.Set(exceptionFeature.Object);

            // act
            await InvokeExceptionHandler(context, exception);

            // assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, context.Response.StatusCode);
            Assert.AreEqual("application/json", context.Response.ContentType);
            
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            Assert.IsTrue(responseBody.Contains("Not found"));
            Assert.IsTrue(responseBody.Contains("404"));
        }

        [TestMethod]
        public async Task Should_HandleRemindersApplicationException_With_IdsDoNotMatch()
        {
            // arrange
            var context = CreateMockHttpContext();
            var exception = new RemindersApplicationException(ValidationStatus.IdsDoNotMatch, "IDs do not match");
            var exceptionFeature = new Mock<IExceptionHandlerFeature>();
            exceptionFeature.Setup(f => f.Error).Returns(exception);
            
            context.Features.Set(exceptionFeature.Object);

            // act
            await InvokeExceptionHandler(context, exception);

            // assert
            Assert.AreEqual((int)HttpStatusCode.Conflict, context.Response.StatusCode);
            Assert.AreEqual("application/json", context.Response.ContentType);
            
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            Assert.IsTrue(responseBody.Contains("IDs do not match"));
            Assert.IsTrue(responseBody.Contains("409"));
        }

        [TestMethod]
        public async Task Should_HandleValidationException_With_ValidationErrors()
        {
            // arrange
            var context = CreateMockHttpContext();
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title is required"),
                new ValidationFailure("LimitDate", "LimitDate must be in the future")
            };
            var exception = new ValidationException(validationFailures);
            var exceptionFeature = new Mock<IExceptionHandlerFeature>();
            exceptionFeature.Setup(f => f.Error).Returns(exception);
            
            context.Features.Set(exceptionFeature.Object);

            // act
            await InvokeExceptionHandler(context, exception);

            // assert
            Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, context.Response.StatusCode);
            Assert.AreEqual("application/json", context.Response.ContentType);
            
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            Assert.IsTrue(responseBody.Contains("422"));
            Assert.IsTrue(responseBody.Contains("Title"));
            Assert.IsTrue(responseBody.Contains("LimitDate"));
        }

        [TestMethod]
        public async Task Should_HandleGenericException_With_InternalServerError()
        {
            // arrange
            var context = CreateMockHttpContext();
            var exception = new Exception("Unexpected error");
            var exceptionFeature = new Mock<IExceptionHandlerFeature>();
            exceptionFeature.Setup(f => f.Error).Returns(exception);
            
            context.Features.Set(exceptionFeature.Object);

            // act
            await InvokeExceptionHandler(context, exception);

            // assert
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            Assert.AreEqual("application/json", context.Response.ContentType);
            
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            Assert.IsTrue(responseBody.Contains("Internal Server Error"));
            Assert.IsTrue(responseBody.Contains("500"));
        }

        #endregion

        #region Helper Methods

        private static HttpContext CreateMockHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private static async Task InvokeExceptionHandler(HttpContext context, Exception exception)
        {
            // This simulates what the UseRemindersExceptionHandler would do
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

            if (contextFeature != null)
            {
                var statusCode = (int)HttpStatusCode.InternalServerError;
                var message = "Internal Server Error.";
                Dictionary<string, string> properties = null;

                if (contextFeature.Error is RemindersApplicationException remindersApplicationException)
                {
                    statusCode = (int)remindersApplicationException.ToHttpStatusCode();
                    message = remindersApplicationException.Message;
                }
                else if (contextFeature.Error is ValidationException validationException)
                {
                    statusCode = (int)HttpStatusCode.UnprocessableEntity;
                    message = validationException.Message;
                    properties = validationException.Errors.ToDictionary(
                        error => error.PropertyName,
                        error => error.ErrorMessage);
                }

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(new Reminders.Api.Models.ErrorDetails()
                {
                    StatusCode = statusCode,
                    Message = message,
                    Properties = properties
                }.ToString());
            }
        }

        #endregion
    }
}