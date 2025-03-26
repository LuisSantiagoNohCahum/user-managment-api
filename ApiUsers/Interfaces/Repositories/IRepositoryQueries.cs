namespace ApiUsers.Interfaces.Repositories
{
    public interface IRepositoryQueries<TEntity, TPrimaryKey> where TEntity : class
    {
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct);
        Task<TEntity?> GetAsync(TPrimaryKey? primaryKeyValue, CancellationToken ct);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct);
        Task<IEnumerable<TEntity>> GetAllAsync(string filter, object?[]? parameters, CancellationToken ct);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct);
    }
}
