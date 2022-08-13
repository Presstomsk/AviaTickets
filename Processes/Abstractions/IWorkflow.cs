

namespace AviaTickets.Processes.Abstractions
{
    public interface IWorkflow
    {
        string WorkflowType { get; set; }
        public void Start();
    }
}
