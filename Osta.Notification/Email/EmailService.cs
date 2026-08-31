using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Osta.Data.Helper;
using Osta.Notification.DTOs;
using Osta.Notification.Interfaces;
using Osta.SharedKernel.Exceptions;
namespace Osta.Notification.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<string> SendEmailAsync(Emaildto emaildto)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(MailboxAddress.Parse(emaildto.Email));
                message.Subject = emaildto.reason ?? "Notification";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = emaildto.Massage,
                    TextBody = "Welcome to Osta"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.SmtpPort,
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _emailSettings.Username,
                    _emailSettings.Password
                );

                await client.SendAsync(message);

                await client.DisconnectAsync(true);

                return "Success";
            }
            catch (Exception ex)
            {
                throw new EmailSendFailedException($"Failed to send email: {ex.Message}");
            }
        }
    }
}
