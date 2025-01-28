using System;
using System.Text.Json;

namespace Papper.Tests.Util
{
    public class DefaultJsonObjectSerializer
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public DefaultJsonObjectSerializer()
        {
            _serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
            _serializerOptions.Converters.Add(new MultiByteArrayConverter());
            _serializerOptions.Converters.Add(new TimeSpanConverter());
        }

        public string Serialize(Type t, object blockData) => JsonSerializer.Serialize(blockData, t, _serializerOptions);

        public object? Deserialize(Type t, string blockData) => JsonSerializer.Deserialize(blockData, t, _serializerOptions);
    }

}
