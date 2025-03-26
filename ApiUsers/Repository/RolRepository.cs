using ApiUsers.Interfaces.Repositories;
using ApiUsers.Models;

namespace ApiUsers.Repository
{
    public class RolRepository : BaseRepository<Rol, int>, IRolRepository
    {
        public RolRepository(AppDbContext dbContext, HttpContextAccessor httpContextAccessor) 
            : base(dbContext, httpContextAccessor)
        { }
    }
}
