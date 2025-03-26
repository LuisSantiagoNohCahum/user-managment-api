namespace ApiUsers.Repository
{
    public sealed class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) 
            : base(dbContext, httpContextAccessor) 
        { }
    }
}
