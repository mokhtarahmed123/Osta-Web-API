using MediatR;
using Osta.Core.Feature.Category.Query.Result;
using Osta.Core.Wrappers;

namespace Osta.Core.Feature.Category.Query.Model
{
    public record GetAllCategoriesPaginatedQuery : IRequest<PaginatedResult<GetAllCategoriesPaginatedResult>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
