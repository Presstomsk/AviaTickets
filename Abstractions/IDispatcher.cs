using AviaTickets.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Abstractions
{
    public interface IDispatcher
    {
        void Start(IServiceProvider serviceProvider, ProcessType process);
    }
}
