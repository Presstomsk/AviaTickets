using AviaTickets.Models.Abstractions;
using Newtonsoft.Json;


namespace AviaTickets.Models
{
    public class Cities : ICities
    {
        [JsonProperty("name")]
        public string City { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
