using Osta.Data.Entities.Booking;

namespace Osta.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public Bookings Booking { get; set; } = null!;


        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}