namespace Osta.Core.Feature.Review.Query.Result
{
    public record GetAllReviewsResult
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public string TechId { get; set; }
        public string CustomerId { get; set; }


    }
}
