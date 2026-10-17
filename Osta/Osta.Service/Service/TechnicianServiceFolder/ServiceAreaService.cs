using Osta.Data.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.Caching;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Service.Service.TechnicianServiceFolder
{
    public class ServiceAreaService : IServiceAreaService
    {
        private readonly IServiceAreaRepository serviceAreaRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly ICacheService cacheService;
        private const string ServiceAreasCacheKey = "service-areas";
        public ServiceAreaService(IServiceAreaRepository serviceAreaRepository, IUnitOfWork unitOfWork, ILoggerService loggerService, ITechnicianServiceAreasService technicianServiceAreasService, ICacheService cacheService)
        {
            this.serviceAreaRepository = serviceAreaRepository;
            this.unitOfWork = unitOfWork;
            this.loggerService = loggerService;
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.cacheService = cacheService;
        }
        public async Task AddServiceAreaAsync(ServiceArea ServiceArea, CancellationToken ct = default)
        {
            try
            {
                await serviceAreaRepository.AddAsync(ServiceArea, ct);
                await unitOfWork.SaveChangesAsync();
                await cacheService.RemoveAsync(ServiceAreasCacheKey);

            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to add Service Area  {ServiceArea.Id}");
                throw new Exception(ex.Message);

            }

        }

        public async Task DeleteServiceAreaAsync(int id, CancellationToken ct = default)
        {

            var ServiceArea = await serviceAreaRepository.GetByIdAsync(id, ct);
            await using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                await serviceAreaRepository.DeleteAsync(ServiceArea, ct);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                await cacheService.RemoveAsync(ServiceAreasCacheKey);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                loggerService.LogError(ex, $"Failed to update Service Area with id {id}");


                throw;
            }


        }

        public async Task<IEnumerable<ServiceArea>> GetAllServiceAreasAsync(CancellationToken ct = default)
        {
            var cachedServiceAreas =
                await cacheService.GetDataAsync<List<ServiceArea>>(ServiceAreasCacheKey);

            if (cachedServiceAreas is not null)
            {
                loggerService.LogInformation("Service areas loaded from cache.");
                return cachedServiceAreas;
            }

            var serviceAreas = (await serviceAreaRepository.GetAllAsync(ct)).ToList();

            await cacheService.SetDataAsync(
                ServiceAreasCacheKey,
                serviceAreas,
                TimeSpan.FromHours(1));

            loggerService.LogInformation("Service areas loaded from database and cached.");

            return serviceAreas;
        }
        public IQueryable<ServiceArea> GetAllServiceAreasQueryable(CancellationToken ct = default)
        {
            return serviceAreaRepository.GetTableNoTracking(ct).AsQueryable();

        }

        public async Task<ServiceArea?> GetServiceAreaAsync(int id, CancellationToken ct = default)
        {
            return await serviceAreaRepository.GetByIdAsync(id, ct);
        }

        public async Task<IEnumerable<ServiceArea>> GetServiceAreaWithSpecificTechIdAsync(string techId, CancellationToken ct = default)
        {
            var technicianServiceAreas = await technicianServiceAreasService
                .GetTechnicianServiceAreasByTechnicianIdAsync(techId, ct);

            var serviceAreaIds = technicianServiceAreas
                .Select(x => x.ServiceAreaId)
                .ToList();

            if (!serviceAreaIds.Any())
                return Enumerable.Empty<ServiceArea>();

            return await serviceAreaRepository.GetAllAsync(
                x => serviceAreaIds.Contains(x.Id), ct);
        }
        public async Task UpdateServiceAreaAsync(int id, ServiceArea ServiceArea, CancellationToken ct = default)
        {
            try
            {
                var existingServiceArea = await serviceAreaRepository.GetByIdAsync(id, ct);

                existingServiceArea.State = ServiceArea.State;
                existingServiceArea.City = ServiceArea.City;
                existingServiceArea.Name = ServiceArea.Name;

                await serviceAreaRepository.UpdateAsync(existingServiceArea, ct);
                await unitOfWork.SaveChangesAsync();
                await cacheService.RemoveAsync(ServiceAreasCacheKey);

            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to update Service Area  with id {id}");
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ServiceArea?> GetServiceByCity(string city, CancellationToken ct = default)
        {
            return await serviceAreaRepository.FirstOrDefaultAsync(x => x.City == city, ct);

        }
    }
}
