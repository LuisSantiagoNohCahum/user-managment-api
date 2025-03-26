namespace ApiUsers.Repository
{
    public class AppDbContext : DbContext
    {
        private readonly IPasswordHasherHelper _passwordHasherHelper;
        public DbSet<User> Users { get; set; }
        public DbSet<Rol> Roles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IPasswordHasherHelper passwordHashHelper) : base(options) 
        {
            _passwordHasherHelper = passwordHashHelper;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserSeeds(_passwordHasherHelper));
            modelBuilder.ApplyConfiguration(new RolSeeds());
        }
    }
}
