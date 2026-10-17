using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Category.Query.Model;
using Osta.Core.Feature.Category.Query.Result;
using Osta.Core.Wrappers;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Osta.Core.Feature.Category.Query.Handler
{
    public class GetAllCategoriesPaginatedQueryHandler : ResponseHandler,
        IRequestHandler<GetAllCategoriesPaginatedQuery, PaginatedResult<GetAllCategoriesPaginatedResult>>

    {

        private readonly ICategoryService categoryService;
        private readonly ILoggerService logger;

        public GetAllCategoriesPaginatedQueryHandler(ICategoryService categoryService, ILoggerService logger)

        {

            this.categoryService = categoryService;
            this.logger = logger;
        }

        public async Task<PaginatedResult<GetAllCategoriesPaginatedResult>> Handle(GetAllCategoriesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            Expression<Func<Osta.Data.Entities.Services.Category, GetAllCategoriesPaginatedResult>>
                expression = e => new GetAllCategoriesPaginatedResult(e.Id, e.Name, e.ImageUrl, e.IsActive);

            var queryable = categoryService.GetAllCategoriesQueryable();
            var paginatedList = await queryable.Select(expression).ToPaginatedListAsync((int)request.PageNumber, (int)request.PageSize);

            sw.Stop();
            logger.LogInformation(
                "Handler took {Elapsed}  ms",
                sw.ElapsedMilliseconds);

            return paginatedList;
        }
    }
}
