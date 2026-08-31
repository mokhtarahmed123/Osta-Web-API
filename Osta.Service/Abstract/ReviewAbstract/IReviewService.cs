using Osta.Data.Entities;

namespace Osta.Service.Abstract.ReviewAbstract
{
    public interface IReviewService
    {
        Task Add(Review review, CancellationToken cancellationToken);
        Task Update(int Id, Review review, CancellationToken cancellationToken);

        Task Delete(int Id, CancellationToken cancellationToken);
        Task<IEnumerable<Review>> GetAllMyReviewAsUser(string userId, CancellationToken cancellationToken);
        Task<IEnumerable<Review>> GetAllMyReviewAsTechnician(string TechId, CancellationToken cancellationToken);
        Task<double> GetAllRatingWithTechnicianId(string TechId, CancellationToken cancellationToken);


        Task<IEnumerable<Review>> GetAll(CancellationToken cancellationToken);

        Task<Review> GetReview(int Id, CancellationToken cancellationToken);
        Task<Review?> GetByBookingId(
    int bookingId,
    CancellationToken cancellationToken);

    }
}
