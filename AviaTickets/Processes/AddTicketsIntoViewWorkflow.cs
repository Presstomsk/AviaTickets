using AviaTickets.Processes.Abstractions;
using Scheduler;
using System;
using System.Collections.Generic;

namespace AviaTickets.Processes
{
    public class AddTicketsIntoViewWorkflow : IAddTicketsIntoViewWorkflow
    {        
        private ISchedulerFactory<IOut> _scheduler;
        private MainWindow _mainWindow;

        private List<TicketForm>? _tickets;

        public string WorkflowType { get; set; } = "ADD_TICKETS_INTO_VIEW_WORKFLOW";

        public AddTicketsIntoViewWorkflow(ISchedulerFactory<IOut> schedulerFactory
                                 , MainWindow mainWindow)
        {            
            _mainWindow = mainWindow;
            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(AddTicketsToMainWindow)
                                         .Build();
        }

        public IMessage? Start(IMessage? msg)
        {
            if (msg != default)
            {
                if (msg.IsSuccess)
                {
                    if (typeof(List<TicketForm>) == msg.DataType)
                    {
                        _tickets = (List<TicketForm>?)msg.Data;
                        return Start();
                    }
                    else throw new Exception("Input Data has incorrect type");

                }
                else
                {
                    throw msg.Error ?? new Exception();
                }
            }
            else throw new Exception("Input Data is null");
        }

        public IMessage? Start()
        {
            var answer = _scheduler.StartProcess();
            return new Msg.Message(answer.Item1, null, null, answer.Item2);
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
