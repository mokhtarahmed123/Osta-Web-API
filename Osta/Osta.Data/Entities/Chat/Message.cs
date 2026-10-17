using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;

namespace Osta.Domain.Entities.Chat
{
    public class Message
    {

        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;

        public User Sender { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; }

        public int BookingId { get; set; }
        public Bookings Booking { get; set; } = null!;
    }
}
