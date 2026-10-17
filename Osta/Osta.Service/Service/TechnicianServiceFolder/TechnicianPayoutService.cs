using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Technician;
using Osta.Domain.Enum;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.TechnicianAbstract;

namespace Osta.Service.Service.TechnicianServiceFolder
{
    public class TechnicianPayoutService : ITechnicianPayoutService
    {
        private readonly ITechnicianPayoutRepository technicianPayoutRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ITechnicianWalletRepository technicianWalletRepository;

        public TechnicianPayoutService(ITechnicianPayoutRepository technicianPayoutRepository, IUnitOfWork unitOfWork, ITechnicianWalletRepository technicianWalletRepository)
        {
            this.technicianPayoutRepository = technicianPayoutRepository;
            this.unitOfWork = unitOfWork;
            this.technicianWalletRepository = technicianWalletRepository;
        }
        public async Task<bool> CancelPayoutAsync(int payoutId, string technicianId, CancellationToken cancellationToken = default)
        {
            try
            {
                var payout = await technicianPayoutRepository
                    .GetByIdAsync(payoutId, cancellationToken);

                if (payout is null)
                    throw new KeyNotFoundException(
                        "Payout was not found.");

                if (payout.TechnicianId != technicianId)
                    throw new UnauthorizedAccessException(
                        "You are not allowed to cancel this payout.");
                // 3. Can only cancel Pending payout
                if (payout.Status != PayoutStatus.Pending)
                    throw new InvalidOperationException(
                        "Only pending payouts can be cancelled.");

                payout.Status = Domain.Enum.PayoutStatus.Cancelled;

                await technicianPayoutRepository.UpdateAsync(payout, cancellationToken);

                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed To Cancel Payout With Id => {payoutId} , {ex.Message} ");
            }

        }

        public async Task<bool> CompletePayoutAsync(
      int payoutId,
      CancellationToken cancellationToken = default)
        {
            var payout = await technicianPayoutRepository
                .GetByIdAsync(payoutId, cancellationToken);

            if (payout is null)
                return false;

            // لازم يكون Pending
            if (payout.Status != PayoutStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending payouts can be completed.");


            var wallet = await technicianWalletRepository
       .FirstOrDefaultAsync(x => x.TechnicianId == payout.TechnicianId,
            cancellationToken);


            if (wallet is null)
                throw new KeyNotFoundException(
                    "Technician wallet was not found.");

            if (wallet.Amount < payout.Amount)
                throw new InvalidOperationException(
                    "Insufficient wallet balance.");


            wallet.Amount -= payout.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;


            payout.Status = PayoutStatus.Completed;
            payout.CompletedAt = DateTime.UtcNow;

            await technicianWalletRepository.UpdateAsync(wallet, cancellationToken);
            await technicianPayoutRepository.UpdateAsync(payout, cancellationToken);

            await unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<TechnicianPayout?> GetPayoutByIdAsync(int payoutId, CancellationToken cancellationToken = default)
        {
            return await technicianPayoutRepository.GetByIdAsync(payoutId, cancellationToken);

        }

        public async Task<IEnumerable<TechnicianPayout>> GetPendingPayoutsAsync(CancellationToken cancellationToken = default)
        {
            return await technicianPayoutRepository.GetTableNoTracking(cancellationToken).

                Where(c => c.Status == Domain.Enum.PayoutStatus.Pending).ToListAsync();
        }

        public async Task<IEnumerable<TechnicianPayout>> GetTechnicianPayoutsAsync(string technicianId, CancellationToken cancellationToken = default)
        {
            return await technicianPayoutRepository.GetTableNoTracking(cancellationToken).

                Where(c => c.TechnicianId == technicianId).ToListAsync();
        }

        public async Task<bool> RejectPayoutAsync(
            int payoutId,
            string rejectionReason,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
                throw new ArgumentException(
                    "Rejection reason is required.");

            var payout = await technicianPayoutRepository
                .GetByIdAsync(payoutId, cancellationToken);

            if (payout is null)
                return false;

            if (payout.Status != PayoutStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending payouts can be rejected.");

            payout.Status = PayoutStatus.Rejected;
            payout.RejectionReason = rejectionReason;

            await technicianPayoutRepository.UpdateAsync(payout, cancellationToken);

            await unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<TechnicianPayout> RequestPayoutAsync(
         string technicianId,
         decimal amount, PayoutMethod Method, string ReceivingDetails,
         CancellationToken cancellationToken = default)
        {
            var payout = new TechnicianPayout
            {
                TechnicianId = technicianId,
                Amount = amount,
                Status = PayoutStatus.Pending,
                RequestedAt = DateTime.UtcNow,
                Method = Method,
                ReceivingDetails = ReceivingDetails,
                CompletedAt = null
            };

            var result = await technicianPayoutRepository
                .AddAsync(payout, cancellationToken);

            await unitOfWork.SaveChangesAsync();

            return result;
        }
    }
}
