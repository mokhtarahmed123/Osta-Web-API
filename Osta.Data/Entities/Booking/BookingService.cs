using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Services;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities
{
    [Table("BookingServices", Schema = "Booking")]
    public class BookingService
    {
        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Bookings Booking { get; set; } = null!;

        public int ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtBooking { get; set; }
    }
}