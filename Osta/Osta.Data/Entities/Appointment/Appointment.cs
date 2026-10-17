using Osta.Data.Entities.Booking;
using System.ComponentModel.DataAnnotations;

namespace Osta.Domain.Entities.Appointment
{

    public class Appointment
    {

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime ScheduledStart { get; set; }

        public DateTime ScheduledEnd { get; set; }


        public bool ReminderSent { get; set; } = false;

        public bool IsApproved { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }


        public int BookingId { get; set; }



        public Bookings Booking { get; set; } = null!;
    }
}
