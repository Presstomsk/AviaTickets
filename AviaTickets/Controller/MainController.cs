using AviaTickets.Processes.Abstractions;
using AviaTickets.ViewModel.Absractions;
using Microsoft.Extensions.DependencyInjection;
using Scheduler;
using System;
using System.Windows;

namespace AviaTickets.Controller
{
    public delegate void TicketClickHandler(string link);
    public class MainController
    {
        private IView? _view;
        private MainWindow? _mainWindow;
        private ServiceProvider _serviceProvider;
        private ISchedulerFactory<IOut>? _scheduller;

        public MainController(ServiceProvider serviceProvider)
        {
            _mainWindow = serviceProvider.GetService<MainWindow>();
            _view = serviceProvider.GetService<IView>();
            _scheduller = serviceProvider.GetService<ISchedulerFactory<IOut>>();

            if (_mainWindow != default) _mainWindow.DataContext = _view;
            _mainWindow?.Show();

            _serviceProvider = serviceProvider;
            var citiesDatabaseUpdateWorkflow = _serviceProvider.GetService<ICitiesDatabaseUpdateWorkflow>();
            var citiesListCreatingWorkflow = _serviceProvider.GetService<ICitiesListCreatingWorkflow>();

            _scheduller?.Create()
                        .Do(citiesDatabaseUpdateWorkflow)
                        .Do(citiesListCreatingWorkflow)
                        .Start();


            if (_view != default)
            {
                _view.SearchTickets += View_SearchTickets;
                _view.OpenTicketLink += Tickets_OpenTicketLink;
            }

        }

        private void View_SearchTickets()
        {
            try
            {
                var inputValidationWorkflow = _serviceProvider.GetService<IInputDataValidationWorkflow>();
                var aviaTicketsGetWorkflow = _serviceProvider.GetService<IAviaTicketsGetWorkflow>();
                var ticketsCreatedWorkflow = _serviceProvider.GetService<ITicketsCreatedWorkflow>();
                var addTicketsIntoViewWorkflow = _serviceProvider.GetService<IAddTicketsIntoViewWorkflow>();

                _scheduller?.Create()
                            .Do(inputValidationWorkflow)
                            .Do(aviaTicketsGetWorkflow)
                            .Do(ticketsCreatedWorkflow)
                            .Do(addTicketsIntoViewWorkflow)
                            .Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
                Application.Current?.Shutdown();
                return;
            }
        }



        private void Tickets_OpenTicketLink(string link)
        {
            try
            {
                var openTicketLinkWorkflow = _serviceProvider.GetService<IOpenTicketLinkWorkflow>();
                openTicketLinkWorkflow.Link = link;
                _scheduller?.Do(openTicketLinkWorkflow)
                            .Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
                Application.Current?.Shutdown();
                return;
            }
        }
    }
}
