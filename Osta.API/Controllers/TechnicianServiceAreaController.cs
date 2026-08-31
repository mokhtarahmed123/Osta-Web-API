using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianServiceArea;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianServiceArea;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TechnicianServiceAreaController : AppBaseController
    {
        [Authorize(Roles = "Technicians")]
        [HttpPost("service-areas")]
        public async Task<IActionResult> AddServiceAreas(

            [FromBody] AddTechnicianServiceAreaCommand command)
        {


            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpGet("service-areas/{serviceAreaId:int}")]

        public async Task<IActionResult> GetByServiceAreaId(int serviceAreaId)
        {
            var Query = new GetAllTechniciansWithServiceAreaIdQuery(serviceAreaId);
            var response = await Mediator.Send(Query);
            return NewResult(response);

        }
        [Authorize(Roles = "Technicians")]
        [HttpPatch("service-areas")]
        public async Task<IActionResult> UpdateServiceAreas(

            [FromBody] UpdateTechnicianServiceAreaCommand command)
        {

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]
        [HttpDelete("service-areas")]
        public async Task<IActionResult> DeleteServiceAreas(

            [FromBody] DeleteTechnicianServiceAreaCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }

    }
}
