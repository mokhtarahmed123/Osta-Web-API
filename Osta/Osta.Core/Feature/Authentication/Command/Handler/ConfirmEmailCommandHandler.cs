using Osta.Core.Bases;
using Osta.Core.Feature.Emails.Query.Model;
using Osta.Identity.Authentication;
using Osta.Identity.DTOs;

namespace Osta.Core.Feature.Authentication.Command.Handler
{
    public class ConfirmEmailCommandHandler : ResponseHandler
    {
        private readonly IAuthenticationService authentication;

        public ConfirmEmailCommandHandler(IAuthenticationService authentication)
        {

            this.authentication = authentication;

        }
        public async Task<Response<string>> Handle(ConfirmEmailQuery request, CancellationToken cancellationToken)
        {
            var confirm = await authentication.ConfirmEmail(
                request.UserId,
                request.Code);

            return confirm switch
            {
                ConfirmEmailResult.UserIdOrCodeNull =>
            BadRequest<string>("User ID or confirmation code is required."),

                ConfirmEmailResult.UserNotFound =>
                    NotFound<string>("User not found."),

                ConfirmEmailResult.Failed =>
                    BadRequest<string>("Email confirmation failed."),

                ConfirmEmailResult.Confirmed =>
                    Success<string>("Email confirmed successfully."),

                _ =>
                    BadRequest<string>("Invalid email confirmation request.")
            };
        }
    }
}
