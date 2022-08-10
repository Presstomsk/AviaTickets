using AviaTickets.Abstractions;
using AviaTickets.Processes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Сontrol
{
    static class Dispatcher 
    {
        public static void Start(IServiceProvider serviceProvider, ProcessType process)
        {
            switch (process)
            {
                case ProcessType.CITIES_LIST_CREATING:
                    serviceProvider.GetService<ICitiesListCreatingWorkflow>()?.Start();
                    break;
                case ProcessType.AVIA_TICKETS_GET:
                    serviceProvider.GetService<IAviaTicketsGetWorkflow>()?.Start();
                    break;
                default:
                    break;
            }
            
        }
    }
}
