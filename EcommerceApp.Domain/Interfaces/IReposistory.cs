using System.Linq.Expressions;

namespace EcommerceApp.Domain.Interfaces
{
    public interface IReposistory<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        IQueryable<T> GetBaseQuery(Expression<Func<T, bool>> predicate);
    }
}
