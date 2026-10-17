namespace Osta.Chat.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task JoinBookingRoom(int bookingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"booking-{bookingId}");
        }

        public async Task LeaveBookingRoom(int bookingId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"booking-{bookingId}");
        }
    }
}
