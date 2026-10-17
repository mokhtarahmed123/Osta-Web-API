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
    public class GetCategoryByIdQueryHandler : ResponseHandler, IRequestHandler<GetCategoryByIdQuery, Response<GetCategoryByIdResult>>
    {
        private readonly IMapper mapper;
        private readonly ICategoryService categoryService;
        private readonly ILoggerService logger;

        public GetCategoryByIdQueryHandler(IMapper mapper, ICategoryService categoryService, ILoggerService logger)
        {
            this.mapper = mapper;
            this.categoryService = categoryService;
            this.logger = logger;
        }
        public async Task<Response<GetCategoryByIdResult>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sw = Stopwatch.StartNew();



                if (request.Id < 0)
                {
                    logger.LogError(" Invalid Id ,Id Must Be Greater Than 0", request.Id);
                    return BadRequest<GetCategoryByIdResult>();
                }
                var category = await categoryService.GetCategoryAsync(request.Id);
                if (category == null)
                {
                    logger.LogWarning("Attempted to retrieve non-existent category {CategoryId}", request.Id);

                    return NotFound<GetCategoryByIdResult>("Category not found.");
                }
                sw.Stop();
                logger.LogInformation(
                    "Handler took {Elapsed}  ms",
                    sw.ElapsedMilliseconds);
                var result = mapper.Map<GetCategoryByIdResult>(category);
                return Success(result, "Category retrieved successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the category with ID {CategoryId}.", request.Id);
                return BadRequest<GetCategoryByIdResult>("An error occurred while processing your request.");

            }
        }

    }
}
