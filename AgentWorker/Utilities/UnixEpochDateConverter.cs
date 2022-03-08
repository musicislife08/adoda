using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Char;

namespace AgentWorker.Utilities
{
    internal sealed class UnixEpochDateConverter : JsonConverter<DateTimeOffset>
    {
        private static readonly DateTimeOffset SEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            // Very special parsing for undocumented Azure DevOps dates
            var formatted = new string((reader.GetString() ?? throw new InvalidOperationException()).Where(IsDigit)
                .ToArray());
            if (!long.TryParse(formatted, out var unixEpochValue))
                throw new Exception("Unexpected value format, unable to parse DateTimeOffset.");
            return DateTimeOffset.FromUnixTimeMilliseconds(unixEpochValue);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            var unixTime = Convert.ToInt64((value - SEpoch).TotalMilliseconds);
            var utcOffset = value.Offset;
            var formatted =
                FormattableString.Invariant(
                    $"/Date({unixTime}{(utcOffset >= TimeSpan.Zero ? "+" : "-")}{utcOffset:hhmm})/");
            writer.WriteStringValue(formatted);
        }
    }
}