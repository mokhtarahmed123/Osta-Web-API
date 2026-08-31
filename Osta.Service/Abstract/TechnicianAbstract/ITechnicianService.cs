using Osta.Data.Entities.Technician;

namespace Osta.Service.Abstract.TechnicianAbstract
{

    public interface ITechnicianService
    {
        Task AddTechnicianAsync(Technicians technician, CancellationToken ct = default);
        Task UpdateTechnicianAsync(string id, Technicians technician, CancellationToken ct = default);
        Task DeleteTechnicianAsync(string id, CancellationToken ct = default);
        Task<bool> TechnicianExistsAsync(string id, CancellationToken ct = default);
        Task<Technicians?> GetTechnicianAsync(string id, CancellationToken ct = default);
        Task<IEnumerable<Technicians>> GetAllTechniciansAsync(CancellationToken ct = default);
        public IQueryable<Technicians> GetTechniciansQueryable(CancellationToken ct = default);
        public Task<IEnumerable<Technicians>> GetTechniciansByMinimumRateAsync(double MinRate, CancellationToken ct = default);
        public Task<IEnumerable<Technicians>> GetTechniciansByServiceIdAsync(int ServiceId, CancellationToken ct = default);
        public Task<IEnumerable<Technicians>> GetTechniciansByServiceAreaIdAsync(int ServiceAreaId, CancellationToken ct = default);
        Task<Technicians> GetTechnicianWithServiceAndServiceAreaAsync(string Id, CancellationToken ct = default);

        Task VerifyRequestOfTechnicianAsync(string id, CancellationToken ct = default);
        Task RejectRequestOfTechnicianAsync(string id, string ReasonOfReject, CancellationToken ct = default);

        Task CompleteBooking(string id, CancellationToken ct = default);

        Task RateTechnicianAsync(string Id, CancellationToken ct = default);
        Task<Technicians> MyProfile(string Id, CancellationToken ct = default);
        Task UpdateReviewCount(string Id, int change, CancellationToken ct = default);

    }
}
