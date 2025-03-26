using ApiUsers.Interfaces.Repositories;
using ApiUsers.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiUsers.Repository
{
    public sealed class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext, HttpContextAccessor httpContextAccessor) 
            : base(dbContext, httpContextAccessor) 
        { }
    }
}
