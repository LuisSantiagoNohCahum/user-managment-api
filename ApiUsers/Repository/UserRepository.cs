using ApiUsers.DataBaseContext;
using ApiUsers.Models;
using ApiUsers.Models.Dto.Request;
using ApiUsers.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiUsers.Repository
{
    public class UserRepository : IRepository<User>
    {
        private GeneralRepositoryContext _context;

        public UserRepository(GeneralRepositoryContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var data = await _context.Users.ToListAsync();

            return data.FindAll(x => x.IsActive == 1); //asEnumerable
        }

        public async Task<IEnumerable<User>> GetAllByAsync<FilterUserDto>(FilterUserDto _filter)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<User> GetByUsername(string _username)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public class FilterResult
        { 
            public bool HaveError { get; set; }
            public bool ExistRequiredFilter { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
            public List<User>? DataFilterd { get; set; }
        }
    }
}
