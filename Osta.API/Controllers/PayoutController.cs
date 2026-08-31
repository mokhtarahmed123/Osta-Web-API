using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout;

namespace Osta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayoutController : AppBaseController
    {
        [Authorize(Roles = "Technicians")]
        [HttpPost("request")]
        public async Task<IActionResult> RequestPayout(
          [FromBody] RequestPayoutCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]

        // Technician → Cancel Payout
        [HttpPut("{payoutId}/cancel")]
        public async Task<IActionResult> CancelPayout(
            int payoutId)
        {
            var command = new CancelPayoutCommand(payoutId);


            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]
        // Admin → Reject Payout
        [HttpPut("{payoutId}/reject")]
        public async Task<IActionResult> RejectPayout(
            int payoutId,
            [FromBody] RejectPayoutCommand command)
        {
            var response = await Mediator.Send(command);

            return Ok(response);
        }
        [Authorize(Roles = "Admin")]

        // Admin → Complete Payout
        [HttpPut("{payoutId}/complete")]
        public async Task<IActionResult> CompletePayout(
            int payoutId)
        {
            var command = new CompletePayoutCommand(payoutId)
     ;
            var response = await Mediator.Send(command);

            return Ok(response);
        }
        [Authorize(Roles = "Technicians")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyPayouts()
        {
            var response = await Mediator.Send(
                new GetAllMyPayoutsQuery());

            return Ok(response);
        }

        // Admin - Get All Pending Payouts
        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPayouts()
        {
            var response = await Mediator.Send(
                new GetAllPendingPayoutQuery());

            return Ok(response);
        }

        // Technician - Get Payout By Id
        [Authorize(Roles = "Technicians")]

        [HttpGet("{payoutId}")]
        public async Task<IActionResult> GetPayoutById(
            int payoutId)
        {
            var response = await Mediator.Send(
                new GetPayoutByIdQuery(payoutId));

            return Ok(response);
        }


    }
}
