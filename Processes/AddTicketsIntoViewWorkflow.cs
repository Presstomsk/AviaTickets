using AviaTickets.Processes.Abstractions;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.Statuses;
using System.Collections.Generic;

namespace AviaTickets.Processes
{
    public class AddTicketsIntoViewWorkflow : IAddTicketsIntoViewWorkflow
    {        
        private ISchedulerFactory _scheduler;
        private MainWindow _mainWindow;

        private List<TicketForm>? _tickets;

        public string WorkflowType { get; set; } = "ADD_TICKETS_INTO_VIEW_WORKFLOW";

        public AddTicketsIntoViewWorkflow(ISchedulerFactory schedulerFactory
                                 , MainWindow mainWindow)
        {            
            _mainWindow = mainWindow;
            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(AddTicketsToMainWindow);
        }

        public Result Start(object? data)
        {
            _tickets = (data != null) ? data as List<TicketForm> : new List<TicketForm>();
            var result = _scheduler.Start();
            return new Result { Success = result, Content = null };
        }
        public Result Start()
        {
            return new Result { Success = false, Content = null };
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
