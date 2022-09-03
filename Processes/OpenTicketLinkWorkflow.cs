using AviaTickets.Processes.Abstractions;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.Statuses;
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
            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(OpenLink);                           
        }        

        public Result Start() {  return new Statuses.Result{ Success = false, Content = null }; }
        public Result Start(string link)
        {
            _link = link;            
            _scheduler.Start();
            return new Statuses.Result { Success = true, Content = null };

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
