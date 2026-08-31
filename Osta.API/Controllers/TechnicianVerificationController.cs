using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TechnicianVerificationController : AppBaseController
    {
        [HttpPatch("{id}/verify")]
        public async Task<IActionResult> Verify(string id)
        {
            var command = new VerifyTechnicianCommand(id);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [HttpPatch("{id}/reject")]
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
