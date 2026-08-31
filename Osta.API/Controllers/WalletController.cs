using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Query.Model.Wallet;

namespace Osta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : AppBaseController
    {
        [Authorize(Roles = "Technicians")]

        [HttpGet("balance")]
        public async Task<IActionResult> GetMyBalance()
        {
            var response = await Mediator.Send(
                new GetMyBalanceQuery());

            return Ok(response);
        }
    }
}
