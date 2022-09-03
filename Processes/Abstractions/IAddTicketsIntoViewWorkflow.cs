
namespace AviaTickets.Processes.Abstractions
{
    public interface IAddTicketsIntoViewWorkflow : IWorkflow
    {
        (bool, object?) Start(object? data);
    }
}
