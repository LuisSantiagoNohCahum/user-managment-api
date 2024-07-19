

namespace ApiUsers.Repository.Interfaces
{
    public interface IRepository <T> where T : class
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int id);
        public Task<IEnumerable<T>> GetAllByAsync<F>(F _filter);

        //Bool or id insert - update
        public Task<int> InsertAsync(T entity);

        public Task<int> UpdateAsync(T entity);

        //returnar id elemento eliminado
        public Task<bool> DeleteAsync(int id);


        //public Task<IEnumerable<T>> GetAllAsync (CancellationToken cancellationToken);
    }
}
