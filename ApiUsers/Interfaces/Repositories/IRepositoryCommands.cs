namespace ApiUsers.Interfaces.Repositories
{
    public interface IRepositoryCommands<TEntity> where TEntity : class
    {
        Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken);
        Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
