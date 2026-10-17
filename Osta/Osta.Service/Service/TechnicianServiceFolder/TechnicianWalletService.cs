using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.TechnicianAbstract;

namespace Osta.Service.Service.TechnicianServiceFolder
{
    public class TechnicianWalletService : ITechnicianWalletService
    {
        private readonly ITechnicianWalletRepository technicianWalletRepository;
        private readonly IUnitOfWork unitOfWork;

        public TechnicianWalletService(ITechnicianWalletRepository technicianWalletRepository, IUnitOfWork unitOfWork)
        {
            this.technicianWalletRepository = technicianWalletRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task AddAmountAsync(string technicianId, decimal amount, CancellationToken cancellationToken = default)
        {
            var wallet = await technicianWalletRepository.FirstOrDefaultAsync(x => x.TechnicianId == technicianId, cancellationToken);
            if (wallet == null) return;
            wallet.Amount += amount;
            await technicianWalletRepository.UpdateAsync(wallet, cancellationToken);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<TechnicianWallet> CreateWalletAsync(TechnicianWallet technicianWallet, CancellationToken cancellationToken = default)
        {
            var wallet = await technicianWalletRepository.AddAsync(technicianWallet, cancellationToken);
            await unitOfWork.SaveChangesAsync();
            if (wallet != null)
                return wallet;
            return null;


        }

        public async Task<bool> DeductAmountAsync(string technicianId, decimal amount, CancellationToken cancellationToken = default)
        {
            var wallet = await technicianWalletRepository.FirstOrDefaultAsync(x => x.TechnicianId == technicianId, cancellationToken);
            wallet.Amount -= amount;
            await technicianWalletRepository.UpdateAsync(wallet, cancellationToken);
            await unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetBalanceAsync(string technicianId, CancellationToken cancellationToken = default)
        {
            var wallet = await technicianWalletRepository.FirstOrDefaultAsync(x => x.TechnicianId == technicianId, cancellationToken);
            if (wallet == null) return 0;
            return wallet.Amount;
        }

        public async Task<TechnicianWallet?> GetWalletAsync(string technicianId, CancellationToken cancellationToken = default)
        {
            return await technicianWalletRepository.FirstOrDefaultAsync(x => x.TechnicianId == technicianId, cancellationToken);

        }
    }
}
