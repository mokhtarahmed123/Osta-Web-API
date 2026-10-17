namespace Osta.Notification.DTOs
{
    public class AppointmentNotification
    {
        public int BookingId { get; set; }
        public string CustomerId { get; set; } = null!;
        public string TechnicianId { get; set; } = null!;
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public string? Notes { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}
