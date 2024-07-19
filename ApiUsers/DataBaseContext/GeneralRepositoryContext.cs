using ApiUsers.Models;
using Microsoft.EntityFrameworkCore;
namespace ApiUsers.DataBaseContext
{
    public class GeneralRepositoryContext : DbContext
    {
        public GeneralRepositoryContext(DbContextOptions<GeneralRepositoryContext> options) : base(options) { }
    
        public DbSet<User> Users { get; set; }
        public DbSet<RolType> RolTypes { get; set; }
    }
}
