using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Api.Extensions;

namespace Reminders.Application.Test.Extensions
{
    [TestClass]
    public class UtcDateTimeOffsetConverterUnitTest
    {
        private static readonly JsonSerializerOptions Options = CreateOptions();

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new UtcDateTimeOffsetConverter());
            return options;
        }

        [TestMethod]
        public void Should_ReadDateOnly_As_MidnightUtc()
        {
            // act
            var value = JsonSerializer.Deserialize<DateTimeOffset>("\"2026-09-30\"", Options);

            // assert
            Assert.AreEqual(TimeSpan.Zero, value.Offset);
            Assert.AreEqual(new DateTime(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc), value.UtcDateTime);
        }

        [TestMethod]
        public void Should_ReadRfc3339_And_NormalizeToUtc()
        {
            // act
            var value = JsonSerializer.Deserialize<DateTimeOffset>("\"2026-09-30T02:00:00+02:00\"", Options);

            // assert
            Assert.AreEqual(new DateTime(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc), value.UtcDateTime);
        }

        [TestMethod]
        public void Should_RejectAnInvalidDate()
        {
            // act, assert
            Assert.ThrowsException<JsonException>(() =>
                JsonSerializer.Deserialize<DateTimeOffset>("\"not a date\"", Options));
        }

        [TestMethod]
        public void Should_WriteUtcRfc3339()
        {
            // arrange
            var value = new DateTimeOffset(2026, 9, 30, 2, 0, 0, TimeSpan.FromHours(2));

            // act
            var json = JsonSerializer.Serialize(value, Options);

            // assert
            Assert.AreEqual("\"2026-09-30T00:00:00Z\"", json);
        }
    }
}
