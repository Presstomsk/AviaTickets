using AviaTickets.Models.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace AviaTickets.Models
{
    public class Result : ITicket
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("data")]
        public List<Data> Data { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }

    }

    public class Data : IEquatable<Data>
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }
        [JsonProperty("destination")]
        public string Destination { get; set; }
        [JsonProperty("origin_airport")]
        public string OriginAirport { get; set; }
        [JsonProperty("destination_airport")]
        public string DestinationAirport { get; set; }
        [JsonProperty("price")]
        public uint Price { get; set; }
        [JsonProperty("airline")]
        public string Airline { get; set; }
        [JsonProperty("flight_number")]
        public string FlightNumber { get; set; }
        [JsonProperty("departure_at")]
        public string DepartureAt { get; set; }
        [JsonProperty("return_at")]
        public string ReturnAt { get; set; }
        [JsonProperty("search_id")]
        public string SearchId { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }
        [JsonProperty("transfers")]
        public uint Transfers { get; set; }
        [JsonProperty("return_transfers")]
        public uint ReturnTransfers { get; set; }
        [JsonProperty("duration")]
        public uint Duration { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }

        public bool Equals(Data? other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            return Airline.Equals(other.Airline) && Link.Equals(other.Link);
        }
        public override int GetHashCode()
        {
            
            int hashAirline = Airline == null ? 0 : Airline.GetHashCode();            
            int hashLink = Link == null ? 0 : Link.GetHashCode();            
            return hashAirline ^ hashLink;
        }
    }
}
