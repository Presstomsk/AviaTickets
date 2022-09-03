

using AviaTickets.Statuses;

namespace AviaTickets.Processes.Abstractions
{
    public interface IWorkflow
    {
        string WorkflowType { get; set; }
        public Result Start();
        
    }
}
