using AviaTickets.Models.Abstractions;
using Newtonsoft.Json;
using System;

namespace AviaTickets.Converters
{
    public class CitiesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ICities));
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(Models.Cities));
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value, typeof(Models.Cities));
        }
    }
}
