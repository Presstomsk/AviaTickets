using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Models
{
    public class Cities
    {
        [JsonProperty("name")]
        public string City { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
