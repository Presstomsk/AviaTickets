using AviaTickets.Models.Abstractions;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AviaTickets.Models
{
    
    public class Cities : ICities
    { 
        
        public int Id { get; set; }
        [JsonProperty("name")]
        [MaybeNull]
        public string City { get; set; }
        [MaybeNull]
        public string Code { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
}
