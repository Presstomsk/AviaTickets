﻿using AviaTickets.Processes.Abstractions;
using AviaTickets.ViewModel.Absractions;
using Microsoft.Extensions.DependencyInjection;


namespace AviaTickets.Controller
{
    public delegate void TicketClickHandler(string link);
    public class MainController 
    {
        private IView? _view;
        private MainWindow? _mainWindow;
        private ServiceProvider _serviceProvider;
        private (bool, object?)? _result;
        public MainController (ServiceProvider serviceProvider)
        {
            _mainWindow = serviceProvider.GetService<MainWindow>();
            _view = serviceProvider.GetService<IView>();            
            if (_mainWindow != default) _mainWindow.DataContext = _view;
            _mainWindow?.Show();

            _serviceProvider = serviceProvider;
            _serviceProvider.GetService<ICitiesDatabaseUpdateWorkflow>()?.Start();
            _serviceProvider.GetService<ICitiesListCreatingWorkflow>()?.Start();

            if (_view != default)
            {
                _view.SearchTickets += View_SearchTickets;
                _view.OpenTicketLink += Tickets_OpenTicketLink;
            }   
            
        }

        private void View_SearchTickets()
        {            
            _result = _serviceProvider.GetService<IInputDataValidationWorkflow>()?.Start();
            if (_result.Value.Item1) _result = _serviceProvider.GetService<IAviaTicketsGetWorkflow>()?.Start();
            if (_result.Value.Item1) _result = _serviceProvider.GetService<ITicketsCreatedWorkflow>()?.Start(_result.Value.Item2);
            if (_result.Value.Item1) _serviceProvider.GetService<IAddTicketsIntoViewWorkflow>()?.Start(_result.Value.Item2);
            
        }

        private void Tickets_OpenTicketLink(string link)
        {
            if(_result.Value.Item1) _result = _serviceProvider.GetService<IOpenTicketLinkWorkflow>()?.Start(link);
        }
    }
}
