namespace ApiUsers.Repository
{
    public sealed class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext, HttpContextAccessor httpContextAccessor) 
            : base(dbContext, httpContextAccessor) 
        { }
    }
}
