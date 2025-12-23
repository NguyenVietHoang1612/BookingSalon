using System.Linq.Expressions;

namespace BookingSalon.Data.Repository
{
    public interface IRepository<TEntity>  where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync<TKey>(TKey id);
        Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        // Write
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);

        // Check
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
