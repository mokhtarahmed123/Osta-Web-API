using Osta.Data.Entities.Booking;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;

namespace Osta.Domain.Entities.Appointment
{

    public class Appointment
    {

=======

namespace Osta.Domain.Entities.Appointment
{
    public class Appointment
    {
>>>>>>> c73a0b6 (Initial commit)
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime ScheduledStart { get; set; }

        public DateTime ScheduledEnd { get; set; }

<<<<<<< HEAD

        public bool ReminderSent { get; set; } = false;

        public bool IsApproved { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }


        public int BookingId { get; set; }



=======
        public bool ReminderSent { get; set; }

        public bool IsApproved { get; set; }

        public string? Notes { get; set; }

        public int BookingId { get; set; }
>>>>>>> c73a0b6 (Initial commit)
        public Bookings Booking { get; set; } = null!;
    }
}
