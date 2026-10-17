using Osta.Domain.Entities.Technician;

namespace Osta.Service.Abstract.TechnicianAbstract
{
    public interface ITechnicianPayoutService
    {
        Task<TechnicianPayout> RequestPayoutAsync(
        string technicianId,
        decimal amount, PayoutMethod Method, string ReceivingDetails,
        CancellationToken cancellationToken = default);

        Task<TechnicianPayout?> GetPayoutByIdAsync(
            int payoutId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TechnicianPayout>> GetTechnicianPayoutsAsync(
            string technicianId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<TechnicianPayout>> GetPendingPayoutsAsync(
            CancellationToken cancellationToken = default);

        Task<bool> CompletePayoutAsync(
            int payoutId,
            CancellationToken cancellationToken = default);

        Task<bool> RejectPayoutAsync(
            int payoutId,
            string rejectionReason,
            CancellationToken cancellationToken = default);

        Task<bool> CancelPayoutAsync(
            int payoutId,
            string technicianId,
            CancellationToken cancellationToken = default);
    }
}
