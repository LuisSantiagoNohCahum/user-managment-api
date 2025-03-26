namespace ApiUsers.Interfaces.Repositories
{
    public interface IRepositoryCommands<TEntity> where TEntity : class
    {
        Task<int> InsertAsync(TEntity entity, CancellationToken ct);
        Task<int> UpdateAsync(TEntity entity, CancellationToken ct);
        Task<int> DeleteAsync(TEntity entity, CancellationToken ct);
    }
}
