using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Emails.Command.Model
{
    public record SendEmailCommand(string Email, string Massege) : IRequest<Response<string>>;

}
