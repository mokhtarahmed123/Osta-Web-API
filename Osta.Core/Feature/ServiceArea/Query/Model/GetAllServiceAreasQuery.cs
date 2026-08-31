using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.ServiceArea.Query.Result;

namespace Osta.Core.Feature.ServiceArea.Query.Model
{
    public record GetAllServiceAreasQuery() : IRequest<Response<List<GetAllServiceAreasResult>>>;
}
