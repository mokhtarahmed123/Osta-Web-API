using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.TechnicianAbstract;

namespace Osta.Service.Service.TechnicianServiceFolder
{
    public class TechnicianEarningService : ITechnicianEarningService
    {
        private readonly ITechnicianEarningRepository technicianEarningReposirotey;
        private readonly IUnitOfWork unitOfWork;

        public TechnicianEarningService(ITechnicianEarningRepository technicianEarningReposirotey, IUnitOfWork unitOfWork)
        {
            this.technicianEarningReposirotey = technicianEarningReposirotey;
            this.unitOfWork = unitOfWork;
        }
        public async Task<TechnicianEarning> CreateEarningAsync(TechnicianEarning technicianEarning, CancellationToken cancellationToken)
        {
            var Result = await technicianEarningReposirotey.AddAsync(technicianEarning, cancellationToken);
            await unitOfWork.SaveChangesAsync();
            return Result;
        }

        public async Task<TechnicianEarning?> GetByIdAsync(int earningId, CancellationToken cancellationToken)
        {
            return await technicianEarningReposirotey.GetByIdAsync(earningId, cancellationToken);

        }

        public async Task<IEnumerable<TechnicianEarning>> GetByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken)
        {
            return await technicianEarningReposirotey.GetTableNoTracking(cancellationToken).Where(s => s.TechnicianId == technicianId).ToListAsync();

        }

        public async Task<decimal> GetTotalEarningsAsync(string technicianId, CancellationToken cancellationToken)
        {

            return await technicianEarningReposirotey.GetTableNoTracking(cancellationToken).Where(a => a.TechnicianId == technicianId).SumAsync(s => s.GrossAmount);

        }

        public async Task<decimal> GetTotalNetEarningsAsync(string technicianId, CancellationToken cancellationToken)
        {
            return await technicianEarningReposirotey.GetTableNoTracking(cancellationToken).Where(a => a.TechnicianId == technicianId).SumAsync(s => s.NetAmount);
        }
    }
}
