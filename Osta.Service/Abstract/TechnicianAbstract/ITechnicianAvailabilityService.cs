using Osta.Data.Entities.Technician;

namespace Osta.Service.Abstract.TechnicianAbstract
{
    public interface ITechnicianAvailabilityService
    {
        Task AddTechnicianAvailabilityAsync(TechnicianAvailability technicianavailability, CancellationToken ct = default);
        Task UpdateTechnicianAvailabilityAsync(int id, TechnicianAvailability technicianavailability, CancellationToken ct = default);
        Task DeleteTechnicianAvailabilityAsync(int id, string TechnicianId, CancellationToken ct = default);
        Task<TechnicianAvailability?> GetTechnicianAvailabilityAsync(int id, CancellationToken ct = default);
        Task<TechnicianAvailability?> GetTechnicianAvailabilityForTechnicianAsync(int id, string TechnicianId, CancellationToken ct = default);
        Task<IEnumerable<TechnicianAvailability>> GetAllTechnicianAvailabilitiesAsync(CancellationToken ct = default);
        IQueryable<TechnicianAvailability> GetAllTechnicianAvailabilityQueryable(CancellationToken ct = default);
        Task<bool> HasOverlappingAvailabilityAsync(TechnicianAvailability technicianAvailability, CancellationToken ct = default);
        Task<bool> HasOverlappingAvailabilityForUpdateAsync(TechnicianAvailability technicianAvailability, CancellationToken ct = default);
        Task<IEnumerable<TechnicianAvailability>> GetAllTechnicianAvailabilitiesByTechnicianIdAsync(string technicianId, CancellationToken ct = default);

    }
}
