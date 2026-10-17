using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.Caching;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;
using System.Diagnostics;

namespace Osta.Service.Service.TechnicianServiceFolder
{
    public class TechnicianAvailabilityService : ITechnicianAvailabilityService
    {
        private readonly ITechnicianAvailabilityRepository technicianAvailabilityRepository;
        private readonly ILoggerService loggerService;
        private readonly ICacheService cacheService;
        private readonly IUnitOfWork unitOfWork;
        private const string TechnicianAvailabilityCacheKey = "TechnicianAvailability";

        public TechnicianAvailabilityService(ITechnicianAvailabilityRepository technicianAvailabilityRepository, ILoggerService loggerService, ICacheService cacheService, IUnitOfWork unitOfWork)
        {
            this.technicianAvailabilityRepository = technicianAvailabilityRepository;
            this.loggerService = loggerService;
            this.cacheService = cacheService;
            this.unitOfWork = unitOfWork;
        }
        public async Task AddTechnicianAvailabilityAsync(TechnicianAvailability TechnicianAvailability, CancellationToken ct = default)
        {
            try
            {
                await technicianAvailabilityRepository.AddAsync(TechnicianAvailability, ct);
                await unitOfWork.SaveChangesAsync();
                await cacheService.RemoveAsync(TechnicianAvailabilityCacheKey);

            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to Add Technician Availability with id {TechnicianAvailability.Id}");
                throw;

            }

        }

        public async Task DeleteTechnicianAvailabilityAsync(int id, string TechnicianId, CancellationToken ct = default)
        {
            var technicianAvailability = await technicianAvailabilityRepository.FirstOrDefaultAsync(
                x => x.Id == id && x.TechnicianId == TechnicianId, ct
                );
            await using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                await technicianAvailabilityRepository.DeleteAsync(technicianAvailability, ct);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                await cacheService.RemoveAsync(TechnicianAvailabilityCacheKey);

            }
            catch (Exception ex)
            {
                {
                    await transaction.RollbackAsync();
                    loggerService.LogError(ex, $"Failed to Delete Technician Availability with id {id}");
                    throw;
                }

            }
        }

        public IQueryable<TechnicianAvailability> GetAllTechnicianAvailabilityQueryable(CancellationToken ct = default)
        {
            return technicianAvailabilityRepository.GetTableNoTracking(ct).AsQueryable();

        }

        public async Task<IEnumerable<TechnicianAvailability>> GetAllTechnicianAvailabilitiesAsync(CancellationToken ct = default)
        {
            var sw = Stopwatch.StartNew();

            var cachedAvailabilities =
                await cacheService.GetDataAsync<List<TechnicianAvailability>>(TechnicianAvailabilityCacheKey);

            sw.Stop();

            loggerService.LogInformation($"Redis Time: {sw.ElapsedMilliseconds} ms");
            if (cachedAvailabilities is not null)
            {
                loggerService.LogInformation("Technician Availabilities loaded from cache.");

                return cachedAvailabilities;
            }

            var TechnicianAvailabilities = (await technicianAvailabilityRepository.GetAllAsync(ct)).ToList();

            await cacheService.SetDataAsync(
                TechnicianAvailabilityCacheKey,
                TechnicianAvailabilities, TimeSpan.FromMinutes(30));

            loggerService.LogInformation("Technician Availabilities loaded from database and cached.");

            return TechnicianAvailabilities;
        }

        public async Task<TechnicianAvailability?> GetTechnicianAvailabilityAsync(int id, CancellationToken ct = default)
        {
            //var cachedTechnicianAvailability = await cacheService.GetDataAsync<TechnicianAvailability>(TechnicianAvailabilityCacheKey);

            //if (cachedTechnicianAvailability is not null)
            //{
            //    return cacheTechnicianAvailability;
            //}

            var TechnicianAvailability = await technicianAvailabilityRepository.GetByIdAsync(id, ct);

            //if (address is not null)
            //{
            //    await cacheService.SetDataAsync(
            //        AddressCacheKey,
            //        address,
            //        TimeSpan.FromMinutes(10));
            //}

            return TechnicianAvailability;

        }

        public async Task UpdateTechnicianAvailabilityAsync(int id, TechnicianAvailability TechnicianAvailability, CancellationToken ct = default)
        {
            try
            {
                var existingTechnicianAvailability = await technicianAvailabilityRepository.GetByIdAsync(id, ct);

                existingTechnicianAvailability.DayOfWeek = TechnicianAvailability.DayOfWeek;
                existingTechnicianAvailability.StartTime = TechnicianAvailability.StartTime;
                existingTechnicianAvailability.EndTime = TechnicianAvailability.EndTime;
                existingTechnicianAvailability.IsAvailable = TechnicianAvailability.IsAvailable;


                await technicianAvailabilityRepository.UpdateAsync(existingTechnicianAvailability, ct);
                await unitOfWork.SaveChangesAsync();
                await cacheService.RemoveAsync(TechnicianAvailabilityCacheKey);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to Update Technician Availability with id {TechnicianAvailability.Id}");
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> HasOverlappingAvailabilityAsync(TechnicianAvailability technicianAvailability, CancellationToken ct = default)
        {
            return await technicianAvailabilityRepository.ExistsAsync(x =>
         x.TechnicianId == technicianAvailability.TechnicianId &&
         x.DayOfWeek == technicianAvailability.DayOfWeek &&
         x.StartTime == technicianAvailability.StartTime &&
             x.EndTime == technicianAvailability.EndTime, ct);
        }

        public async Task<TechnicianAvailability?> GetTechnicianAvailabilityForTechnicianAsync(int id, string TechnicianId, CancellationToken ct = default)
        {
            return await technicianAvailabilityRepository.FirstOrDefaultAsync(
       x => x.Id == id && x.TechnicianId == TechnicianId, ct
       );

        }

        public async Task<IEnumerable<TechnicianAvailability>> GetAllTechnicianAvailabilitiesByTechnicianIdAsync(string technicianId, CancellationToken ct = default)
        {
            return await technicianAvailabilityRepository.GetAllAsync(x => x.TechnicianId == technicianId, ct);
        }

        public async Task<bool> HasOverlappingAvailabilityForUpdateAsync(TechnicianAvailability technicianAvailability, CancellationToken ct = default)
        {
            return await technicianAvailabilityRepository.GetTableNoTracking(ct)
                  .AnyAsync(x =>
                      x.Id != technicianAvailability.Id &&
                      x.TechnicianId == technicianAvailability.TechnicianId &&
                      x.DayOfWeek == technicianAvailability.DayOfWeek &&
                      x.StartTime < technicianAvailability.EndTime &&
                      x.EndTime > technicianAvailability.StartTime);
        }
    }
}
