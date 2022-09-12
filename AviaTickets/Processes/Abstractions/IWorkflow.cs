using Scheduler;

namespace AviaTickets.Processes.Abstractions
{
    public interface IWorkflow : IOut
    {
        string WorkflowType { get; set; }        
        IMessage? Start();

    }
}
