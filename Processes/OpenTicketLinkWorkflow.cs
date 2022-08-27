using AviaTickets.Processes.Abstractions;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.Statuses;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace AviaTickets.Processes
{
    internal class OpenTicketLinkWorkflow : IOpenTicketLinkWorkflow
    {
        private ILogger<OpenTicketLinkWorkflow> _logger;
        private ISchedulerFactory _scheduler;
        private string _link;
        public string WorkflowType { get; set; } = "OPEN_TICKET_LINK";
        public OpenTicketLinkWorkflow(ILogger<OpenTicketLinkWorkflow> logger            
            , ISchedulerFactory schedulerFactory)
        { 
            _logger = logger;

            _scheduler = schedulerFactory.Create()
                            .Do(OpenLink);
                           
        }        

        public (bool, object?) Start() { return (false, null); }
        public (bool, object?) Start(string link)
        {
            _link = link;
            try
            {
                _scheduler.Start();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger?.LogError($"PROCESS: {WorkflowType} STATUS: {STATUS.ERROR}", ex.Message);
                return (false, null);
            }
        }
        private void OpenLink()
        {
           Process.Start(new ProcessStartInfo
           {
               FileName = $"https://www.aviasales.ru{_link}",
               UseShellExecute = true
           });
        }

        
    }
}
