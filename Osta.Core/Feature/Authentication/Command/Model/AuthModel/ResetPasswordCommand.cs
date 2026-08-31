using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authentication.Command.Model.AuthModel
{
    public record ResetPasswordCommand(string Email, string Password, string ConfirmPassword) : IRequest<Response<string>>;

}
