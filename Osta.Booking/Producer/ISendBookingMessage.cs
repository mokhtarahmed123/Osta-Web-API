namespace Osta.Booking.Producer
{
    public interface ISendBookingMessage
    {
        public Task SendBooking<T>(T Message, string QueueName);
    }
}
