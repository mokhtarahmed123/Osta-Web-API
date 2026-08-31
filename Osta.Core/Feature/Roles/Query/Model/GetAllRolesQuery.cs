using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Roles.Query.Result;

namespace Osta.Core.Feature.Roles.Query.Model
{
    public record GetAllRolesQuery() : IRequest<Response<List<GetAllRolesResult>>>;

}
