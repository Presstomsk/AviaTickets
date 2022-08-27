

namespace AviaTickets.Processes.Abstractions
{
    public interface IOpenTicketLinkWorkflow : IWorkflow
    {
        public (bool, object?) Start(string link);
    }
}
