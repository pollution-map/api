using PollutionMapAPI.Data.Entities;
using System.Linq.Expressions;

namespace PollutionMapAPI.Data.Repositories.Interfaces;

public interface IGenericRepository<T, IdType> where T : BaseEntity<IdType>
{
    Task<T?> GetByIdAsync(IdType id);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);

    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);

    Task<int> CountAllAsync();
    Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);
}
