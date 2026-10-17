using Osta.Data.Entities.Services;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.ServicesAbstract
{
    public interface IServiceRepository : IGenericRepositoryAsync<Service>
    {
        Task<bool> DoesCategoryHaveServiceAsync(int CategoryId, CancellationToken cancellationToken);
    }
}
