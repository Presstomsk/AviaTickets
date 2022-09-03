using AviaTickets.DB.Abstractions;


namespace AviaTickets.DB
{
    public class ContextFactory : IContextFactory
    {
        public MainContext CreateContext()
        {
            return new MainContext();
        }
    }
}
