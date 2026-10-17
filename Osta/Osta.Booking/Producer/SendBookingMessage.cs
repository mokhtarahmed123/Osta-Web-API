using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Osta.Booking.Producer
{
    public class SendBookingMessage : ISendBookingMessage
    {
        private readonly ConnectionFactory _factory;

        public SendBookingMessage()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "user",
                Password = "mypassword",
                VirtualHost = "/"
            };
        }

        public async Task SendBooking<T>(T message, string queueName)
        {
            await using var connection =
                await _factory.CreateConnectionAsync();

            await using var channel =
                await connection.CreateChannelAsync();

            const string exchangeName = "osta.booking";

            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "booking");

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: "booking",
                mandatory: false,
                body: body);
        }
    }
}
