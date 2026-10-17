using Osta.Data.Entities.Booking;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities
{
    [Table("Reviews", Schema = "Review")]

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