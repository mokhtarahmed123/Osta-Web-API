using Osta.Data.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Service.Service.TechnicianServiceFolder
{
    public class TechnicianServiceAreasService : ITechnicianServiceAreasService
    {
        private readonly ITechnicianServiceAreasRepository technicianServiceAreasRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILoggerService loggerService;

        public TechnicianServiceAreasService(ITechnicianServiceAreasRepository technicianServiceAreasRepository, IUnitOfWork unitOfWork, ILoggerService loggerService)
        {
            this.technicianServiceAreasRepository = technicianServiceAreasRepository;
            this.unitOfWork = unitOfWork;
            this.loggerService = loggerService;
        }
        public async Task AddTechnicianServiceAreasRangeAsync(ICollection<TechnicianServiceArea> technicianServiceAreas, CancellationToken cancellationToken)
        {
            using var transaction = unitOfWork.BeginTransactionAsync();
            try
            {
                await technicianServiceAreasRepository.AddRangeAsync(technicianServiceAreas, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
                loggerService.LogInformation(" Technician Service Areas Added Successfully . ");

            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to Add Technician Services  ");
                await unitOfWork.RollbackAsync();
                throw;

            }

        }

        public async Task AddTechnicianServiceAreaAsync(TechnicianServiceArea technicianServiceArea, CancellationToken cancellationToken)
        {
            try
            {
                await technicianServiceAreasRepository.AddAsync(technicianServiceArea, cancellationToken);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to Add Technician Service  ");
                throw;

            }


        }

        public async Task DeleteRangeTechnicianServiceAreaAsync(ICollection<TechnicianServiceArea> technicianServiceArea, CancellationToken cancellationToken)
        {

            await using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                await technicianServiceAreasRepository.DeleteRangeAsync(technicianServiceArea, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                loggerService.LogError(ex, $"Failed to Delete Technician Service Area  ");

                throw;
            }


        }

        public async Task DeleteTechnicianServiceAreaAsync(TechnicianServiceArea technicianServiceArea, CancellationToken cancellationToken)
        {

            await using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                await technicianServiceAreasRepository.DeleteAsync(technicianServiceArea, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                loggerService.LogError(ex, $"Failed to Add Technician Services Area     ");

                throw;
            }

        }

        public async Task<ICollection<TechnicianServiceArea>> GetTechnicianServiceAreasByServiceAreaIdAsync(int ServiceAreaId, CancellationToken cancellationToken)
        {
            return await technicianServiceAreasRepository.GetAllAsync(x => x.ServiceAreaId == ServiceAreaId, cancellationToken);


        }

        public async Task<ICollection<TechnicianServiceArea>> GetTechnicianServiceAreasByTechnicianIdAsync(string TechnicianId, CancellationToken cancellationToken)
        {
            return await technicianServiceAreasRepository.GetAllAsync(x => x.TechnicianId == TechnicianId, cancellationToken);

        }

        public async Task<bool> TechnicianHasThisServiceAreaAsync(string TechnicianId, int ServiceId, CancellationToken cancellationToken)
        {
            return await technicianServiceAreasRepository.TechnicianHasThisServiceArea(TechnicianId, ServiceId);

        }

        public async Task ChangeTechnicianServiceAreaAsync(int ServiceAreaId, TechnicianServiceArea technicianServiceArea, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await technicianServiceAreasRepository.GetByIdAsync(x =>
                    x.TechnicianId == technicianServiceArea.TechnicianId &&
                    x.ServiceAreaId == ServiceAreaId, cancellationToken);

                if (entity == null) return;
                await technicianServiceAreasRepository.DeleteAsync(entity, cancellationToken);


                await technicianServiceAreasRepository.AddAsync(new TechnicianServiceArea
                {
                    TechnicianId = technicianServiceArea.TechnicianId,
                    ServiceAreaId = technicianServiceArea.ServiceAreaId
                }, cancellationToken);

                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to Update Technician Services Area  ");
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task DeleteAllTechnicianServiceAreasWithSpecifyTechnicianIdAsync(string TechnicianId, CancellationToken cancellationToken)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {

                var AllTechnicianServiceAreasWithSpecifyTechnicianId = await technicianServiceAreasRepository.GetAllAsync(
                    x => x.TechnicianId == TechnicianId, cancellationToken, true
                    );
                await technicianServiceAreasRepository.DeleteRangeAsync(AllTechnicianServiceAreasWithSpecifyTechnicianId, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();



            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                loggerService.LogError(ex, $"Failed to Delete Technician Services  ");

                throw new Exception(ex.Message);

            }

        }

        public async Task<ICollection<TechnicianServiceArea>> GetAllTechnicianServiceAreasAsync(CancellationToken cancellationToken)
        {
            return await technicianServiceAreasRepository.GetAllAsync(cancellationToken);
        }

        public async Task<bool> ServiceAreaHasTechniciansAsync(int ServiceAreaId, CancellationToken cancellationToken)
        {
            return await technicianServiceAreasRepository.ExistsAsync(x => x.ServiceAreaId == ServiceAreaId, cancellationToken);
        }

        public async Task<bool> TechnicianHasServiceAreasAsync(string TechnicianId, CancellationToken cancellationToken)
        {
            return await technicianServiceAreasRepository.ExistsAsync(x => x.TechnicianId == TechnicianId, cancellationToken);
        }
    }
}
