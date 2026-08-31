using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Osta.Booking.Interface;
using Osta.Data.Entities.Booking;
using Osta.Domain.Enum;
using Osta.Infrastructure.Abstract.BookingAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.SharedKernel;

namespace Osta.Booking.Service
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository mediaRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFileService imageUpload;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MediaService(IMediaRepository mediaRepository, IUnitOfWork unitOfWork, IFileService imageUpload, IHttpContextAccessor httpContextAccessor)
        {
            this.mediaRepository = mediaRepository;
            this.unitOfWork = unitOfWork;
            this.imageUpload = imageUpload;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task AddAsync(Media media, IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file != null)
            {
                var request = httpContextAccessor.HttpContext?.Request
                   ?? throw new InvalidOperationException("No HTTP context");
                var baseUrl = $"{request.Scheme}://{request.Host}";

                var location = $"Images/MediaOfBooking/{media.Id}";

                var imagePath = await imageUpload.UploadImageAsync(file, location, cancellationToken);
                media.FileUrl = baseUrl + imagePath;
            }

            try
            {
                await mediaRepository.AddAsync(media, cancellationToken);
                await unitOfWork.SaveChangesAsync();

            }
            catch
            {

            }

        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var media = await mediaRepository.GetByIdAsync(id, cancellationToken);

            if (media is null)
                return;
            var fileurl = media.FileUrl;
            await mediaRepository.DeleteAsync(media, cancellationToken);

            await unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(fileurl))
            {
                await imageUpload.DeleteImage(fileurl, $"Images/MediaOfBooking/{media.Id}");
            }

        }

        public async Task<IEnumerable<Media>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return await mediaRepository
                         .GetTableNoTracking(cancellationToken)
                         .Where(x => x.BookingId == bookingId)
                         .ToListAsync(cancellationToken);
        }

        public async Task<Media?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await mediaRepository.GetByIdAsync(id, cancellationToken);

        }

        public async Task<IEnumerable<Media>> GetByTypeAsync(int bookingId, RepairMediaTypeEnum type, CancellationToken cancellationToken = default)
        {
            return await mediaRepository
                .GetTableNoTracking(cancellationToken)
                .Where(x =>
                    x.BookingId == bookingId &&
                    x.RepairMediaType == type)
                .ToListAsync(cancellationToken);
        }


        public async Task UpdateAsync(int Id, Media media, IFormFile file, CancellationToken cancellationToken = default)
        {
            var existingMedia =
                      await mediaRepository.GetByIdAsync(Id, cancellationToken);

            if (existingMedia is null)
                return;


            existingMedia.RepairMediaType = media.RepairMediaType;
            existingMedia.FileType = media.FileType;
            existingMedia.Description = media.Description;


            var request = httpContextAccessor.HttpContext?.Request
                ?? throw new InvalidOperationException("No HTTP context");

            var baseUrl = $"{request.Scheme}://{request.Host}";
            var location = $"Images/MediaOfBooking/{Id}";

            var imagePath = await imageUpload.UploadImageAsync(file, location, cancellationToken);

            var oldImageUrl = existingMedia.FileUrl;

            existingMedia.FileUrl = baseUrl + imagePath;

            if (!string.IsNullOrEmpty(oldImageUrl))
            {
                await imageUpload.DeleteImage(oldImageUrl, $"Images/MediaOfBooking/{Id}");
            }


            await mediaRepository.UpdateAsync(existingMedia, cancellationToken);

            await unitOfWork.SaveChangesAsync();
        }
    }
}
