namespace Osta.Chat.Model
{
    public class MessageModel
    {
        public required int Id { get; set; }
        public required int BookingId { get; set; }
        public required string SenderId { get; set; }
        public required string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }


    }
}
