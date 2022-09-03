using AviaTickets.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AviaTickets.DB
{
    public class MainContext : DbContext
    {
        public string ConnString { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public MainContext(){}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnString);           
        }
    }
}
