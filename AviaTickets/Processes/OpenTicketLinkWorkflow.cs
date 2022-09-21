using AviaTickets.Processes.Abstractions;
using Scheduler;
using System;
using System.Diagnostics;

namespace AviaTickets.Processes
{
    internal class OpenTicketLinkWorkflow : IOpenTicketLinkWorkflow
    {        
        private ISchedulerFactory _scheduler;
        private string _link;
        
        public string WorkflowType { get; set; } = "OPEN_TICKET_LINK";
        public OpenTicketLinkWorkflow(ISchedulerFactory schedulerFactory)
        {
            _scheduler = schedulerFactory.Create()
                                         .Do(OpenLink);
                                         
        }

        public IMessage? Start(IMessage? msg)
        {
            if (msg != default)
            {
                _link = msg.GetData<string>();
                return Start();                
            }
            else throw new Exception("Input Data is null");
        }

        public IMessage? Start()
        {
            return _scheduler.Start();            
        }
       
        private IMessage? OpenLink(IMessage? message = default)
        {
           System.Diagnostics.Process.Start(new ProcessStartInfo
           {
               FileName = $"https://www.aviasales.ru{_link}",
               UseShellExecute = true
           });

            return message;
        }
       
    }
}
