using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Service.Service.TechnicianServiceFolder
{


    public class TechnicianServiceService : ITechnicianServiceService
    {
        private readonly ITechnicianServiceRepository technicianServiceRepo;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILoggerService loggerService;

        public TechnicianServiceService(ITechnicianServiceRepository technicianServiceRepo, IUnitOfWork unitOfWork, ILoggerService loggerService)
        {
            this.technicianServiceRepo = technicianServiceRepo;
            this.unitOfWork = unitOfWork;
            this.loggerService = loggerService;
        }
        public async Task AddAsync(Data.Entities.Technician.TechnicianService technicianService, CancellationToken cancellationToken)
        {
            try
            {
                await technicianServiceRepo.AddAsync(technicianService, cancellationToken);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to Add Technician Services  ");
                throw;
            }
        }

        public async Task AddRangeAsync(IEnumerable<Data.Entities.Technician.TechnicianService> technicianServices, CancellationToken cancellationToken)
        {
            try
            {
                await technicianServiceRepo.AddRangeAsync((ICollection<Data.Entities.Technician.TechnicianService>)technicianServices, cancellationToken);
                await unitOfWork.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                loggerService.LogError("Updated Added ");
                throw new Exception(ex.Message, ex);

            }

        }

        public async Task DeleteAsync(Data.Entities.Technician.TechnicianService technicianService, CancellationToken cancellationToken)
        {
            var TechnicianService = await
                technicianServiceRepo.FirstOrDefaultAsync
                (x => x.ServiceId == technicianService.ServiceId &&
                x.TechnicianId == technicianService.TechnicianId, cancellationToken);

            if (TechnicianService == null) return;
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                await technicianServiceRepo.DeleteAsync(TechnicianService, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                loggerService.LogError(ex, $"Failed to Delete  Technician Service  ");

                throw;
            }

        }

        public async Task DeleteAllService_technicianBy_ServiceIdAsync(int ServiceId, CancellationToken cancellationToken)
        {
            try
            {
                var AllService_technicianBy_ServiceId = await technicianServiceRepo.GetAllByServiceId(ServiceId);
                using var transaction = await unitOfWork.BeginTransactionAsync();
                await technicianServiceRepo.DeleteRangeAsync
                    ((ICollection<Data.Entities.Technician.TechnicianService>)AllService_technicianBy_ServiceId, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                loggerService.LogError(ex, $"Failed to Delete Technician Services By Service Id ");
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task DeleteAllService_technicianBy_technicianIdAsync(string technicianId, CancellationToken cancellationToken)
        {
            try
            {
                var AllService_technicianBy_ServiceId = await technicianServiceRepo.GetAllByTechnicianId(technicianId);
                using var transaction = await unitOfWork.BeginTransactionAsync();
                await technicianServiceRepo.DeleteRangeAsync
                    ((ICollection<Data.Entities.Technician.TechnicianService>)AllService_technicianBy_ServiceId, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                throw new Exception(ex.Message, ex);

            }
        }

        public async Task DeleteByServiceIdAsync(int serviceId, CancellationToken cancellationToken) // 
        {
            var TechnicianService = await technicianServiceRepo.FirstOrDefaultAsync(x => x.ServiceId == serviceId, cancellationToken);
            if (TechnicianService == null) return;
            var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                await technicianServiceRepo.DeleteAsync(TechnicianService, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteServiceByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken)
        {
            var TechnicianService = await technicianServiceRepo.FirstOrDefaultAsync(x => x.TechnicianId == technicianId, cancellationToken
                );
            if (TechnicianService == null) return;
            var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                await technicianServiceRepo.DeleteAsync(TechnicianService, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }

            }
        }

        public async Task<bool> ExistsAsync(string technicianId, int serviceId, CancellationToken cancellationToken)
        {
            var result = await technicianServiceRepo.FirstOrDefaultAsync(
                 x => x.TechnicianId == technicianId && x.ServiceId == serviceId, cancellationToken
             );
            if (result == null) return false;
            return true;
        }

        public async Task<IEnumerable<Data.Entities.Technician.TechnicianService>> GetAllAsync(CancellationToken cancellationToken)
        {
            var Result = await technicianServiceRepo.GetAllAsync(cancellationToken);
            return Result;

        }

        public async Task<Data.Entities.Technician.TechnicianService?> GetByIdAsync(string technicianId, int serviceId, CancellationToken cancellationToken)
        {
            var result = await technicianServiceRepo.FirstOrDefaultAsync(
                 x => x.TechnicianId == technicianId && x.ServiceId == serviceId, cancellationToken
             );

            return result;

        }

        public async Task<IEnumerable<Data.Entities.Technician.TechnicianService>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken)
        {
            return await technicianServiceRepo.GetAllByServiceId(serviceId);
        }

        public async Task<IEnumerable<Data.Entities.Technician.TechnicianService>> GetByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken)
        {
            return await technicianServiceRepo.GetAllByTechnicianId(technicianId);

        }

        public async Task UpdateAsync(Data.Entities.Technician.TechnicianService technicianService, CancellationToken cancellationToken)
        {
            try
            {
                var existingtechnicianService = await technicianServiceRepo.
                    FirstOrDefaultAsync(x => x.TechnicianId == technicianService.TechnicianId
                && x.ServiceId == technicianService.ServiceId, cancellationToken);
                if (existingtechnicianService == null) return;
                existingtechnicianService.ServiceId = technicianService.ServiceId;
                existingtechnicianService.TechnicianId = technicianService.TechnicianId;


                await technicianServiceRepo.UpdateAsync(existingtechnicianService, cancellationToken);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to Update  Technician Services  ");
                throw new Exception(ex.Message, ex);
            }


        }
    }
}
