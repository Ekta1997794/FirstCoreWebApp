using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FirstCoreWebApp
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }  
    }
}
