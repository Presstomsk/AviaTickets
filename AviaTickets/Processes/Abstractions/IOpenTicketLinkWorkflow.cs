
namespace AviaTickets.Processes.Abstractions
{
    public interface IOpenTicketLinkWorkflow : IWorkflow
    {
        string Link { get; set; }
    }
}
