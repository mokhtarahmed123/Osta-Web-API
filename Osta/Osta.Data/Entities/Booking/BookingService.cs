using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Services;

namespace Osta.Data.Entities
{

    public class BookingService
    {
        public int BookingId { get; set; }
<<<<<<< HEAD


        public Bookings Booking { get; set; } = null!;

        public int ServiceId { get; set; }


        public Service Service { get; set; } = null!;


=======
        public Bookings Booking { get; set; } = null!;
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
>>>>>>> c73a0b6 (Initial commit)
        public decimal PriceAtBooking { get; set; }
    }
}