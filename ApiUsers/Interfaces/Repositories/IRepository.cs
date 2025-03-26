using System.Linq.Expressions;

namespace ApiUsers.Interfaces.Repositories
{
    public interface IRepository<TEntity, TPrimaryKey> : 
        IRepositoryQueries<TEntity, TPrimaryKey>, 
        IRepositoryCommands<TEntity>, 
        IRepositoryBulk<TEntity>
        where TEntity : class
    { }
}
