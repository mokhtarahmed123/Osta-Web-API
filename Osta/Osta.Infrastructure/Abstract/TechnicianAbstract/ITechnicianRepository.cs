using Osta.Data.Entities.Technician;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.TechnicianAbstract
{
    public interface ITechnicianRepository : IGenericRepositoryAsync<Technicians>
    {
        public Task<IEnumerable<Technicians>> GetAllWithServiceId(int serviceId);
        Task<IEnumerable<Technicians>> GetAllTechniciansWithServiceArea(int ServiceAreaId);

        Task<Technicians> GetByIdWithServiceAndServiceArea(string Id);
    }
}
