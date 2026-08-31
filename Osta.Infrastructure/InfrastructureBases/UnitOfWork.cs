using Microsoft.EntityFrameworkCore.Storage;
using Osta.Infrastructure.DataBase;

namespace Osta.Infrastructure.InfrastructureBases
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OstaContext ostaContext;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(OstaContext ostaContext)
        {
            this.ostaContext = ostaContext;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await ostaContext.Database.BeginTransactionAsync();

            return _transaction;
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ostaContext.SaveChangesAsync();
        }
    }
}
