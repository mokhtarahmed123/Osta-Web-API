using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Query.Model.Wallet;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : AppBaseController
    {
        [Authorize(Roles = "Technicians")]

        [HttpGet("balance")]
        [SwaggerOperation(Summary = "Gets technician wallet balance", Description = "Retrieves the wallet balance of the currently authenticated technician.")]
        [SwaggerResponse(200, "Wallet balance retrieved successfully", type: typeof(decimal))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Wallet not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetMyBalance()
        {
            var response = await Mediator.Send(
                new GetMyBalanceQuery());

            return Ok(response);
        }
    }
}
