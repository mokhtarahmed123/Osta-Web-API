using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Core.Feature.Technician.Query.Model.TechnicianModel;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]

    public class TechnicianController : AppBaseController
    {
        [Authorize]
        [HttpPost("Request")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddTechnician(
            [FromForm] AddTechnicianCommand addTechnicianCommand,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(addTechnicianCommand, cancellationToken);
            return NewResult(response);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(string Id)
        {
            var Query = new GetTechnicianByIdQuery(Id);
            var response = await Mediator.Send(Query);
            return NewResult(response);


        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Query = new GetAllTechniciansQuery();
            var response = await Mediator.Send(Query);
            return NewResult(response);
        }
        [HttpGet("Paginated/{pageNumber:int}/{pageSize:int}")]
        public async Task<IActionResult> Paginated(int pageNumber, int pageSize)
        {
            var query = new GetAllTechniciansPaginatedQuery(pageNumber, pageSize);

            var response = await Mediator.Send(query);

            return Ok(response);
        }
        [Authorize]

        [HttpGet("rate/{rate}")]
        public async Task<IActionResult> GetAllTechniciansWithRate(double rate)
        {
            var Query = new GetAllTechniciansWithRateQuery(rate);
            var response = await Mediator.Send(Query);
            return NewResult(response);


        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] GetAllTechniciansSearchQuery query)
        {
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            {
                var command = new DeleteTechnicianCommand(Id);
                var response = await Mediator.Send(command);
                return NewResult(response);

            }

        }
        [Authorize(Roles = "Technicians")]

        [HttpPatch]
        public async Task<IActionResult> Update(

    [FromForm] UpdateTechnicianCommand command)
        {

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]
        [HttpGet("My-Profile")]
        public async Task<IActionResult> MyProfile()
        {
            var response = await Mediator.Send(new GetMyProfileQuery());
            return NewResult(response);


        }






    }
}
