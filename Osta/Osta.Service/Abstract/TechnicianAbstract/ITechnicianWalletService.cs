using Osta.Domain.Entities.Technician;

namespace Osta.Service.Abstract.TechnicianAbstract
{
    public interface ITechnicianWalletService
    {
        Task<TechnicianWallet?> GetWalletAsync(
        string technicianId,
        CancellationToken cancellationToken = default);

        Task<TechnicianWallet> CreateWalletAsync(
          TechnicianWallet technicianWallet,
            CancellationToken cancellationToken = default);

        Task AddAmountAsync(
            string technicianId,
            decimal amount,
            CancellationToken cancellationToken = default); // When earning Table Updated or Added Record

        Task<bool> DeductAmountAsync(
            string technicianId,
            decimal amount,
            CancellationToken cancellationToken = default); // When Payout 

        Task<decimal> GetBalanceAsync(
            string technicianId,
            CancellationToken cancellationToken = default);
    }
}
