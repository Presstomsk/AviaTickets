using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Abstractions
{
    public interface ISchedulerFactory
    {
        ISchedulerFactory Create();
        ISchedulerFactory Do(Action action);
        void Start();        
    }
}
