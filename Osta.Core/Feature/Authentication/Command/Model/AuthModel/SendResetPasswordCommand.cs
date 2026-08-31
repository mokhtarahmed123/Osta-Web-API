using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authentication.Command.Model.AuthModel
{
    public record SendResetPasswordCommand(string Email) : IRequest<Response<string>>;

}
