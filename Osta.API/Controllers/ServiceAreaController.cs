using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.ServiceArea.Command.Model;
using Osta.Core.Feature.ServiceArea.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]

    public class ServiceAreaController : AppBaseController
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("Osta Service  Area API is running.");
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> AddServiceArea([FromBody] AddServiceAreaCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceAreaById(int id, CancellationToken cancellationToken)
        {
            var query = new GetServiceAreaByIdQuery(id);
            var response = await Mediator.Send(query, cancellationToken);
            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceArea(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteServiceAreaCommand(id);
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceArea(int id, [FromBody] UpdateServiceAreaCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServiceArea(CancellationToken cancellationToken)
        {
            var query = new GetAllServiceAreasQuery();
            var response = await Mediator.Send(query, cancellationToken);
            return NewResult(response);
        }

    }
}
