using Osta.Data.Entities.Technician;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.TechnicianAbstract
{
    public interface ITechnicianServiceRepository : IGenericRepositoryAsync<TechnicianService>
    {
        Task<IEnumerable<TechnicianService>> GetAllByServiceId(int serviceId);
        Task<IEnumerable<TechnicianService>> GetAllByTechnicianId(string TechnicianId);

    }
}
