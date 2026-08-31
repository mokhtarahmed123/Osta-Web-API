using Osta.Data.Entities.Booking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Chat
{
    [Table("Messages", Schema = "Chat")]

    public class Message
    {
        [Key]
        public int Id { get; set; }


        public string SenderId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; }

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Bookings Booking { get; set; } = null!;




    }
}
