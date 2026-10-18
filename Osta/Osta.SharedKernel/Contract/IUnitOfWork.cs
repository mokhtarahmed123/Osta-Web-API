using Microsoft.EntityFrameworkCore.Storage;

namespace Osta.Infrastructure.InfrastructureBases
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
