using AviaTickets.Processes.Abstractions;
using AviaTickets.Scheduler.Abstractions;
using System.Collections.Generic;

namespace AviaTickets.Processes
{
    public class AddTicketsIntoView : IAddTicketsIntoView
    {        
        private ISchedulerFactory _scheduler;
        private MainWindow _mainWindow;

        private List<TicketForm>? _tickets;

        public string WorkflowType { get; set; } = "ADD_TICKETS_INTO_VIEW";

        public AddTicketsIntoView(ISchedulerFactory schedulerFactory
                                 , MainWindow mainWindow)
        {            
            _mainWindow = mainWindow;
            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(AddTicketsToMainWindow);
        }

        public (bool, object?) Start(object? data)
        {
            _tickets = (data != null) ? data as List<TicketForm> : new List<TicketForm>();
            var result = _scheduler.Start();
            return (result, null);
        }
        public (bool, object?) Start()
        {
            return (false, null);
        }

        private void AddTicketsToMainWindow()
        {
            _mainWindow.Tickets.Children.Clear();

            if (_tickets != default)
            {
                for (int i = _tickets.Count - 1; i >= 0; i--)
                {
                    _mainWindow.Tickets.Children.Insert(0, _tickets[i]);
                }
            }
        }
    }
}
