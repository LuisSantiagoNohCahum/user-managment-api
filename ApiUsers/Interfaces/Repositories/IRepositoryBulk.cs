namespace ApiUsers.Interfaces.Repositories
{
    public interface IRepositoryBulk<TEntity> where TEntity : class
    {
        Task<int> BulkInsert(IEnumerable<TEntity> bulkData, CancellationToken cancellationToken);
    }
}
