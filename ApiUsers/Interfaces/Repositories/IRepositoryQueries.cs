namespace ApiUsers.Interfaces.Repositories
{
    public interface IRepositoryQueries<TEntity, TPrimaryKey> where TEntity : class
    {
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken);
        Task<TEntity?> GetAsync(TPrimaryKey? primaryKeyValue, CancellationToken cancellationToken);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken);
        Task<IEnumerable<TEntity>> GetAllAsync(string filter, object?[]? parameters, CancellationToken cancellationToken);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    }
}
