using AviaTickets.Abstractions;
using AviaTickets.Processes;
using AviaTickets.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Сontrol
{
    public class Dispatcher : IDispatcher
    {
        public void Start(IServiceProvider serviceProvider, ProcessType process)
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
