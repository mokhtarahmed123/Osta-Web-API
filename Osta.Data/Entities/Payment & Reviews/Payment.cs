using Osta.Data.Entities.Booking;
using Osta.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities
{
    [Table("Payments", Schema = "Payment")]

    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Bookings bookings { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public PaymentMethod Method { get; set; }

        [MaxLength(200)]
        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
