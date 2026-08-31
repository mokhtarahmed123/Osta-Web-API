using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Identity.Authentication;
using Osta.Identity.DTOs;

namespace Osta.Core.Feature.Authentication.Command.Handler
{
    public class ResetPasswordCommandHandler : ResponseHandler, IRequestHandler<ResetPasswordCommand, Response<string>>
    {
        private readonly IAuthenticationService authentication;

        public ResetPasswordCommandHandler(IAuthenticationService authentication)
        {
            this.authentication = authentication;
        }

        public async Task<Response<string>> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest<string>("InvalidInput");
            }

            var password = request.Password;

            if (password.Length < 8 ||
                !password.Any(char.IsUpper) ||
                !password.Any(char.IsLower) ||
                !password.Any(char.IsDigit) ||
                (!password.Any(char.IsSymbol) &&
                 !password.Any(char.IsPunctuation)))
            {
                return BadRequest<string>("PasswordTooWeak");
            }

            var result = await authentication.ResetPasswordCode(
                request.Email,
                request.Password);

            return result switch
            {
                ResetPasswordResult.InvalidInput =>
                    BadRequest<string>("Invalid input."),

                ResetPasswordResult.UserNotFound =>
                    NotFound<string>("User not found."),

                ResetPasswordResult.InvalidPassword =>
                    BadRequest<string>("Invalid password."),

                ResetPasswordResult.Failed =>
                    BadRequest<string>("Failed to reset password."),

                ResetPasswordResult.Success =>
                    Success<string>(
                        "Password reset successfully. You can login now."),

                _ =>
                    BadRequest<string>("Failed to reset password.")
            };
        }



    }
}
