

namespace ApiUsers.Repository.Interfaces
{
    public interface IRepository <T> where T : class
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int id);

        public Task<bool> SaveAsync(T entity);
        public Task<int> InsertAsync(T entity);

        public Task<int> UpdateAsync(T entity);

        //returnar id elemento eliminado
        public Task<bool> DeleteAsync(int id);


        //public Task<IEnumerable<T>> GetAllAsync (CancellationToken cancellationToken);
    }
}
