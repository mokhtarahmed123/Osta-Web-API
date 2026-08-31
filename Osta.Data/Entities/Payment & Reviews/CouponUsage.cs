using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Domain.Entities.Customer;
using System.ComponentModel.DataAnnotations;

namespace Osta.Domain.Entities.Payment___Reviews
{
    public class CouponUsage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CouponId { get; set; }
        public Coupons Coupon { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public int? BookingId { get; set; }
        public Bookings? Booking { get; set; }

        [Required]
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}
