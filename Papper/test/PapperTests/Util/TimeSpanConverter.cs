using System;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Papper.Tests.Util
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        private readonly string _format;

        private readonly IFormatProvider _formatProvider;

        public TimeSpanConverter(string format = "c", IFormatProvider? formatProvider = null)
        {
            _format = format ?? throw new ArgumentNullException(nameof(format));
            _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    {
                        if (TimeSpan.TryParseExact(reader.GetString(), _format, _formatProvider, out var result2))
                        {
                            return result2;
                        }

                        break;
                    }
                case JsonTokenType.StartObject:
                    {
                        string text = string.Empty;
                        TimeSpan result = TimeSpan.Zero;
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                        {
                            if (reader.TokenType == JsonTokenType.PropertyName)
                            {
                                text = reader.GetString() ?? String.Empty;
                            }
                            else if (text != null && text.Equals("Ticks", StringComparison.InvariantCultureIgnoreCase) && reader.TryGetInt64(out long value))
                            {
                                result = new TimeSpan(value);
                            }
                        }

                        return result;
                    }
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format, _formatProvider));
        }
    }

}
