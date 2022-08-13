using AviaTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Abstractions
{
    public interface ITicket
    {
        public bool Success { get; set; }
        List<Data> Data { get; set; }
        string Currency { get; set; }
    }
}
