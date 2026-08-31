using Osta.Data.Entities.Booking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Appointment
{
    [Table("Appointments", Schema = "Appointment")]
    public class Appointment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime ScheduledStart { get; set; }

        public DateTime ScheduledEnd { get; set; }


        public bool ReminderSent { get; set; } = false;

        public bool IsApproved { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Foreign Key
        public int BookingId { get; set; }

        // Navigation Property
        [ForeignKey(nameof(BookingId))]
        public Bookings Booking { get; set; } = null!;
    }
}
