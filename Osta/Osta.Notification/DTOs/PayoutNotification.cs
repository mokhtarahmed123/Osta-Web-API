namespace Osta.Notification.DTOs
{
    public class PayoutNotification
    {
        public required string TechnicianId { get; set; } = null!;

        public required string Email { get; set; }
        public required int PayoutId { get; set; }
        public required decimal Amount { get; set; }
        public required string Method { get; set; }
        public required string ReceivingDetails { get; set; } = null!;
        public DateTime CompletedAt { get; set; }
        public string? ReasonOfRejection { get; set; } = null;

        public required string Message { get; set; } = null!;

    }
}
