using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianService;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    public class TechnicianServiceController : AppBaseController
    {

        [HttpGet("services/{serviceId}")]
        public async Task<IActionResult> GetByServiceId(int serviceId)
        {
            var Query = new GetAllTechniciansWithServiceIdQuery(serviceId);
            var response = await Mediator.Send(Query);
            return NewResult(response);

        }
        [Authorize(Roles = "Technicians")]
        [HttpPost("services")]
        public async Task<IActionResult> AddRequestService(

            [FromQuery] TechnicianAddServiceCommand command)
        {


            var response = await Mediator.Send(command);

            return NewResult(response);
        }


    }
}
