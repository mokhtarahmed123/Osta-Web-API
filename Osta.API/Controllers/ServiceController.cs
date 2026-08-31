using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Service.Command.Model;
using Osta.Core.Feature.Service.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]


    public class ServiceController : AppBaseController
    {
        [HttpGet("Satrt")]
        public IActionResult Get()
        {
            return Ok("Service Controller is working!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]

        public async Task<IActionResult> AddService([FromForm] AddServiceCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var respone = await Mediator.Send(new GetAllServicesQuery());
            return NewResult(respone);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var query = new GetServiceByIdQuery(Id);
            var respone = await Mediator.Send(query);
            return NewResult(respone);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id, CancellationToken cancellationToken)
        {
            var command = new DeleteServiceCommand(Id);
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromForm] UpdateServiceCommand command, CancellationToken cancellationToken)
        {

            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }

    }
}
