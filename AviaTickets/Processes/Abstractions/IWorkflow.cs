using Scheduler;

namespace AviaTickets.Processes.Abstractions
{
    public interface IWorkflow 
    {
        string WorkflowType { get; set; }        
        IMessage? Start();
        IMessage? Start(IMessage message);

    }
}
