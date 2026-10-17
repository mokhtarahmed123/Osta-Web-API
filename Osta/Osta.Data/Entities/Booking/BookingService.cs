using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Services;

namespace Osta.Data.Entities
{

    public class BookingService
    {
        public int BookingId { get; set; }


        public Bookings Booking { get; set; } = null!;

        public int ServiceId { get; set; }


        public Service Service { get; set; } = null!;


        public decimal PriceAtBooking { get; set; }
    }
}