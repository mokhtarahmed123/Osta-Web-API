using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Emails.Command.Model;
using Osta.Notification.DTOs;
using Osta.Notification.Interfaces;

namespace Osta.Core.Feature.Emails.Command.Handler
{
    public class EmailCommandHandler : ResponseHandler, IRequestHandler<SendEmailCommand, Response<string>>
    {
        private readonly IEmailService emailService;

        public EmailCommandHandler(IEmailService emailService)
        {
            this.emailService = emailService;
        }
        public async Task<Response<string>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            var EmailDto = new Emaildto(request.Email, request.Massege, null);
            var response = await emailService.SendEmailAsync(EmailDto);
            if (response == "Success")
                return Success<string>("Email sent successfully");
            return BadRequest<string>("Failed to send email");
        }
    }
}
