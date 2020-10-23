using Microsoft.EntityFrameworkCore;
namespace ScamScraper.Models
{ 
    public class MyContext : DbContext 
    { 
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Message> Messages {get;set;}
        public DbSet<Comment> Comments {get;set;}
        public DbSet<Association> Associations {get;set;}
    }
}