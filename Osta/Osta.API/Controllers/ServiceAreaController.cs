using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.ServiceArea.Command.Model;
using Osta.Core.Feature.ServiceArea.Query.Model;
using Osta.Core.Feature.ServiceArea.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class ServiceAreaController : AppBaseController
    {
        [HttpGet("ping")]
        [SwaggerOperation(Summary = "Checks Service Area API status", Description = "Returns a simple response to verify that the Service Area API is running.")]
        [SwaggerResponse(200, "Service Area API is running")]
        public IActionResult Ping() => Ok("Osta Service  Area API is running.");
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new service area", Description = "Allows an administrator to create a new service area.")]
        [SwaggerResponse(201, "Service area added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid service area data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AddServiceArea([FromBody] AddServiceAreaCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Gets a service area by ID", Description = "Retrieves a specific service area using its unique identifier.")]
        [SwaggerResponse(200, "Service area retrieved successfully", type: typeof(GetServiceAreaByIdResult))]
        [SwaggerResponse(404, "Service area not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetServiceAreaById(int id, CancellationToken cancellationToken)
        {
            var query = new GetServiceAreaByIdQuery(id);
            var response = await Mediator.Send(query, cancellationToken);
            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes a service area", Description = "Allows an administrator to delete an existing service area.")]
        [SwaggerResponse(200, "Service area deleted successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Service area not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> DeleteServiceArea(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteServiceAreaCommand(id);
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates a service area", Description = "Allows an administrator to update an existing service area.")]
        [SwaggerResponse(200, "Service area updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid service area data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Service area not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UpdateServiceArea(int id, [FromBody] UpdateServiceAreaCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Gets all service areas", Description = "Retrieves all service areas available in the system.")]
        [SwaggerResponse(200, "List of service areas returned successfully", type: typeof(List<GetAllServiceAreasResult>))]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAllServiceArea(CancellationToken cancellationToken)
        {
            var query = new GetAllServiceAreasQuery();
            var response = await Mediator.Send(query, cancellationToken);
            return NewResult(response);
        }

    }
}
