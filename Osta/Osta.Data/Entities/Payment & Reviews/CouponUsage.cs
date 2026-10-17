using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Domain.Entities.Customer;

namespace Osta.Domain.Entities.Payment___Reviews
{
    public class CouponUsage
    {

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
