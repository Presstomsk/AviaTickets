using Microsoft.EntityFrameworkCore;

namespace AviaTickets.DB
{
    public class ContextFactory : IDbContextFactory<MainContext>
    {       
        public ContextFactory()
        {            
        }
        public MainContext CreateDbContext()
        {
           return new MainContext();
        }
    }
}
