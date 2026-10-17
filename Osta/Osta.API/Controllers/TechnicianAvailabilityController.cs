using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianAvailabilities;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianAvailabilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/TechnicianAvailability")]
    [ApiController]

    public class TechnicianAvailabilityController : AppBaseController
    {
        [Authorize(Roles = "Technicians")]
        [HttpPost("Technician/availabilities")]
        [SwaggerOperation(Summary = "Adds technician availability", Description = "Allows a technician to add a new availability schedule.")]
        [SwaggerResponse(201, "Technician availability added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid availability data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AddAvailability(

            RequestTechnicianAvailabilityCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = "Technicians")]

        [HttpDelete("Technician/availabilities/{id:int}")]

        [SwaggerOperation(Summary = "Deletes technician availability", Description = "Allows a technician to delete an existing availability schedule.")]
        [SwaggerResponse(200, "Technician availability deleted successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Availability not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> DeleteAvailability(

            int id)
        {
            var command =
                new DeleteTechnicianAvailabilityCommand(
                    id
                        );

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize]
        [HttpGet("availabilities")]

        [SwaggerOperation(Summary = "Gets all technician availabilities", Description = "Retrieves all technician availability schedules.")]
        [SwaggerResponse(200, "Technician availabilities returned successfully", type: typeof(List<GetAllTechnicianAvailabilitiesResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAllAvailabilities()
        {
            var response =
                await Mediator.Send(
                    new GetAllTechnicianAvailabilitiesQuery());

            return NewResult(response);
        }

        [Authorize]
        [HttpGet("Technician/{technicianId}/availabilities")]

        [SwaggerOperation(Summary = "Gets technician availabilities", Description = "Retrieves all availability schedules for a specific technician.")]
        [SwaggerResponse(200, "Technician availabilities returned successfully", type: typeof(List<GetAllTechnicianAvailabilitiesByTechnicianIdResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetTechnicianAvailabilities(
            string technicianId)
        {
            var query =
                new GetAllTechnicianAvailabilitiesByTechnicianIdQuery(
                    technicianId);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }

        [Authorize]
        [HttpGet("availabilities/{id:int}")]

        [SwaggerOperation(Summary = "Gets availability by ID", Description = "Retrieves a specific technician availability schedule by its ID.")]
        [SwaggerResponse(200, "Availability retrieved successfully", type: typeof(GetTechnicianAvailabilityByIdResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Availability not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAvailabilityById(int id)
        {
            var response =
                await Mediator.Send(
                    new GetTechnicianAvailabilityByIdQuery(id));

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]

        [HttpPatch("Technician/availabilities/{id:int}")]
        [SwaggerOperation(Summary = "Updates technician availability", Description = "Allows a technician to update an existing availability schedule.")]
        [SwaggerResponse(200, "Technician availability updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid availability data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Availability not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UpdateAvailability(
            string technicianId,
            int id,
            [FromBody] UpdateTechnicianAvailabilityCommand command)
        {
            command = command with { Id = id, };
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
    }


}

