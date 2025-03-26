namespace ApiUsers.Repository
{
    public abstract class BaseRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity
    {
        protected readonly AppDbContext _dbContext;
        private readonly HttpContextAccessor _httpContextAccessor;

        public BaseRepository(AppDbContext dbContext, HttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken)
            => await _dbContext.Set<TEntity>().Where(filter).FirstOrDefaultAsync(cancellationToken);

        public async Task<TEntity?> GetAsync(TPrimaryKey? primaryKeyValue, CancellationToken cancellationToken)
        {
            if (primaryKeyValue is null)
                throw new Exception("Provides a Primary Key value to find any record.");
            
            return await _dbContext.Set<TEntity>().FindAsync(primaryKeyValue, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
            => await GetAllAsync(null, cancellationToken);

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter, CancellationToken cancellationToken)
        {
            return filter is null ? 
                await _dbContext.Set<TEntity>().ToListAsync(cancellationToken) : 
                await _dbContext.Set<TEntity>().Where(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(string filter, object?[]? parameters, CancellationToken cancellationToken)
        {
            var expression = DynamicExpressionParser.ParseLambda<TEntity, bool>(null, false, filter, parameters ?? []);
            return await GetAllAsync(expression, cancellationToken);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken) 
        {
            return await _dbContext.Set<TEntity>().AnyAsync(filter, cancellationToken);
        }

        public async Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            entity.CreatedOn = DateTime.Now;
            entity.CreatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            _dbContext.Set<TEntity>().Add(entity);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            entity.UpdatedOn = DateTime.Now;
            entity.UpdatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknow";
            _dbContext.Entry(entity).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity is null) return 0;

            _dbContext.Set<TEntity>().Remove(entity);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> BulkInsert(IEnumerable<TEntity> bulkData, CancellationToken cancellationToken)
        {
            var data = bulkData.Select(item => {
                item.CreatedOn = DateTime.Now;
                item.CreatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
                return item;
            });

            await _dbContext.Set<TEntity>().AddRangeAsync(data, cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
