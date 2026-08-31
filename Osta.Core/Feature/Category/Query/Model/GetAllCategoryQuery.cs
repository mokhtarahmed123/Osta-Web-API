using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Category.Query.Result;

namespace Osta.Core.Feature.Category.Query.Model
{
    public record GetAllCategoryQuery() : IRequest<Response<List<GetAllCategoryResult>>>;


}
