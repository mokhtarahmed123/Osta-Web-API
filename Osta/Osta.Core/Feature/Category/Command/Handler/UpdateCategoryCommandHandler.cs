using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Category.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Category.Command.Handler
{
    public class UpdateCategoryCommandHandler : ResponseHandler, IRequestHandler<UpdateCategoryCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly ICategoryService categoryService;
        private readonly ILoggerService logger;

        public UpdateCategoryCommandHandler(IMapper mapper, ICategoryService categoryService, ILoggerService logger)

        {
            this.mapper = mapper;
            this.categoryService = categoryService;
            this.logger = logger;
        }
        public async Task<Response<string>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var Category = await categoryService.GetCategoryAsync(request.Id);
                if (Category is null)
                {
                    logger.LogWarning("Attempted to update non-existent category {CategoryId}", request.Id);

                    throw new KeyNotFoundException($"This Category With Id {request.Id} Not Found");
                }

                logger.LogInformation("Updating category with ID {CategoryId}", request.Id);

                var category = mapper.Map<Data.Entities.Services.Category>(request);

                await categoryService.UpdateCategoryAsync(request.Id, category, request.Image, cancellationToken);

                logger.LogInformation("Category with ID {CategoryId} updated successfully", request.Id);

                return Updated<string>("Category updated successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating category with ID {CategoryId}", request.Id);

                return BadRequest<string>("An error occurred while processing your request.");
            }
        }

    }
}
