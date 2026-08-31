using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Osta.Core.HandlerMiddleware;
using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.Service.Model;
using Osta.SharedKernel;
using Osta.SharedKernel.Logging;

namespace Osta.Service.Service.TechnicianServiceFolder
{
    public class TechnicianImagesService : ITechnicianImagesService
    {
        private readonly IFileService imageUpload;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ITechnicianImagesRepository technicianImagesRepository;
        private readonly IWebHostEnvironment env;
        private readonly ILoggerService loggerService;
        private readonly IUnitOfWork unitOfWork;

        public TechnicianImagesService(IFileService imageUpload,
            IHttpContextAccessor httpContextAccessor, ITechnicianImagesRepository technicianImagesRepository,
            IWebHostEnvironment env, ILoggerService loggerService, IUnitOfWork unitOfWork)
        {
            this.imageUpload = imageUpload;
            this.httpContextAccessor = httpContextAccessor;
            this.technicianImagesRepository = technicianImagesRepository;
            this.env = env;
            this.loggerService = loggerService;
            this.unitOfWork = unitOfWork;
        }

        // Helper بيحول أي Relative Path لـ Full URL
        private string? BuildFullUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return relativePath;

            var request = httpContextAccessor.HttpContext?.Request
                ?? throw new InvalidOperationException("No HTTP context");

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return baseUrl + relativePath;
        }

        // بيحول الـ Entity كامل قبل الرجوع للـ Client
        private TechnicianImages MapToFullUrls(TechnicianImages entity)
        {
            return new TechnicianImages
            {
                TechnicianId = entity.TechnicianId,
                ProfilePicture = BuildFullUrl(entity.ProfilePicture),
                FrontNationalIdImage = BuildFullUrl(entity.FrontNationalIdImage) ?? string.Empty,
                BackNationalIdImage = BuildFullUrl(entity.BackNationalIdImage) ?? string.Empty
            };
        }

        public async Task<TechnicianImages> Add(string technicianId, TechnicianImageModel technicianImages, CancellationToken ct = default)
        {
            var location = $"Images/Technicians/{technicianId}";
            string? profileImagePath = null;
            string? frontNationalIdImagePath = null;
            string? backNationalIdImagePath = null;

            try
            {
                profileImagePath = await imageUpload.UploadImageAsync(technicianImages.ProfileImage, location, CancellationToken.None);
                frontNationalIdImagePath = await imageUpload.UploadImageAsync(technicianImages.FrontNationalIdImage, location, CancellationToken.None);
                backNationalIdImagePath = await imageUpload.UploadImageAsync(technicianImages.BackNationalIdImage, location, CancellationToken.None);

                var entity = new TechnicianImages
                {
                    TechnicianId = technicianId,
                    ProfilePicture = profileImagePath,
                    FrontNationalIdImage = frontNationalIdImagePath,
                    BackNationalIdImage = backNationalIdImagePath
                };

                await technicianImagesRepository.AddAsync(entity, ct);
                await unitOfWork.SaveChangesAsync();

                // بيرجع Full URL للـ Client
                return MapToFullUrls(entity);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to add technician images for id {technicianId}");

                if (profileImagePath is not null)
                    await imageUpload.DeleteImage(profileImagePath, location);
                if (frontNationalIdImagePath is not null)
                    await imageUpload.DeleteImage(frontNationalIdImagePath, location);
                if (backNationalIdImagePath is not null)
                    await imageUpload.DeleteImage(backNationalIdImagePath, location);

                throw;
            }
        }

        public async Task Delete(string technicianId, CancellationToken ct = default)
        {
            var existing = await technicianImagesRepository
                .FirstOrDefaultAsync(x => x.TechnicianId == technicianId, ct)
                ?? throw new NotFoundException($"No images found for technician with id {technicianId}");

            var location = $"Images/Technicians/{technicianId}";

            try
            {
                if (!string.IsNullOrEmpty(existing.ProfilePicture))
                    await imageUpload.DeleteImage(existing.ProfilePicture, location);
                if (!string.IsNullOrEmpty(existing.FrontNationalIdImage))
                    await imageUpload.DeleteImage(existing.FrontNationalIdImage, location);
                if (!string.IsNullOrEmpty(existing.BackNationalIdImage))
                    await imageUpload.DeleteImage(existing.BackNationalIdImage, location);

                await technicianImagesRepository.DeleteAsync(existing, ct);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to delete technician images for id {technicianId}");
                throw;
            }
        }

        public async Task<TechnicianImages> Get(string technicianId, CancellationToken ct = default)
        {
            var entity = await technicianImagesRepository.FirstOrDefaultAsync(x => x.TechnicianId == technicianId, ct)
                ?? throw new NotFoundException($"No images found for technician with id {technicianId}");

            return MapToFullUrls(entity);
        }

        public async Task<TechnicianImages> Update(string technicianId, TechnicianImageModel technicianImages, CancellationToken ct = default)
        {
            var existing = await technicianImagesRepository
                  .FirstOrDefaultAsync(x => x.TechnicianId == technicianId, ct)
                  ?? throw new NotFoundException($"No images found for technician with id {technicianId}");

            var location = $"Images/Technicians/{technicianId}";
            string? newProfilePath = null, newFrontPath = null, newBackPath = null;

            try
            {
                if (technicianImages.ProfileImage is not null)
                {
                    newProfilePath = await imageUpload.UploadImageAsync(technicianImages.ProfileImage, location, CancellationToken.None);
                    if (!string.IsNullOrEmpty(existing.ProfilePicture))
                        await imageUpload.DeleteImage(existing.ProfilePicture, location);
                    existing.ProfilePicture = newProfilePath;
                }

                if (technicianImages.FrontNationalIdImage is not null)
                {
                    newFrontPath = await imageUpload.UploadImageAsync(technicianImages.FrontNationalIdImage, location, CancellationToken.None);
                    if (!string.IsNullOrEmpty(existing.FrontNationalIdImage))
                        await imageUpload.DeleteImage(existing.FrontNationalIdImage, location);
                    existing.FrontNationalIdImage = newFrontPath;
                }

                if (technicianImages.BackNationalIdImage is not null)
                {
                    newBackPath = await imageUpload.UploadImageAsync(technicianImages.BackNationalIdImage, location, CancellationToken.None);
                    if (!string.IsNullOrEmpty(existing.BackNationalIdImage))
                        await imageUpload.DeleteImage(existing.BackNationalIdImage, location);
                    existing.BackNationalIdImage = newBackPath;
                }

                await unitOfWork.SaveChangesAsync();

                return MapToFullUrls(existing);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to update technician images for id {technicianId}");

                if (newProfilePath is not null)
                    await imageUpload.DeleteImage(newProfilePath, location);
                if (newFrontPath is not null)
                    await imageUpload.DeleteImage(newFrontPath, location);
                if (newBackPath is not null)
                    await imageUpload.DeleteImage(newBackPath, location);

                throw;
            }
        }
    }
}