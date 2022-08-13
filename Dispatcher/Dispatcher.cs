using AviaTickets.Dispatcher.Abstractions;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.AllProcessesList;
using AviaTickets.ViewModel;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace AviaTickets.Dispatcher
{
    public class Dispatcher : IDispatcher
    {
        private AviaTicketsViewModel _view;
        public Dispatcher(AviaTicketsViewModel view)
        {
            _view = view;
        }

        public void Start(IServiceProvider serviceProvider, ProcessType process)
        { 
            switch (process)
            {
                case ProcessType.CITIES_LIST_CREATING:
                    serviceProvider.GetService<ICitiesListCreatingWorkflow>()?.Start();
                    break;
                case ProcessType.AVIA_TICKETS_GET:
                    serviceProvider.GetService<IInputDataValidationWorkflow>()?.Start();
                    if (_view.WithoutValidationErrors) serviceProvider.GetService<IAviaTicketsGetWorkflow>()?.Start();                    
                    break;
                default:
                    break;
            }
            
        }
    }
}
