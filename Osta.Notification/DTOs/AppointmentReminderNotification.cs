namespace Osta.Notification.DTOs
{
    public record AppointmentReminderNotification
    {
        public required string ToEmail { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}
