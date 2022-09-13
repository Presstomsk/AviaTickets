using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.Msg;
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
       
        public MainController(ServiceProvider serviceProvider)
        {
            _mainWindow = serviceProvider.GetService<MainWindow>();
            _view = serviceProvider.GetService<IView>();
            var scheduller = serviceProvider.GetService<ISchedulerFactory>();

            if (_mainWindow != default) _mainWindow.DataContext = _view;
            _mainWindow?.Show();

            _serviceProvider = serviceProvider;
            var citiesDatabaseUpdateWorkflow = _serviceProvider.GetService<ICitiesDatabaseUpdateWorkflow>();
            var citiesListCreatingWorkflow = _serviceProvider.GetService<ICitiesListCreatingWorkflow>();

             scheduller?.Create()
                        .Do(citiesDatabaseUpdateWorkflow.Start)
                        .Do(citiesListCreatingWorkflow.Start)
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
                var scheduller = _serviceProvider.GetService<ISchedulerFactory>();
                var inputValidationWorkflow = _serviceProvider.GetService<IInputDataValidationWorkflow>();
                var aviaTicketsGetWorkflow = _serviceProvider.GetService<IAviaTicketsGetWorkflow>();
                var ticketsCreatedWorkflow = _serviceProvider.GetService<ITicketsCreatedWorkflow>();
                var addTicketsIntoViewWorkflow = _serviceProvider.GetService<IAddTicketsIntoViewWorkflow>();

                 scheduller?.Create()
                            .Do(inputValidationWorkflow.Start)
                            .Do(aviaTicketsGetWorkflow.Start)
                            .Do(ticketsCreatedWorkflow.Start)
                            .Do(addTicketsIntoViewWorkflow.Start)
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
                var scheduller = _serviceProvider.GetService<ISchedulerFactory>();
                var openTicketLinkWorkflow = _serviceProvider.GetService<IOpenTicketLinkWorkflow>();

                var msg = new Message(link,link.GetType());

                 scheduller?.Create()
                            .Do(openTicketLinkWorkflow.Start)
                            .Start(msg);
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
