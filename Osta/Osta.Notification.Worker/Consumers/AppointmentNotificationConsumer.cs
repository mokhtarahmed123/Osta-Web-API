
using Osta.Notification.DTOs;
using Osta.Notification.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Osta.Notification.Worker.Consumers
{
    public class AppointmentNotificationConsumer : BackgroundService
    {
        private readonly ConnectionFactory _factory;

        private readonly IServiceScopeFactory scopeFactory;

        public AppointmentNotificationConsumer(IServiceScopeFactory scopeFactory)
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
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var connection =
          await _factory.CreateConnectionAsync(stoppingToken);

            await using var channel =
                await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: "appointment-email-notifications",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

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
                        JsonSerializer.Deserialize<AppointmentNotification>(message);

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
                queue: "appointment-email-notifications",
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            Console.WriteLine("Appointment Notification Consumer is waiting for messages...");


            try
            {
                await Task.Delay(
                    Timeout.Infinite,
                    stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error processing notification: {ex.Message}");

            }
        }

    }
}

