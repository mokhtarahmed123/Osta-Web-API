using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianServiceArea;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianServiceArea;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianServiceArea;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TechnicianServiceAreaController : AppBaseController
    {
        [Authorize(Roles = "Technicians")]
        [HttpPost("service-areas")]
        [SwaggerOperation(Summary = "Adds technician service area", Description = "Allows a technician to add a service area to their profile.")]
        [SwaggerResponse(201, "Technician service area added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid service area data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Service area or technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AddServiceAreas(

            [FromBody] AddTechnicianServiceAreaCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }


        [HttpGet("service-areas/{serviceAreaId:int}")]
        [SwaggerOperation(Summary = "Gets technicians by service area", Description = "Retrieves all technicians associated with the specified service area.")]
        [SwaggerResponse(200, "Technicians returned successfully", type: typeof(List<GetAllTechniciansWithServiceAreaIdResult>))]
        [SwaggerResponse(404, "Service area not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetByServiceAreaId(int serviceAreaId)
        {
            var Query = new GetAllTechniciansWithServiceAreaIdQuery(serviceAreaId);
            var response = await Mediator.Send(Query);
            return NewResult(response);

        }

        [Authorize(Roles = "Technicians")]
        [HttpPatch("service-areas")]
        [SwaggerOperation(Summary = "Updates technician service area", Description = "Allows a technician to update their associated service area.")]
        [SwaggerResponse(200, "Technician service area updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid service area data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Technician service area not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UpdateServiceAreas(

            [FromBody] UpdateTechnicianServiceAreaCommand command)
        {

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]
        [HttpDelete("service-areas")]
        [SwaggerOperation(Summary = "Deletes technician service area", Description = "Allows a technician to remove a service area from their profile.")]
        [SwaggerResponse(200, "Technician service area deleted successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid service area data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Technician service area not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> DeleteServiceAreas(

            [FromBody] DeleteTechnicianServiceAreaCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }

    }
}
