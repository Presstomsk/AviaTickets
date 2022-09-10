

namespace AviaTickets.DB.Abstractions
{
    public interface IContextFactory
    {
        MainContext CreateContext();
    }
}
