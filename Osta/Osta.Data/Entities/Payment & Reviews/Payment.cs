using Osta.Data.Entities.Booking;
using Osta.Data.Enum;

namespace Osta.Data.Entities
{
    public class Payment
    {

        public int Id { get; set; }

        public int BookingId { get; set; }

        public Bookings bookings { get; set; } = null!;

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public PaymentMethod Method { get; set; }

        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
