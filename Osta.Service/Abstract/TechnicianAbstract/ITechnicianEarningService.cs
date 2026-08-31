using Osta.Domain.Entities.Technician;

namespace Osta.Service.Abstract.TechnicianAbstract
{
    public interface ITechnicianEarningService
    {
        Task<TechnicianEarning> CreateEarningAsync(
   TechnicianEarning technicianEarning,
      CancellationToken cancellationToken);

        Task<TechnicianEarning?> GetByIdAsync(
            int earningId,
            CancellationToken cancellationToken);

        Task<IEnumerable<TechnicianEarning>> GetByTechnicianIdAsync(
            string technicianId,
            CancellationToken cancellationToken);

        Task<decimal> GetTotalEarningsAsync(
            string technicianId,
            CancellationToken cancellationToken);

        Task<decimal> GetTotalNetEarningsAsync(
            string technicianId,
            CancellationToken cancellationToken);
    }
}
