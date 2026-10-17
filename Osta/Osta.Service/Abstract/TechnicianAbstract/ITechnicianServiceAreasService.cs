using Osta.Data.Entities.Technician;

namespace Osta.Service.Abstract.TechnicianAbstract
{
    public interface ITechnicianServiceAreasService
    {
        public Task AddTechnicianServiceAreasRangeAsync(ICollection<TechnicianServiceArea> technicianServiceAreas, CancellationToken cancellationToken);
        public Task AddTechnicianServiceAreaAsync(TechnicianServiceArea technicianServiceArea, CancellationToken cancellationToken);
        public Task ChangeTechnicianServiceAreaAsync(int ServiceAreaId, TechnicianServiceArea technicianServiceArea, CancellationToken cancellationToken);
        public Task DeleteTechnicianServiceAreaAsync(TechnicianServiceArea technicianServiceArea, CancellationToken cancellationToken);
        public Task DeleteRangeTechnicianServiceAreaAsync(ICollection<TechnicianServiceArea> technicianServiceArea, CancellationToken cancellationToken);
        public Task<ICollection<TechnicianServiceArea>> GetAllTechnicianServiceAreasAsync(CancellationToken cancellationToken);
        public Task DeleteAllTechnicianServiceAreasWithSpecifyTechnicianIdAsync(string TechnicianId, CancellationToken cancellationToken);

        public Task<ICollection<TechnicianServiceArea>> GetTechnicianServiceAreasByTechnicianIdAsync(string TechnicianId, CancellationToken cancellationToken);
        public Task<ICollection<TechnicianServiceArea>> GetTechnicianServiceAreasByServiceAreaIdAsync(int ServiceAreaId, CancellationToken cancellationToken);

        public Task<bool> ServiceAreaHasTechniciansAsync(int ServiceAreaId, CancellationToken cancellationToken);
        public Task<bool> TechnicianHasServiceAreasAsync(string TechnicianId, CancellationToken cancellationToken);

        public Task<bool> TechnicianHasThisServiceAreaAsync(string TechnicianId, int ServiceAreaId, CancellationToken cancellationToken);
    }
}
