using System.Linq.Expressions;

namespace Osta.Infrastructure.InfrastructureBases
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        Task DeleteRangeAsync(ICollection<T> entities, CancellationToken cancellationToken);
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        IQueryable<T> GetTableNoTracking(CancellationToken cancellationToken);
        IQueryable<T> GetTableAsTracking(CancellationToken cancellationToken);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken);
        Task AddRangeAsync(ICollection<T> entities, CancellationToken cancellationToken);
        Task UpdateAsync(T entity, CancellationToken cancellationToken);
        Task UpdateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken);
        Task DeleteAsync(T entity, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken, bool tracking = false);
    }
}
