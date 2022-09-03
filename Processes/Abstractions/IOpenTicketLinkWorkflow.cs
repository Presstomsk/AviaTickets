

using AviaTickets.Statuses;

namespace AviaTickets.Processes.Abstractions
{
    public interface IOpenTicketLinkWorkflow : IWorkflow
    {
        public Result Start(string link);
    }
}
