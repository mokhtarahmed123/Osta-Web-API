using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Osta.Notification.Queue
{
    public class SendNotificationMessage : ISendNotificationMessage
    {
        private readonly ConnectionFactory _factory;

        public SendNotificationMessage()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "user",
                Password = "mypassword",
                VirtualHost = "/"
            };
        }

        public async Task SendNotification<T>(T notification, string queue)
        {
            await using var connection =
                await _factory.CreateConnectionAsync();

            await using var channel =
                await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonSerializer.Serialize(notification);

            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                body: body);
        }
    }
}
