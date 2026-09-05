using System;
using System.Collections.Generic;
using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Api.Extensions;
using Reminders.Application.Validators.Reminders.Exceptions;
using Reminders.Application.Validators.Reminders.Exceptions.Enumerables;

namespace Reminders.Application.Test.Extensions
{
    [TestClass]
    public class ExceptionMiddlewareExtensionsUnitTest
    {
        #region RemindersProblemDetailsFactory Tests

        [TestMethod]
        public void Should_MapRemindersApplicationException_To_NotFound()
        {
            // arrange
            var exception = new RemindersApplicationException(ValidationStatus.NotFound, "Not found");

            // act
            var problemDetails = RemindersProblemDetailsFactory.Create(exception);

            // assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, problemDetails.Status);
            Assert.AreEqual("Not found", problemDetails.Title);
            Assert.AreEqual(RemindersProblemDetailsFactory.ClientErrorType, problemDetails.Type);
        }

        [TestMethod]
        public void Should_MapRemindersApplicationException_To_Conflict()
        {
            // arrange
            var exception = new RemindersApplicationException(ValidationStatus.IdsDoNotMatch, "IDs do not match");

            // act
            var problemDetails = RemindersProblemDetailsFactory.Create(exception);

            // assert
            Assert.AreEqual((int)HttpStatusCode.Conflict, problemDetails.Status);
            Assert.AreEqual("IDs do not match", problemDetails.Title);
        }

        [TestMethod]
        public void Should_MapValidationException_To_BadRequest_With_FieldErrors()
        {
            // arrange
            var exception = new ValidationException(new List<ValidationFailure>
            {
                new ValidationFailure("title", "Title is required"),
                new ValidationFailure("limitDate", "The limit date must be later than today."),
                new ValidationFailure("limitDate", "Second failure on the same field")
            });

            // act
            var problemDetails = RemindersProblemDetailsFactory.Create(exception);

            // assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, problemDetails.Status);
            Assert.AreEqual("One or more validation errors occurred.", problemDetails.Title);

            var validationProblemDetails = problemDetails as ValidationProblemDetails;
            Assert.IsNotNull(validationProblemDetails);
            Assert.AreEqual(2, validationProblemDetails.Errors.Count);
            Assert.AreEqual(1, validationProblemDetails.Errors["title"].Length);
            Assert.AreEqual(2, validationProblemDetails.Errors["limitDate"].Length);
        }

        [TestMethod]
        public void Should_MapUnknownException_To_InternalServerError()
        {
            // arrange
            var exception = new Exception("Unexpected error");

            // act
            var problemDetails = RemindersProblemDetailsFactory.Create(exception);

            // assert
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, problemDetails.Status);
            Assert.AreEqual("Internal Server Error.", problemDetails.Title);
            Assert.AreEqual(RemindersProblemDetailsFactory.ServerErrorType, problemDetails.Type);
            Assert.IsNull(problemDetails as ValidationProblemDetails);
        }

        [TestMethod]
        public void Should_ExposeProblemDetailsContentType()
        {
            // assert
            Assert.AreEqual("application/problem+json", ExceptionMiddlewareExtensions.ProblemDetailsContentType);
        }

        #endregion
    }
}
