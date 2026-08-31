namespace Osta.Core.Feature.MediaBooking.Query.Result
{
    public record GetMediaBookingByIdResult
    {
        public int BookingId { get; set; }
        public string File { get; set; }
        public string FileType { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string RepairMediaType { get; set; }

    }
}
