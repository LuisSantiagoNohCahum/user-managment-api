namespace ApiUsers.Repository
{
    public abstract class BaseRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity
    {
        protected readonly AppDbContext _dbContext;
        private readonly HttpContextAccessor _httpContextAccessor; //TODO. Or inject IServiceProvider and use GetRequiredService<>

        public BaseRepository(AppDbContext dbContext, HttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct)
            => await _dbContext.Set<TEntity>().Where(filter).FirstOrDefaultAsync();

        public async Task<TEntity?> GetAsync(TPrimaryKey? primaryKeyValue, CancellationToken ct)
        {
            if (primaryKeyValue is null)
                throw new Exception("Provides a Primary Key value to find any record.");
            
            return await _dbContext.Set<TEntity>().FindAsync(primaryKeyValue, ct);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct)
            => await GetAllAsync(null, ct);

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter, CancellationToken ct)
        {
            return filter is null ? 
                await _dbContext.Set<TEntity>().ToListAsync() : 
                await _dbContext.Set<TEntity>().Where(filter).ToListAsync(ct);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(string filter, object?[]? parameters, CancellationToken ct)
        {
            var expression = DynamicExpressionParser.ParseLambda<TEntity, bool>(null, false, filter, parameters ?? []);
            return await GetAllAsync(expression, ct);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct) 
        {
            return await _dbContext.Set<TEntity>().AnyAsync(filter, ct);
        }

        public async Task<int> InsertAsync(TEntity entity, CancellationToken ct)
        {
            entity.CreatedOn = DateTime.Now;
            entity.CreatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            _dbContext.Set<TEntity>().Add(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> UpdateAsync(TEntity entity, CancellationToken ct)
        {
            entity.UpdatedOn = DateTime.Now;
            entity.UpdatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknow";
            _dbContext.Entry(entity).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> DeleteAsync(TEntity entity, CancellationToken ct)
        {
            if (entity is null) return 0;

            _dbContext.Set<TEntity>().Remove(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> BulkInsert(IEnumerable<TEntity> bulkData, CancellationToken ct)
        {
            var data = bulkData.Select(item => {
                item.CreatedOn = DateTime.Now;
                item.CreatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
                return item;
            });

            await _dbContext.Set<TEntity>().AddRangeAsync(data, ct);
            return await _dbContext.SaveChangesAsync(ct);
        }
    }
}
