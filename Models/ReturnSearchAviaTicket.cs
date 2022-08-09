using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Models
{
    public class Result
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("data")]
        public List<Data> Data { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }

    }

    public class Data
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


    }
}
