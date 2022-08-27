
namespace AviaTickets.Processes.Abstractions
{
    public interface ITicketsCreatedWorkflow : IWorkflow
    {
        (bool, object?) Start(object? data);
    }
}
