using Osta.SharedKernel.dto;

namespace Osta.Notification.Interfaces
{
    public interface IEmailService
    {
        public Task<string> SendEmailAsync(Emaildto emaildto);

    }
}
