using AviaTickets.Models;
using System.Collections.Generic;


namespace AviaTickets.Models.Abstractions
{
    public interface ITicket
    {
        public bool Success { get; set; }
        List<Data> Data { get; set; }
        string Currency { get; set; }
    }
}
