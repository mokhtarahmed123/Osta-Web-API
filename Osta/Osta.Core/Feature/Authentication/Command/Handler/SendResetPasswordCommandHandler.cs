using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Identity.Authentication;
using Osta.Identity.DTOs;

namespace Osta.Core.Feature.Authentication.Command.Handler
{
    public class SendResetPasswordCommandHandler : ResponseHandler, IRequestHandler<SendResetPasswordCommand, Response<string>>
    {
        private readonly IAuthenticationService authentication;

        public SendResetPasswordCommandHandler(IAuthenticationService authentication)
        {
            this.authentication = authentication;
        }

        public async Task<Response<string>> Handle(
            SendResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var result = await authentication.SendResetPasswordCode(
                request.Email);

            return result switch
            {
                SendResetPasswordCodeResult.InvalidInput =>
                    BadRequest<string>("Invalid email."),

                SendResetPasswordCodeResult.UserNotFound =>
                    NotFound<string>("User not found."),

                SendResetPasswordCodeResult.ErrorInUpdating =>
                    BadRequest<string>("Failed to update user."),

                SendResetPasswordCodeResult.FailedToSendEmail =>
                    BadRequest<string>("Failed to send reset password code."),

                SendResetPasswordCodeResult.Success =>
                    Success<string>(
                        "Reset password code sent successfully."),

                SendResetPasswordCodeResult.Failed =>
                    BadRequest<string>(
                        "Failed to send reset password code."),

                _ =>
                    BadRequest<string>(
                        "Failed to send reset password code.")
            };
        }

    }
}
