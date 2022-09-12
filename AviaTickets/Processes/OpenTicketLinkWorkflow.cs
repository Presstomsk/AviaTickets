using AviaTickets.Processes.Abstractions;
using Scheduler;
using System;
using System.Diagnostics;

namespace AviaTickets.Processes
{
    internal class OpenTicketLinkWorkflow : IOpenTicketLinkWorkflow
    {        
        private ISchedulerFactory<IOut> _scheduler;
        public string Link { get; set; }
        public string WorkflowType { get; set; } = "OPEN_TICKET_LINK";
        public OpenTicketLinkWorkflow(ISchedulerFactory<IOut> schedulerFactory)
        {
            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(OpenLink)
                                         .Build();
        }

        public IMessage? Start(IMessage? msg)
        {
            if (msg != default)
            {
                if (msg.IsSuccess)
                {
                    return Start();
                }
                else
                {
                    throw msg.Error ?? new Exception();
                }
            }
            return Start();
        }

        public IMessage? Start()
        {
            var answer = _scheduler.StartProcess();
            return new Msg.Message(answer.Item1, null, null, answer.Item2);
        }
       
        private void OpenLink()
        {
           Process.Start(new ProcessStartInfo
           {
               FileName = $"https://www.aviasales.ru{Link}",
               UseShellExecute = true
           });
        }
       
    }
}
