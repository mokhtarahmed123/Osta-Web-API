namespace Osta.Notification.DTOs
{
    public class NotificationMessage
    {
        public required string RecipientId { get; set; }
        public required string RecipientEmail { get; set; }
        public required int BookingId { get; set; }
        public required string RecipientName { get; set; }
        public required string Message { get; set; }
    }
}
