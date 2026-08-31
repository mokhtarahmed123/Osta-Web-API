using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authentication.Query.Model.AuthModel
{
    public record ConfirmResetPasswordQuery(string Code, string Email) : IRequest<Response<string>>;

}
