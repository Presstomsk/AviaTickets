using AviaTickets.Abstractions;
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
        public static void Start(IServiceProvider serviceProvider)
        {
            serviceProvider.GetService<IAviaTicketsGetWorkflow>()?.Start();
        }
    }
}
