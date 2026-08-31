namespace Osta.Notification.Queue
{
    public interface ISendNotificationMessage
    {
        public Task SendNotification<T>(T notification, string queue);

    }
}
