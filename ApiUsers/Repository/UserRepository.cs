using ApiUsers.DataBaseContext;
using ApiUsers.Models;
using ApiUsers.Models.Dto.Request;
using ApiUsers.Repository.Interfaces;
using Microsoft.Data.SqlClient;
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

        public async Task<IEnumerable<User>> GetAllByAsync(FilterUserDto _filter)
        {
            FilterUserDto filterTmp = new FilterUserDto();

            var users = _context.Users.AsQueryable();

            if (_filter.UserName != filterTmp.UserName && !string.IsNullOrEmpty(_filter.UserName)) users = users.Where(x => x.UserName.Contains(_filter.UserName));

            if (_filter.Type != filterTmp.Type) users = users.Where(x => x.RolType == _filter.Type);
            
            if (_filter.CreatedOn != filterTmp.CreatedOn)  users = users.Where(x => x.CreatedOn.Date.Equals(_filter.CreatedOn.Date));

            var data = await users.ToListAsync();

            return data.FindAll(x => x.IsActive == 1);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }
        public async Task<User> GetByUserName(string _username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _username && x.IsActive == 1);
            return user;
        }

        public async Task<int> InsertAsync(User entity)
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return 1;
        }

        public async Task<int> UpdateAsync(User entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return 1;
        }

        public async Task<bool> SaveAsync(User entity)
        {
            int result = 0;

            if (entity.Id > 0)
            {
                result = await UpdateAsync(entity);
            }
            else 
            {
                result = await InsertAsync(entity);
            }

            return (result > 0) ? true : false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var _user = await GetByIdAsync(id);

            if (_user == null)
            {
                return false;
            }
            _context.Users.Remove(_user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> BulkInsertAsync(IEnumerable<User> data)
        {
            await _context.Users.AddRangeAsync(data);

            await _context.SaveChangesAsync();

            return true;
        }

        
    }
}
