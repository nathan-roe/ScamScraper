using Microsoft.EntityFrameworkCore;
namespace ScamScraper.Models
{ 
    public class MyContext : DbContext 
    { 
        public MyContext(DbContextOptions options) : base(options) { }
        // public DbSet<Scam> Scams {get;set;}
    }
}