using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Abstractions
{
    public interface ICities
    {
        string City { get; set; }
        string Code { get; set; }
    }
}
