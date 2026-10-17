using Osta.Data.Entities.Technician;

namespace Osta.Service.Abstract.TechnicianAbstract
{
    public interface IServiceAreaService
    {
        Task AddServiceAreaAsync(ServiceArea serviceArea, CancellationToken ct = default);
        Task UpdateServiceAreaAsync(int id, ServiceArea serviceArea, CancellationToken ct = default);
        Task DeleteServiceAreaAsync(int id, CancellationToken ct = default);
        Task<ServiceArea?> GetServiceAreaAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<ServiceArea>> GetAllServiceAreasAsync(CancellationToken ct = default);
        IQueryable<ServiceArea> GetAllServiceAreasQueryable(CancellationToken ct = default);

        Task<ServiceArea?> GetServiceByCity(string city, CancellationToken ct = default);
        Task<IEnumerable<ServiceArea>> GetServiceAreaWithSpecificTechIdAsync(string techId, CancellationToken ct = default);



    }
}
