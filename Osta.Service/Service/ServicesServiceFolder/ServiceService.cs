using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Osta.Infrastructure.Abstract.ServicesAbstract;
using Osta.Infrastructure.Caching;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel;
using Osta.SharedKernel.Logging;
using System.Diagnostics;

namespace Osta.Service.Service.ServicesServiceFolder
{
    using Service = Osta.Data.Entities.Services.Service;
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository serviceRepository;
        private readonly IFileService imageUpload;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebHostEnvironment env;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianServiceService technicianServiceService;
        private readonly ICacheService cacheService;
        private const string ServicesCacheKey = "services";

        public ServiceService(IServiceRepository serviceRepository, IFileService imageUpload,
            IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment env, ILoggerService loggerService, ITechnicianServiceService technicianServiceService, ICacheService cacheService)
        {
            this.serviceRepository = serviceRepository;
            this.imageUpload = imageUpload;
            this.unitOfWork = unitOfWork;
            this.httpContextAccessor = httpContextAccessor;
            this.env = env;
            this.loggerService = loggerService;
            this.technicianServiceService = technicianServiceService;
            this.cacheService = cacheService;
        }
        public async Task AddServiceAsync(Service service, IFormFile? formFile, CancellationToken ct = default)
        {
            if (formFile != null)
            {
                var request = httpContextAccessor.HttpContext?.Request
                   ?? throw new InvalidOperationException("No HTTP context");
                var baseUrl = $"{request.Scheme}://{request.Host}";

                var location = $"Images/Services/{service.Id}";

                var imagePath = await imageUpload.UploadImageAsync(formFile, location, ct);
                service.ImageUrl = baseUrl + imagePath;
            }
            try
            {
                await serviceRepository.AddAsync(service, ct);
                await unitOfWork.SaveChangesAsync();
                await cacheService.RemoveAsync(ServicesCacheKey);


                loggerService.LogInformation($"Service {service.Id} added successfully");

            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Failed to add service {ServiceName}", service.Name);

                if (service.ImageUrl != null)
                    await imageUpload.DeleteImage(service.ImageUrl, "Images/Services");
                throw;
            }
        }

        public async Task DeleteServiceAsync(int id, CancellationToken ct = default)
        {
            var Service = await serviceRepository.GetByIdAsync(id, ct);
            var Iamge = Service.ImageUrl;
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                await serviceRepository.DeleteAsync(Service, ct);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                await cacheService.RemoveAsync(ServicesCacheKey);

                loggerService.LogInformation($"Service {id} deleted successfully");

            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to delete service With Id : {id}");
                await transaction.RollbackAsync();
                throw;
            }

            if (!string.IsNullOrEmpty(Iamge))
            {
                await imageUpload.DeleteImage(Iamge, "Images/Services");
            }


        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync(CancellationToken ct = default)
        {
            var cachedServices =
                await cacheService.GetDataAsync<List<Service>>(ServicesCacheKey);

            if (cachedServices is not null)
            {
                loggerService.LogInformation("Services loaded from cache.");
                return cachedServices;
            }

            var sw = Stopwatch.StartNew();

            var services = (await serviceRepository.GetAllWithCategoryAsync(ct)).ToList();

            sw.Stop();

            loggerService.LogInformation(
                "Database took {Elapsed} ms",
                sw.ElapsedMilliseconds);

            await cacheService.SetDataAsync(
                ServicesCacheKey,
                services,
                TimeSpan.FromMinutes(30));

            loggerService.LogInformation("Services cached successfully.");

            return services;
        }
        public IQueryable<Service> GetAllServicesQueryable()
        {
            var Ct = CancellationToken.None;
            return serviceRepository.GetTableNoTracking(Ct).AsQueryable();

        }

        public async Task<Service?> GetServiceAsync(int id, CancellationToken ct = default)
        {
            return await serviceRepository.GetByIdAsync(id, ct);
        }
        public async Task<IEnumerable<Service>> GetServicesByTechnicianIdAsync(string techId, CancellationToken ct = default)
        {
            var technicianServices = await technicianServiceService.GetByTechnicianIdAsync(techId, ct);

            var serviceIds = technicianServices
                .Select(x => x.ServiceId)
                .ToList();

            if (!serviceIds.Any())
                return Enumerable.Empty<Service>();

            return await serviceRepository.GetAllAsync(
                x => serviceIds.Contains(x.Id), ct);
        }

        public async Task<bool> DoesCategoryHaveServiceAsync(int CategoryId, CancellationToken ct = default)
        {
            return await serviceRepository.DoesCategoryHaveServiceAsync(CategoryId, ct);
        }

        public async Task UpdateServiceAsync(int id, Service service, IFormFile? formFile, CancellationToken ct = default)
        {
            try
            {
                var existingService = await serviceRepository.GetByIdAsync(id, ct);

                existingService.Name = service.Name;
                existingService.Description = service.Description;
                existingService.Price = service.Price;
                existingService.CategoryId = service.CategoryId;
                existingService.IsActive = service.IsActive;


                if (formFile != null)
                {
                    var request = httpContextAccessor.HttpContext?.Request
                        ?? throw new InvalidOperationException("No HTTP context");

                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    var location = $"Images/Services/{id}";

                    var imagePath = await imageUpload.UploadImageAsync(formFile, location, ct);

                    var oldImageUrl = existingService.ImageUrl;

                    existingService.ImageUrl = baseUrl + imagePath;

                    if (!string.IsNullOrEmpty(oldImageUrl))
                    {
                        await imageUpload.DeleteImage(oldImageUrl, $"Images/Services/{id}");
                    }
                }

                await serviceRepository.UpdateAsync(existingService, ct);
                await unitOfWork.SaveChangesAsync();
                await cacheService.RemoveAsync(ServicesCacheKey);

                loggerService.LogInformation("Service {ServiceId} updated successfully", id);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Failed to update address with id {ServiceId}", id);

                throw new Exception(ex.Message, ex);
            }

        }
    }
}
