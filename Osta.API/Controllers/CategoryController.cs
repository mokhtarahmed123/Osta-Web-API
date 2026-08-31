using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Category.Command.Model;
using Osta.Core.Feature.Category.Query.Model;
using Osta.Core.Feature.Category.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    public class CategoryController : AppBaseController
    {
        [HttpGet("ping")]
        [SwaggerOperation(
            Summary = "Checks Category API status",
            Description = "Returns a simple message to verify that the Category API is running."
        )]
        [SwaggerResponse(
            200,
            "Category API is running",
            type: typeof(string))]
        public IActionResult Ping() => Ok(" Category API is running.");


        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Creates a new Category",
            Description = "This endpoint allows an administrator to create a new category and store it in the database."
        )]
        [SwaggerResponse(
            201,
            "Category Added Successfully",
            type: typeof(string))]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] AddCategoryCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Gets a Category by ID",
            Description = "Retrieves a specific category using its unique identifier."
        )]
        [SwaggerResponse(
            200,
            "Category retrieved successfully",
            type: typeof(GetCategoryByIdResult))]
        [SwaggerResponse(404, "Category not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetCategoryById(int id, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery(id);
            var response = await Mediator.Send(query, cancellationToken);
            return NewResult(response);
        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets all Categories",
            Description = "Retrieves all categories available in the system."
        )]
        [SwaggerResponse(
            200,
            "List of categories returned successfully",
            type: typeof(List<GetAllCategoryResult>))]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
        {
            var query = new GetAllCategoryQuery();
            var response = await Mediator.Send(query, cancellationToken);
            return NewResult(response);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]

        [SwaggerOperation(
            Summary = "Deletes a Category",
            Description = "This endpoint allows an administrator to delete an existing category by its unique identifier."
        )]
        [SwaggerResponse(
            200,
            "Category deleted successfully",
            type: typeof(string))]
        [SwaggerResponse(400, "Invalid category ID")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Category not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Updates a Category",
            Description = "This endpoint allows an administrator to update an existing category."
        )]
        [SwaggerResponse(
            200,
            "Category updated successfully",
            type: typeof(string))]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Category not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {

            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [HttpGet("Paginated")]
        [SwaggerOperation(
            Summary = "Gets paginated Categories",
            Description = "Retrieves categories using pagination with the provided query parameters."
        )]
        [SwaggerResponse(
            200,
            "Paginated categories returned successfully",
            type: typeof(GetAllCategoriesPaginatedResult))]
        [SwaggerResponse(400, "Invalid pagination parameters")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAllCategoriesPaginated([FromQuery] GetAllCategoriesPaginatedQuery Query)
        {


            var response = await Mediator.Send(Query);
            return Ok(response);

        }

    }
}
