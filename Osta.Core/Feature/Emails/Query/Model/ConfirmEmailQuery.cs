using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Emails.Query.Model
{
    public record ConfirmEmailQuery(string Code, string UserId) : IRequest<Response<string>>;
}
