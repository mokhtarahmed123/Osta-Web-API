using Osta.Data.Entities.Technician;

namespace Osta.Core.Abstract.TechnicianAbstract
{
    public interface ITechnicianServiceService
    {

        Task AddAsync(TechnicianService technicianService, CancellationToken cancellationToken);
        Task DeleteAsync(TechnicianService technicianService, CancellationToken cancellationToken);
        Task UpdateAsync(TechnicianService technicianService, CancellationToken cancellationToken);


        Task<IEnumerable<TechnicianService>> GetAllAsync(CancellationToken cancellationToken);


        Task<TechnicianService?> GetByIdAsync(string technicianId, int serviceId, CancellationToken cancellationToken);


        Task<IEnumerable<TechnicianService>> GetByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken);

        Task<IEnumerable<TechnicianService>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken);


        Task<bool> ExistsAsync(string technicianId, int serviceId, CancellationToken cancellationToken);


        Task AddRangeAsync(IEnumerable<TechnicianService> technicianServices, CancellationToken cancellationToken);
        Task DeleteAllService_technicianBy_technicianIdAsync(string technicianId, CancellationToken cancellationToken);
        Task DeleteAllService_technicianBy_ServiceIdAsync(int ServiceId, CancellationToken cancellationToken);

        Task DeleteServiceByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken);


        Task DeleteByServiceIdAsync(int serviceId, CancellationToken cancellationToken);



    }
}