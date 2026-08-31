using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianService;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianService;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    public class TechnicianServiceController : AppBaseController
    {

        [HttpGet("services/{serviceId}")]
        [SwaggerOperation(Summary = "Gets technicians by service", Description = "Retrieves all technicians associated with the specified service.")]
        [SwaggerResponse(200, "Technicians returned successfully", type: typeof(List<GetAllTechniciansWithServiceIdResult>))]
        [SwaggerResponse(404, "Service not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetByServiceId(int serviceId)
        {
            var Query = new GetAllTechniciansWithServiceIdQuery(serviceId);
            var response = await Mediator.Send(Query);
            return NewResult(response);

        }
        [Authorize(Roles = "Technicians")]
        [HttpPost("services")]
        [SwaggerOperation(Summary = "Adds a service to technician", Description = "Allows an authenticated technician to request or add a service to their profile.")]
        [SwaggerResponse(201, "Technician service added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid service data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Service or technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AddRequestService(

            [FromQuery] TechnicianAddServiceCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }


    }
}
