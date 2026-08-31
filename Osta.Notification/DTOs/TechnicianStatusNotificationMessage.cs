namespace Osta.Notification.DTOs
{
    public class TechnicianStatusNotificationMessage
    {
        public required string Id { get; set; }
        public required string Email { get; set; }

        public required string StatusOfRequest { get; set; }

        public string? ReasonOfReject { get; set; }
        public required string Message { get; set; }

    }
}
