using System.Linq.Expressions;

namespace PollutionMapAPI.Repositories.Core;

public interface IAsyncRepository<T, IdType> where T : BaseEntity<IdType>
{
    Task<T> GetByIdAsync(IdType id);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);

    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);

    Task<int> CountAllAsync();
    Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);

}
