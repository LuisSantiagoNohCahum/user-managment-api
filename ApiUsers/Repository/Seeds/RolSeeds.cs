using ApiUsers.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiUsers.Repository.Seeds
{
    public class RolSeeds : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.HasData(
            new Rol {
                Code = "ADM",
                Name = "Administrator",
                CreatedBy = "System",
                CreatedOn = DateTime.Now,
            },
            new Rol {
                Code = "GST",
                Name = "Guest",
                CreatedBy = "System",
                CreatedOn = DateTime.Now,
            });
        }
    }
}
