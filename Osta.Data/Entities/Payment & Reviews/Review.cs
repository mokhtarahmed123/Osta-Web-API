using Osta.Data.Entities.Booking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities
{
    [Table("Reviews", Schema = "Review")]

    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Bookings Booking { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}