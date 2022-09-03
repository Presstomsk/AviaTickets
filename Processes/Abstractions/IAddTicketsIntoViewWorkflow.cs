
using AviaTickets.Statuses;

namespace AviaTickets.Processes.Abstractions
{
    public interface IAddTicketsIntoViewWorkflow : IWorkflow
    {
        Result Start(object? data);
    }
}
