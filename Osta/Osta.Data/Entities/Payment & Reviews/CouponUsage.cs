using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Domain.Entities.Customer;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;
=======
>>>>>>> c73a0b6 (Initial commit)

namespace Osta.Domain.Entities.Payment___Reviews
{
    public class CouponUsage
    {
<<<<<<< HEAD
        [Key]
=======

>>>>>>> c73a0b6 (Initial commit)
        public int Id { get; set; }


        public int CouponId { get; set; }
        public Coupons Coupon { get; set; } = null!;


        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public int? BookingId { get; set; }
        public Bookings? Booking { get; set; }


        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}
