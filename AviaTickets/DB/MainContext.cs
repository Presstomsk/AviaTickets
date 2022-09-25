using AviaTickets.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AviaTickets.DB
{
    public class MainContext : DbContext
    {
        public DbSet<Cities> Cities { get; set; }
        public DbSet<UpdateDate> UpdateDate { get; set; }
        public MainContext(){}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("MainDb"));
        }
    }
}
