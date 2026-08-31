namespace Osta.Core.Feature.Review.Query.Result
{
    public record GetAllMyReviewsAsUserResult
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

    }
}
