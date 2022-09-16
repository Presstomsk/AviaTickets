using AviaTickets.Models;
using Microsoft.EntityFrameworkCore;


namespace AviaTickets.DB
{
    public class MainContext : DbContext
    {
        public DbSet<Cities> Cities { get; set; }
        public MainContext(DbContextOptions options) : base(options) {}
        
    }
}
