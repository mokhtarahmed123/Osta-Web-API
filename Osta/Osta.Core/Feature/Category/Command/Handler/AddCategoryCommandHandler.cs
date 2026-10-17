using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Category.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Category.Command.Handler
{
    public class AddCategoryCommandHandler : ResponseHandler,
        IRequestHandler<AddCategoryCommand, Response<string>>

    {
        private readonly IMapper mapper;
        private readonly ICategoryService categoryService;
        private readonly ILoggerService logger;

        public AddCategoryCommandHandler(
            IMapper mapper,
            ICategoryService categoryService,
            ILoggerService logger
           )

        {
            this.mapper = mapper;
            this.categoryService = categoryService;
            this.logger = logger;
        }

        public async Task<Response<string>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {


                logger.LogInformation("Adding new category with name {CategoryName}", request.Name);

                var category = mapper.Map<Data.Entities.Services.Category>(request);

                await categoryService.AddCategoryAsync(category, request.Image, cancellationToken);

                logger.LogInformation("Category {CategoryId} created successfully", category.Id);

                return Created<string>("Category created successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding category with name {CategoryName}", request.Name);
                return BadRequest<string>("An error occurred while processing your request.");
            }
        }
    }
}
