namespace Osta.Notification.DTOs
{
    public class CustomerNotificationMessage
    {
        public required string CustomerId { get; set; }
        public required string CustomerEmail { get; set; }
        public required int BookingId { get; set; }
        public required string CustomerName { get; set; }
        public required string Message { get; set; }
    }
}
