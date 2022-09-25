using AviaTickets.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Models
{
    public class UpdateDate : IUpdateDate
    {
        public int Id { get; set; }
        public DateTime LastUpdateDate { get; set; } = DateTime.Now;
    }
}
