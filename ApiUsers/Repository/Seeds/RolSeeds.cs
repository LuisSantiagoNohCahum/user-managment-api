namespace ApiUsers.Repository.Seeds
{
    public class RolSeeds : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.HasData(
                new Rol {
                    Id = 1,
                    Code = "ADM",
                    Name = "Administrator",
                    CreatedBy = "System",
                    CreatedOn = DateTime.Now,
                },
                new Rol {
                    Id = 2,
                    Code = "GST",
                    Name = "Guest",
                    CreatedBy = "System",
                    CreatedOn = DateTime.Now,
                });
        }
    }
}
