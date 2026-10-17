using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authorization.Query.Model
{
    public record GetUserRolesQuery(string UserId)
        : IRequest<Response<IList<string>>>;
}
