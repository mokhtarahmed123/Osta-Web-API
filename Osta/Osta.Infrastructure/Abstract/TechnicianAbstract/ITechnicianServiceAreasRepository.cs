using Osta.Data.Entities.Technician;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.TechnicianAbstract
{
    public interface ITechnicianServiceAreasRepository : IGenericRepositoryAsync<TechnicianServiceArea>
    {
        Task<bool> TechnicianHasThisServiceArea(string TechnicianId, int ServiceAreaId);
    }
}
