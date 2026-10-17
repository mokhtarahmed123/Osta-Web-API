using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Category.Query.Model;
using Osta.Core.Feature.Category.Query.Result;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;
using System.Diagnostics;

namespace Osta.Core.Feature.Category.Query.Handler
{
    public class GetAllCategoryQueryHandler : ResponseHandler, IRequestHandler<GetAllCategoryQuery, Response<List<GetAllCategoryResult>>>

    {
        private readonly IMapper mapper;
        private readonly ICategoryService categoryService;
        private readonly ILoggerService logger;

        public GetAllCategoryQueryHandler(IMapper mapper, ICategoryService categoryService, ILoggerService logger)
        {
            this.mapper = mapper;
            this.categoryService = categoryService;
            this.logger = logger;
        }

        public async Task<Response<List<GetAllCategoryResult>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {

            var sw = Stopwatch.StartNew();

            var categories = await categoryService.GetAllCategoriesAsync();

            sw.Stop();
            logger.LogInformation(
                "Handler took {Elapsed}  ms",
                sw.ElapsedMilliseconds);
            var result = mapper.Map<List<GetAllCategoryResult>>(categories);
            return Success(result, "Categories retrieved successfully.");
        }

    }
}
