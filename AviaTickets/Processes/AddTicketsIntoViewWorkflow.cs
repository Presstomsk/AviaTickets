using AviaTickets.Processes.Abstractions;
using Scheduler;
using System;
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
            _scheduler = schedulerFactory.Create()
                                         .Do(AddTicketsToMainWindow);
                                         
        }

        public IMessage? Start(IMessage? msg)
        {
            if (msg != default)
            {
                _tickets = msg.GetData<List<TicketForm>>();
                return Start();                
            }
            else throw new Exception("Input Data is null");
        }

        public IMessage? Start()
        {
           return _scheduler.Start();            
        }
             

        private IMessage? AddTicketsToMainWindow(IMessage? message = default)
        {
            _mainWindow.Tickets.Children.Clear();

            if (_tickets != default)
            {
                for (int i = _tickets.Count - 1; i >= 0; i--)
                {
                    _mainWindow.Tickets.Children.Insert(0, _tickets[i]);
                }
            }

            return message;
        }        
    }
}
