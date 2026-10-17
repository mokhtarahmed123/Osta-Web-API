using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Service.Command.Model;
using Osta.Core.Feature.Service.Query.Model;
using Osta.Core.Feature.Service.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]


    public class ServiceController : AppBaseController
    {
        [HttpGet("Start")]
        [SwaggerOperation(Summary = "Checks Service API status", Description = "Returns a simple response to verify that the Service API is running.")]
        [SwaggerResponse(200, "Service API is running")]
        public IActionResult Get()
        {
            return Ok("Service Controller is working!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new service", Description = "Allows an administrator to create a new service.")]
        [SwaggerResponse(201, "Service added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid service data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]

        public async Task<IActionResult> AddService([FromForm] AddServiceCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }


        [HttpGet]
        [SwaggerOperation(Summary = "Gets all services", Description = "Retrieves all services available in the system.")]
        [SwaggerResponse(200, "List of services returned successfully", type: typeof(List<GetAllServiceResult>))]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAll()
        {
            var respone = await Mediator.Send(new GetAllServicesQuery());
            return NewResult(respone);
        }
        [HttpGet("{Id}")]
        [SwaggerOperation(Summary = "Gets a service by ID", Description = "Retrieves a specific service using its unique identifier.")]
        [SwaggerResponse(200, "Service retrieved successfully", type: typeof(GetServiceByIdResult))]
        [SwaggerResponse(404, "Service not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetById(int Id)
        {
            var query = new GetServiceByIdQuery(Id);
            var respone = await Mediator.Send(query);
            return NewResult(respone);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        [SwaggerOperation(Summary = "Deletes a service", Description = "Allows an administrator to delete an existing service.")]
        [SwaggerResponse(200, "Service deleted successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Service not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Delete(int Id, CancellationToken cancellationToken)
        {
            var command = new DeleteServiceCommand(Id);
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates a service", Description = "Allows an administrator to update an existing service.")]
        [SwaggerResponse(200, "Service updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid service data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Service not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UpdateService(int id, [FromForm] UpdateServiceCommand command, CancellationToken cancellationToken)
        {

            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }

    }
}
