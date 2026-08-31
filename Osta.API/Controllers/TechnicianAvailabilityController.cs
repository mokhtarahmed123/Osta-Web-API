using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianAvailabilities;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/TechnicianAvailability")]
    [ApiController]

    public class TechnicianAvailabilityController : AppBaseController
    {
        [Authorize(Roles = "Technicians")]
        [HttpPost("Technician/availabilities")]
        public async Task<IActionResult> AddAvailability(

            RequestTechnicianAvailabilityCommand command)
        {

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]

        [HttpDelete("Technician/availabilities/{id:int}")]
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
        public async Task<IActionResult> GetAllAvailabilities()
        {
            var response =
                await Mediator.Send(
                    new GetAllTechnicianAvailabilitiesQuery());

            return NewResult(response);
        }
        [Authorize]
        [HttpGet("Technician/{technicianId}/availabilities")]
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
        public async Task<IActionResult> GetAvailabilityById(int id)
        {
            var response =
                await Mediator.Send(
                    new GetTechnicianAvailabilityByIdQuery(id));

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]

        [HttpPatch("Technician/availabilities/{id:int}")]
        public async Task<IActionResult> UpdateAvailability(
            string technicianId,
            int id,
            [FromBody] UpdateTechnicianAvailabilityCommand command)
        {
            command = command with
            {
                Id = id,

            };

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
    }


}

