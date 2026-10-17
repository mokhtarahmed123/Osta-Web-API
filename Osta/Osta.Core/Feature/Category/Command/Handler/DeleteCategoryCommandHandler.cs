using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Category.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Category.Command.Handler
{
    public class DeleteCategoryCommandHandler : ResponseHandler, IRequestHandler<DeleteCategoryCommand, Response<string>>

    {

        private readonly ICategoryService categoryService;
        private readonly ILoggerService logger;
        private readonly IServiceService serviceService;

        public DeleteCategoryCommandHandler(ICategoryService categoryService, ILoggerService logger, IServiceService serviceService)

        {

            this.categoryService = categoryService;
            this.logger = logger;
            this.serviceService = serviceService;
        }


        public async Task<Response<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var Category = await categoryService.GetCategoryAsync(request.Id, cancellationToken);
                if (Category is null)
                {
                    logger.LogError("Attempted to delete non-existent category {CategoryId}", request.Id);
                    throw new KeyNotFoundException($"This Category With Id {request.Id} Not Found");
                }

                // Check If Category Has Services or Not 
                var CategoryHasServices = await serviceService.DoesCategoryHaveServiceAsync(request.Id, cancellationToken);
                if (CategoryHasServices)
                {
                    logger.LogError($" Category With Id {request.Id} Has Services , So you can't Remove it  ");

                    return BadRequest<string>($" Category With Id {request.Id} Has Services , So you can't Remove it  ");


                }


                logger.LogInformation("Deleting category with ID {CategoryId}", request.Id);
                await categoryService.DeleteCategoryAsync(request.Id, cancellationToken);

                logger.LogInformation("Category with ID {CategoryId} deleted successfully", request.Id);
                return Deleted<string>("Category deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting category with ID {CategoryId}", request.Id);
                return BadRequest<string>("An error occurred while processing your request.");
            }
        }

    }
}
