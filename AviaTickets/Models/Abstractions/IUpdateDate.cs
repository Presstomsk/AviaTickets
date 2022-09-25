using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Models.Abstractions
{
    public interface IUpdateDate
    {
        DateTime LastUpdateDate { get; set; }
    }
}
