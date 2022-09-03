using AviaTickets.Models.Abstractions;
using Newtonsoft.Json;
using System;

namespace AviaTickets.Models
{
    public class Cities : ICities
    {       
        public int Id { get; set; }
        [JsonProperty("name")]
        public string City { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
}
