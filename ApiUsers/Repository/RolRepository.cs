namespace ApiUsers.Repository
{
    public class RolRepository : BaseRepository<Rol, int>, IRolRepository
    {
        public RolRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) 
            : base(dbContext, httpContextAccessor)
        { }
    }
}
