using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Papper.Tests.Util
{

    public class MultiByteArrayConverter : JsonConverter<byte[][]>
    {
        private readonly string _format;

        private readonly IFormatProvider _formatProvider;

        public MultiByteArrayConverter(string format = "c", IFormatProvider? formatProvider = null)
        {
            _format = (format ?? throw new ArgumentNullException(nameof(format)));
            _formatProvider = (formatProvider ?? CultureInfo.InvariantCulture);
        }

        public override byte[][]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    {
                        var dim1 = new List<List<byte>>();
                        int indent = 0;
                        while (reader.Read() && (reader.TokenType != JsonTokenType.EndArray || indent > 0))
                        {
                            if (reader.TokenType == JsonTokenType.StartArray)
                            {
                                dim1.Add(new List<byte>());
                                indent++;
                            }
                            else if (reader.TokenType == JsonTokenType.EndArray)
                            {
                                indent--;
                            }
                            else if (reader.TokenType == JsonTokenType.Number)
                            {
                                dim1[^1].Add(reader.GetByte());
                            }
                        }



                        var result = new byte[dim1.Count][];
                        for (int i = 0; i < dim1.Count; i++)
                        {
                            result[i] = new byte[dim1[i].Count];
                            var c = dim1[i];
                            for (int j = 0; j < dim1.Count; j++)
                            {
                                result[i][j] = c[j];
                            }
                        }

                        return result;

                    }
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, byte[][] value, JsonSerializerOptions options)
        {
            //writer.WriteStringValue(value.ToString(_format, _formatProvider));
        }
    }

}
