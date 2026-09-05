using System.Globalization;
using JsonException = System.Text.Json.JsonException;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Utf8JsonReader = System.Text.Json.Utf8JsonReader;
using Utf8JsonWriter = System.Text.Json.Utf8JsonWriter;
using JsonConverter = System.Text.Json.Serialization.JsonConverter<System.DateTimeOffset>;

namespace Reminders.Api.Extensions
{
    // limitDate accepts "YYYY-MM-DD" (midnight UTC) and RFC3339, and always
    // serializes RFC3339 in UTC, so all three API implementations agree (ADR-0011).
    public class UtcDateTimeOffsetConverter : JsonConverter
    {
        private const string DateOnlyFormat = "yyyy-MM-dd";

        public override DateTimeOffset Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                throw new JsonException("The limit date is required.");

            if (DateTimeOffset.TryParseExact(
                    value,
                    DateOnlyFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dateOnly))
                return dateOnly;

            if (DateTimeOffset.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var timestamp))
                return timestamp;

            throw new JsonException($"'{value}' is not a valid date.");
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTimeOffset value,
            JsonSerializerOptions options) =>
            writer.WriteStringValue(value.UtcDateTime);
    }
}
