using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Reminders.Api.Models;

namespace Reminders.Application.Test.Models
{
    [TestClass]
    public class ErrorDetailsUnitTest
    {
        #region Constructor Tests

        [TestMethod]
        public void Should_CreateErrorDetails_WithDefaultValues()
        {
            // arrange & act
            var errorDetails = new ErrorDetails();

            // assert
            Assert.IsNotNull(errorDetails);
            Assert.AreEqual(0, errorDetails.StatusCode);
            Assert.IsNull(errorDetails.Message);
            Assert.IsNull(errorDetails.Properties);
        }

        #endregion

        #region Property Tests

        [TestMethod]
        public void Should_SetStatusCode_Correctly()
        {
            // arrange
            var errorDetails = new ErrorDetails();

            // act
            errorDetails.StatusCode = 404;

            // assert
            Assert.AreEqual(404, errorDetails.StatusCode);
        }

        [TestMethod]
        public void Should_SetMessage_Correctly()
        {
            // arrange
            var errorDetails = new ErrorDetails();
            var message = "Test error message";

            // act
            errorDetails.Message = message;

            // assert
            Assert.AreEqual(message, errorDetails.Message);
        }

        [TestMethod]
        public void Should_SetProperties_Correctly()
        {
            // arrange
            var errorDetails = new ErrorDetails();
            var properties = new Dictionary<string, string>
            {
                { "Field1", "Error1" },
                { "Field2", "Error2" }
            };

            // act
            errorDetails.Properties = properties;

            // assert
            Assert.AreSame(properties, errorDetails.Properties);
            Assert.AreEqual(2, errorDetails.Properties.Count);
        }

        #endregion

        #region ToString Tests

        [TestMethod]
        public void Should_ToString_ReturnJsonString_WithAllProperties()
        {
            // arrange
            var errorDetails = new ErrorDetails
            {
                StatusCode = 400,
                Message = "Bad Request",
                Properties = new Dictionary<string, string>
                {
                    { "Field1", "Error1" },
                    { "Field2", "Error2" }
                }
            };

            // act
            var result = errorDetails.ToString();

            // assert
            Assert.IsNotNull(result);
            
            // Deserialize back to verify it's valid JSON
            var deserializedError = JsonConvert.DeserializeObject<ErrorDetails>(result);
            Assert.IsNotNull(deserializedError);
            Assert.AreEqual(400, deserializedError.StatusCode);
            Assert.AreEqual("Bad Request", deserializedError.Message);
            Assert.IsNotNull(deserializedError.Properties);
            Assert.AreEqual(2, deserializedError.Properties.Count);
        }

        [TestMethod]
        public void Should_ToString_ReturnJsonString_WithNullProperties()
        {
            // arrange
            var errorDetails = new ErrorDetails
            {
                StatusCode = 500,
                Message = "Internal Server Error",
                Properties = null
            };

            // act
            var result = errorDetails.ToString();

            // assert
            Assert.IsNotNull(result);
            
            // Deserialize back to verify it's valid JSON
            var deserializedError = JsonConvert.DeserializeObject<ErrorDetails>(result);
            Assert.IsNotNull(deserializedError);
            Assert.AreEqual(500, deserializedError.StatusCode);
            Assert.AreEqual("Internal Server Error", deserializedError.Message);
            Assert.IsNull(deserializedError.Properties);
        }

        [TestMethod]
        public void Should_ToString_ReturnJsonString_WithEmptyProperties()
        {
            // arrange
            var errorDetails = new ErrorDetails
            {
                StatusCode = 422,
                Message = "Unprocessable Entity",
                Properties = new Dictionary<string, string>()
            };

            // act
            var result = errorDetails.ToString();

            // assert
            Assert.IsNotNull(result);
            
            // Deserialize back to verify it's valid JSON
            var deserializedError = JsonConvert.DeserializeObject<ErrorDetails>(result);
            Assert.IsNotNull(deserializedError);
            Assert.AreEqual(422, deserializedError.StatusCode);
            Assert.AreEqual("Unprocessable Entity", deserializedError.Message);
            Assert.IsNotNull(deserializedError.Properties);
            Assert.AreEqual(0, deserializedError.Properties.Count);
        }

        #endregion
    }
}