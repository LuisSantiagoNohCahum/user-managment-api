using ApiUsers.Models.Constants;

namespace ApiUsers.Repository.Seeds
{
    public class RolSeeds : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.HasData(
                new Rol {
                    Id = 1,
                    Code = RolCode.Admin,
                    Name = "Administrator",
                    CreatedBy = "System",
                    CreatedOn = DateTime.Now,
                },
                new Rol {
                    Id = 2,
                    Code = RolCode.Guest,
                    Name = "Guest",
                    CreatedBy = "System",
                    CreatedOn = DateTime.Now,
                },
                new Rol
                {
                    Id = 3,
                    Code = RolCode.SysAdmin,
                    Name = "System Administrator",
                    CreatedBy = "System",
                    CreatedOn = DateTime.Now,
                },
                new Rol
                {
                    Id = 4,
                    Code = RolCode.Internal,
                    Name = "Internal",
                    CreatedBy = "System",
                    CreatedOn = DateTime.Now,
                });
        }
    }
}
