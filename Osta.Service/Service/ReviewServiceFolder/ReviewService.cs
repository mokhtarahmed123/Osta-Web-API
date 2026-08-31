using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities;
using Osta.Infrastructure.Abstract.ReviewAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Service.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IReviewRepository reviewRepository;
        private readonly ILoggerService loggerService;

        public ReviewService(IUnitOfWork unitOfWork, IReviewRepository reviewRepository, ILoggerService loggerService)
        {
            this.unitOfWork = unitOfWork;
            this.reviewRepository = reviewRepository;
            this.loggerService = loggerService;
        }
        public async Task Add(Review review, CancellationToken cancellationToken)
        {
            await reviewRepository.AddAsync(review, cancellationToken);
            await unitOfWork.SaveChangesAsync();
            loggerService.LogInformation(
                          $"Review added successfully for Booking Id {review.BookingId}.");
        }

        public async Task Delete(int Id, CancellationToken cancellationToken)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var review = await reviewRepository.GetByIdAsync(Id, cancellationToken);
                if (review == null) return;
                await reviewRepository.DeleteAsync(review, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                loggerService.LogInformation(
                    $"Review Id {Id} deleted successfully.");

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                loggerService.LogError(
                                  $"Error deleting Review Id {Id}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Review>> GetAll(CancellationToken cancellationToken)
        {
            return await reviewRepository.GetTableNoTracking(cancellationToken).Include(x => x.Booking).ToListAsync();

        }

        public async Task<IEnumerable<Review>> GetAllMyReviewAsTechnician(string TechId, CancellationToken cancellationToken)
        {
            return await reviewRepository.GetTableNoTracking(cancellationToken)
                .Include(a => a.Booking).
                Where(a => a.Booking.TechnicianId == TechId).
                ToListAsync();


        }

        public async Task<IEnumerable<Review>> GetAllMyReviewAsUser(string userId, CancellationToken cancellationToken)
        {
            return await reviewRepository.GetTableNoTracking(cancellationToken)
                .Include(a => a.Booking).
                Where(a => a.Booking.CustomerId == userId).
                ToListAsync();

        }

        public async Task<double> GetAllRatingWithTechnicianId(string TechId, CancellationToken cancellationToken)
        {

            return await reviewRepository
                   .GetTableNoTracking(cancellationToken)
                   .Where(x => x.Booking.TechnicianId == TechId)
                   .Select(x => (double?)x.Rating)
                   .AverageAsync(cancellationToken) ?? 0;
        }

        public async Task<Review?> GetByBookingId(int bookingId, CancellationToken cancellationToken)
        {
            return await reviewRepository.FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);

        }

        public async Task<Review?> GetReview(int Id, CancellationToken cancellationToken)
        {
            return await reviewRepository.GetByIdAsync(Id, cancellationToken);
        }

        public async Task Update(int Id, Review NewReview, CancellationToken cancellationToken)
        {
            var OldReview = await reviewRepository.GetByIdAsync(Id, cancellationToken);
            if (OldReview == null) return;

            OldReview.Rating = NewReview.Rating;
            OldReview.Comment = NewReview.Comment;
            OldReview.BookingId = NewReview.BookingId;
            await reviewRepository.UpdateAsync(OldReview, cancellationToken);
            await unitOfWork.SaveChangesAsync();
            loggerService.LogInformation(
                       $"Review Id {Id} updated successfully.");
        }
    }
}
