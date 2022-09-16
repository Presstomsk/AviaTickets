using Microsoft.EntityFrameworkCore;

namespace AviaTickets.DB
{
    public class ContextFactory : IDbContextFactory<MainContext>
    {
        private DbContextOptions _options;

        public ContextFactory(DbContextOptions options)
        {
            _options = options;
        }

        public MainContext CreateDbContext()
        {
           return new MainContext(_options);
        }
    }
}
