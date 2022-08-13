using AviaTickets.Converters.ParentClasses;
using AviaTickets.Models;
using AviaTickets.Models.Abstractions;
using Newtonsoft.Json;
using System;

namespace AviaTickets.Converters
{
    public class Tickets : TicketConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ITicket));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(Result));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value, typeof(Result));
        }
    }
}
