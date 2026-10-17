using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Service.Query.Result;

namespace Osta.Core.Feature.Service.Query.Model
{
    public record GetAllServicesQuery() : IRequest<Response<List<GetAllServiceResult>>>;

}
