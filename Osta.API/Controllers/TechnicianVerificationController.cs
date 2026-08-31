using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TechnicianVerificationController : AppBaseController
    {
        [HttpPatch("{id}/verify")]
        [SwaggerOperation(Summary = "Verifies a technician", Description = "Allows an administrator to verify a technician request.")]
        [SwaggerResponse(200, "Technician verified successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Verify(string id)
        {
            var command = new VerifyTechnicianCommand(id);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [HttpPatch("{id}/reject")]

        [SwaggerOperation(Summary = "Rejects a technician", Description = "Allows an administrator to reject a technician request.")]
        [SwaggerResponse(200, "Technician rejected successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid rejection data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Reject(
    string id,
    [FromBody] RejectTechnicianCommand command)
        {
            command = command with { TechId = id };

            var response = await Mediator.Send(command);

            return NewResult(response);
        }


    }
}
