

namespace AviaTickets.Processes.Abstractions
{
    public interface IWorkflow
    {
        string WorkflowType { get; set; }
        public (bool, object?) Start();
        
    }
}
