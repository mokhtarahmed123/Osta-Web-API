using Microsoft.EntityFrameworkCore;
using Osta.Infrastructure.DataBase;
using System.Linq.Expressions;

namespace Osta.Infrastructure.InfrastructureBases
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        #region Vars / Props

        protected readonly OstaContext _dbContext;

        #endregion

        #region Constructor(s)
        public GenericRepositoryAsync(OstaContext dbContext)
        {
            _dbContext = dbContext;

        }
        #endregion

        #region Methods

        #endregion

        #region Actions
        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {

            return await _dbContext.Set<T>().FindAsync(id, cancellationToken);
        }


        public IQueryable<T> GetTableNoTracking(CancellationToken cancellationToken)
        {
            return _dbContext.Set<T>().AsNoTracking().AsQueryable();
        }


        public virtual async Task AddRangeAsync(ICollection<T> entities, CancellationToken cancellationToken)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);

        }
        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
            return entity;


        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            _dbContext.Set<T>().Update(entity);

        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            _dbContext.Set<T>().Remove(entity);
        }
        public virtual async Task DeleteRangeAsync(ICollection<T> entities, CancellationToken cancellationToken)
        {
            _dbContext.Set<T>().RemoveRange(entities);

        }



        public IQueryable<T> GetTableAsTracking(CancellationToken cancellationToken)
        {
            return _dbContext.Set<T>().AsQueryable();

        }

        public virtual async Task UpdateRangeAsync(ICollection<T> entities, CancellationToken cancellationToken)
        {
            _dbContext.Set<T>().UpdateRange(entities);

        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return _dbContext.Set<T>().AnyAsync(predicate);
        }

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return _dbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);

        }

        public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<T>().FindAsync(id, cancellationToken);
        }

        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
    CancellationToken cancellationToken, bool tracking = false)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (!tracking)
                query = query.AsNoTracking();

            return await query.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<T>()
                .Where(predicate)
                .FirstOrDefaultAsync();
        }


        #endregion

    }
}
