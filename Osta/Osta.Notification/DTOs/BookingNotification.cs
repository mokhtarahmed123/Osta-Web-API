namespace Osta.Notification.DTOs
{
    public record BookingNotification
    {
        public int BookingId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public string TechnicianId { get; set; } = null!;
        public string Status { get; set; } = null!;

    }
}
