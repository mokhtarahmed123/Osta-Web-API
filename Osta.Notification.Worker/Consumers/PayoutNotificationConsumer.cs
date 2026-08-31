
using Osta.Notification.DTOs;
using Osta.Notification.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Osta.Notification.Worker.Consumers
{
    public class PayoutNotificationConsumer : BackgroundService
    {
        private readonly ConnectionFactory _factory;

        private readonly IServiceScopeFactory scopeFactory;

        public PayoutNotificationConsumer(IServiceScopeFactory scopeFactory)
        {

            this.scopeFactory = scopeFactory;
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "user",
                Password = "mypassword",
                VirtualHost = "/"
            };

        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await using var connection =
      await _factory.CreateConnectionAsync(cancellationToken);

            await using var channel =
                await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: "payout-notification",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                try
                {
                    var body = eventArgs.Body.ToArray();

                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine(
                        $"Received Notification: {message}");

                    var notification =
                        JsonSerializer.Deserialize<PayoutNotification>(message);

                    if (notification is null)
                        throw new Exception("Invalid notification message.");

                    var email = new Emaildto(notification.Email, notification.Message, "OSTA Notification");
                    using var scope = scopeFactory.CreateScope();

                    var emailService =
               scope.ServiceProvider.GetRequiredService<IEmailService>();

                    await emailService.SendEmailAsync(email);

                    await channel.BasicAckAsync(
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Error processing notification: {ex.Message}");

                    await channel.BasicNackAsync(
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: "Notification",
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);

            Console.WriteLine(" Payout Notification Consumer is waiting for messages...");


            try
            {
                await Task.Delay(
                    Timeout.Infinite,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error processing notification: {ex.Message}");

            }
        }

    }
}
