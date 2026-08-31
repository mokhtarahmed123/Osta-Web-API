using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Emails.Query.Model;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authentication;
using Osta.Identity.DTOs;

namespace Osta.Core.Feature.Emails.Query.Handler
{
    public class EmailQueryHandler : ResponseHandler,
      IRequestHandler<ConfirmEmailQuery, Response<string>>
    {
        private readonly IAuthenticationService authentication;
        private readonly UserManager<User> userManager;

        public EmailQueryHandler(IAuthenticationService authentication, UserManager<User> userManager)
        {
            this.authentication = authentication;
            this.userManager = userManager;
        }
        public async Task<Response<string>> Handle(
            ConfirmEmailQuery request,
            CancellationToken cancellationToken)
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
