using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities;
using Osta.Infrastructure.Abstract.CustomerAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.CustomerAbstract;

namespace Osta.Service.Service.CustomerServiceFolder
{
    public class FavoriteTechnicianService : IFavoriteTechnicianService
    {
        private readonly IFavoriteTechnicianRepository favoriteTechnicianRepository;
        private readonly IUnitOfWork unitOfWork;

        public FavoriteTechnicianService(
            IFavoriteTechnicianRepository favoriteTechnicianRepository,
            IUnitOfWork unitOfWork)
        {
            this.favoriteTechnicianRepository =
                favoriteTechnicianRepository;

            this.unitOfWork = unitOfWork;
        }

        public async Task Add(
            FavoriteTechnician favoriteTechnician, CancellationToken ct = default)
        {
            await favoriteTechnicianRepository.AddAsync(favoriteTechnician, ct);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(
            string customerId,
            string technicianId, CancellationToken ct = default)
        {

            var favorite =
                await favoriteTechnicianRepository
                    .GetTableAsTracking(ct)
                    .FirstOrDefaultAsync(x =>
                        x.CustomerId == customerId &&
                        x.TechnicianId == technicianId);

            if (favorite is null)
                throw new KeyNotFoundException(
                    "Technician is not in your favorites.");

            await favoriteTechnicianRepository.DeleteAsync(
                favorite, ct);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<FavoriteTechnician>> GetMyFavorites(string CustomerId, CancellationToken ct = default)
        {
            return await favoriteTechnicianRepository
                .GetTableAsTracking(ct)
                .Where(a => a.CustomerId == CustomerId)
                .ToListAsync();
        }
    }
}