using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Api.Extensions;
using Reminders.Application.Validators.Reminders.Exceptions;
using Reminders.Application.Validators.Reminders.Exceptions.Enumerables;

namespace Reminders.Application.Test.Extensions
{
    [TestClass]
    public class RemindersApplicationExceptionExtensionsUnitTest
    {
        #region ToHttpStatusCode Tests

        [TestMethod]
        public void Should_ToHttpStatusCode_ReturnNotFound_WhenValidationStatusIsNotFound()
        {
            // arrange
            var exception = new RemindersApplicationException(ValidationStatus.NotFound, "Test message");

            // act
            var result = exception.ToHttpStatusCode();

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, result);
        }

        [TestMethod]
        public void Should_ToHttpStatusCode_ReturnConflict_WhenValidationStatusIsIdsDoNotMatch()
        {
            // arrange
            var exception = new RemindersApplicationException(ValidationStatus.IdsDoNotMatch, "Test message");

            // act
            var result = exception.ToHttpStatusCode();

            // assert
            Assert.AreEqual(HttpStatusCode.Conflict, result);
        }

        [TestMethod]
        public void Should_ToHttpStatusCode_ReturnBadRequest_ForDefaultCase()
        {
            // arrange
            // Using a cast to simulate an unmapped ValidationStatus
            var exception = new RemindersApplicationException((ValidationStatus)999, "Test message");

            // act
            var result = exception.ToHttpStatusCode();

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result);
        }

        #endregion
    }
}