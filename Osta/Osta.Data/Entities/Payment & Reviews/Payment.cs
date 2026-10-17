using Osta.Data.Entities.Booking;
using Osta.Data.Enum;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities
{
    [Table("Payments", Schema = "Payment")]

=======

namespace Osta.Data.Entities
{
>>>>>>> c73a0b6 (Initial commit)
    public class Payment
    {

        public int Id { get; set; }

        public int BookingId { get; set; }

<<<<<<< HEAD

        public Bookings bookings { get; set; } = null!;


=======
        public Bookings bookings { get; set; } = null!;

>>>>>>> c73a0b6 (Initial commit)
        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public PaymentMethod Method { get; set; }

        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
