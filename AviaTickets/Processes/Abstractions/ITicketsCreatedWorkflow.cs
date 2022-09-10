
using AviaTickets.Statuses;

namespace AviaTickets.Processes.Abstractions
{
    public interface ITicketsCreatedWorkflow : IWorkflow
    {
        Result Start(object? data);
    }
}
