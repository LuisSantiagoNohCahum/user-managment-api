namespace ApiUsers.Repository.Seeds
{
    public class UserSeeds : IEntityTypeConfiguration<User>
    {
        private readonly IPasswordHasherHelper _passwordHasherHelper;
        public UserSeeds(IPasswordHasherHelper passwordHasherHelper)
        {
            _passwordHasherHelper = passwordHasherHelper;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(
                new User() 
                { 
                    Email = "default@default.com", 
                    Password = _passwordHasherHelper.HashPassword("admin"), 
                    FirstName = "Admin", 
                    LastName = "Admin", 
                    IsActive = true, 
                    RolId = 1, 
                    CreatedBy = "System", 
                    CreatedOn = DateTime.Now 
                });
        }
    }
}
