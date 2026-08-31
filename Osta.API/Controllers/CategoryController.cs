using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Category.Command.Model;
using Osta.Core.Feature.Category.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    public class CategoryController : AppBaseController
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok(" Category API is running.");


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] AddCategoryCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery(id);
            var response = await Mediator.Send(query, cancellationToken);
            return NewResult(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
        {
            var query = new GetAllCategoryQuery();
            var response = await Mediator.Send(query, cancellationToken);
            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {

            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [HttpGet("Paginated")]

        public async Task<IActionResult> GetAllCategoriesPaginated([FromQuery] GetAllCategoriesPaginatedQuery Query)
        {


            var response = await Mediator.Send(Query);
            return Ok(response);

        }

    }
}
