using System.Linq.Expressions;

namespace BookingSalon.Data.Repository
{
    public interface IRepository<TEntity>  where TEntity : class
    {
        IQueryable<TEntity> Query();

        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync<TKey>(TKey id);

        // Write
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);

        // Check
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

    }
}
