
namespace AviaTickets.Processes.Abstractions
{
    public interface IAddTicketsIntoView : IWorkflow
    {
        (bool, object?) Start(object? data);
    }
}
