using Sample.Data.Entities;
using System.Linq.Expressions;

namespace Sample.Data.Persistence.Interfaces
{
    public interface ICrudRepository<T, TKey> where T : BaseEntity<TKey>
    {
        Task AddAsync(T entity, bool saveChanges = true);

        Task UpdateAsync(T entity, bool saveChanges = true);

        Task DeleteAsync(T entity, bool saveChanges = true);

        Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<T> GetByIdAsync(TKey key, params Expression<Func<T, object>>[] includes);
    }
}