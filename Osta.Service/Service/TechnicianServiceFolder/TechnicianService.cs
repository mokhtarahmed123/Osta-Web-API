using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Technician;
using Osta.Data.Enum;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Service.Service.TechnicianServiceFolder
{
    public class TechnicianService : ITechnicianService
    {
        private readonly ITechnicianRepository technicianRepository;
        private readonly ILoggerService loggerService;
        private readonly IUnitOfWork unitOfWork;



        private readonly ITechnicianServiceRepository technicianServiceRepository;
        private readonly ITechnicianServiceAreasRepository technicianServiceAreasRepository;
        private readonly IReviewService reviewService;

        public TechnicianService(ITechnicianRepository technicianRepository, ILoggerService loggerService,
            IUnitOfWork unitOfWork, ITechnicianServiceRepository technicianServiceRepository,
            ITechnicianServiceAreasRepository technicianServiceAreasRepository, IReviewService reviewService)
        {
            this.technicianRepository = technicianRepository;
            this.loggerService = loggerService;
            this.unitOfWork = unitOfWork;
            this.technicianServiceRepository = technicianServiceRepository;
            this.technicianServiceAreasRepository = technicianServiceAreasRepository;
            this.reviewService = reviewService;
        }
        public async Task AddTechnicianAsync(Technicians Technicians, CancellationToken ct = default)
        {
            try
            {
                await technicianRepository.AddAsync(Technicians, ct);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to Add Technician  with id {Technicians.Id}");
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteTechnicianAsync(string id, CancellationToken ct = default)
        {
            var Technician = await technicianRepository.GetByIdAsync(id, ct);

            await using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                await technicianRepository.DeleteAsync(Technician, ct);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                {
                    await transaction.RollbackAsync();
                    loggerService.LogError(ex, $"Failed to Delete Technician  with id {id}");

                    throw new Exception(ex.Message);

                }
            }

        }

        public async Task<IEnumerable<Technicians>> GetAllTechniciansAsync(CancellationToken ct = default)
        {

            var Result = await technicianRepository.GetAllAsync(ct);
            return Result;

        }

        public IQueryable<Technicians> GetTechniciansQueryable(CancellationToken ct = default)
        {
            return technicianRepository.GetTableNoTracking(ct).AsQueryable();

        }

        public async Task<Technicians?> GetTechnicianAsync(string id, CancellationToken ct = default)
        {
            return await technicianRepository.GetByIdAsync(id, ct);
        }

        public async Task<bool> TechnicianExistsAsync(string id, CancellationToken ct = default)
        {
            var Technician = await technicianRepository.GetByIdAsync(id, ct);
            if (Technician is null) return false;
            return true;

        }

        public async Task UpdateTechnicianAsync(string id, Technicians Technicians, CancellationToken ct = default)
        {
            try
            {
                var existingTechnician = await technicianRepository.GetByIdAsync(id, ct);
                if (existingTechnician is null) return;
                existingTechnician.YearsOfExperience = Technicians.YearsOfExperience;
                existingTechnician.Bio = Technicians.Bio;
                existingTechnician.NationalId = Technicians.NationalId;
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to Update Technician with id {Technicians.Id}");
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<Technicians>> GetTechniciansByMinimumRateAsync(double rating, CancellationToken ct = default)
        {
            return await technicianRepository.GetAllAsync(
                x => x.Rating >= rating, ct);
        }
        public async Task<IEnumerable<Technicians>> GetTechniciansByServiceIdAsync(int ServiceId, CancellationToken ct = default)
        {
            return await technicianRepository.GetAllWithServiceId(ServiceId);
        }

        public async Task<IEnumerable<Technicians>> GetTechniciansByServiceAreaIdAsync(int ServiceAreaId, CancellationToken ct = default)
        {
            return await technicianRepository.GetAllTechniciansWithServiceArea(ServiceAreaId);
        }

        public async Task<Technicians> GetTechnicianWithServiceAndServiceAreaAsync(string Id, CancellationToken ct = default)
        {
            return await technicianRepository.GetByIdWithServiceAndServiceArea(Id);
        }

        public async Task VerifyRequestOfTechnicianAsync(string id, CancellationToken ct = default)
        {

            var technician = await technicianRepository.GetByIdAsync(id, ct);

            if (technician is null)
                throw new KeyNotFoundException("Technician not found.");

            technician.IsVerified = true;
            technician.Status = StatusOfTechnicianRequestEnum.Accepted;

            await technicianRepository.UpdateAsync(technician, ct);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RejectRequestOfTechnicianAsync(string id, string ReasonOfReject, CancellationToken ct = default)
        {
            var technician = await technicianRepository.GetByIdAsync(id, ct);

            if (technician is null)
                throw new KeyNotFoundException("Technician not found.");

            technician.IsVerified = false;
            technician.Status = StatusOfTechnicianRequestEnum.Rejected;
            technician.ReasonOfReject = ReasonOfReject;

            await technicianRepository.UpdateAsync(technician, ct);
            await unitOfWork.SaveChangesAsync();

        }

        public async Task CompleteBooking(string id, CancellationToken ct = default)
        {
            var technician = await technicianRepository.GetByIdAsync(id, ct);
            if (technician is null) return;
            technician.CompletedBookings++;
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RateTechnicianAsync(string id, CancellationToken cancellationToken = default)
        {

            var technician = await technicianRepository.GetByIdAsync(id, cancellationToken);

            if (technician is null)
                throw new KeyNotFoundException("Technician not found.");

            technician.Rating =
                await reviewService.GetAllRatingWithTechnicianId(
                    id,
                    cancellationToken);

            await unitOfWork.SaveChangesAsync();


        }

        public async Task UpdateReviewCount(string technicianId,
    int change, CancellationToken cancellationToken = default)
        {
            var technician =
                 await technicianRepository.GetByIdAsync(technicianId, cancellationToken);

            if (technician is null)
                throw new KeyNotFoundException("Technician not found.");

            technician.TotalReviews += change;

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<Technicians?> MyProfile(string Id, CancellationToken ct = default)
        {
            return await technicianRepository.GetTableNoTracking(ct).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == Id, ct);

        }
    }
}
