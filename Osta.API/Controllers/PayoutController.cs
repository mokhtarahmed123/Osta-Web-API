using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayoutController : AppBaseController
    {
        [Authorize(Roles = "Technicians")]
        [HttpPost("request")]
        [SwaggerOperation(Summary = "Requests a payout", Description = "Allows an authenticated technician to request a payout from their wallet.")]
        [SwaggerResponse(200, "Payout requested successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid payout request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> RequestPayout(
          [FromBody] RequestPayoutCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]

        // Technician → Cancel Payout
        [HttpPut("{payoutId}/cancel")]

        [SwaggerOperation(Summary = "Cancels a payout", Description = "Allows a technician to cancel their pending payout request.")]
        [SwaggerResponse(200, "Payout cancelled successfully", type: typeof(string))]
        [SwaggerResponse(400, "Payout cannot be cancelled")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Payout not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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

        [SwaggerOperation(Summary = "Rejects a payout", Description = "Allows an administrator to reject a pending payout request.")]
        [SwaggerResponse(200, "Payout rejected successfully", type: typeof(string))]
        [SwaggerResponse(400, "Payout cannot be rejected")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Payout not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> RejectPayout(
            int payoutId,
            [FromBody] RejectPayoutCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Admin")]

        // Admin → Complete Payout
        [HttpPut("{payoutId}/complete")]

        [SwaggerOperation(Summary = "Completes a payout", Description = "Allows an administrator to mark a pending payout as completed.")]
        [SwaggerResponse(200, "Payout completed successfully", type: typeof(string))]
        [SwaggerResponse(400, "Payout cannot be completed")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Payout not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> CompletePayout(
            int payoutId)
        {
            var command = new CompletePayoutCommand(payoutId)
     ;
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]
        [HttpGet("my")]

        [SwaggerOperation(Summary = "Gets my payouts", Description = "Retrieves all payout requests belonging to the authenticated technician.")]
        [SwaggerResponse(200, "Technician payouts returned successfully", type: typeof(List<GetAllMyPayoutsResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetMyPayouts()
        {
            var response = await Mediator.Send(
                new GetAllMyPayoutsQuery());

            return NewResult(response);
        }

        // Admin - Get All Pending Payouts
        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        [SwaggerOperation(Summary = "Gets pending payouts", Description = "Retrieves all pending payout requests for administrators.")]
        [SwaggerResponse(200, "Pending payouts returned successfully", type: typeof(List<GetAllPendingPayoutResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetPendingPayouts()
        {
            var response = await Mediator.Send(
                new GetAllPendingPayoutQuery());

            return NewResult(response);
        }

        // Technician - Get Payout By Id
        [Authorize(Roles = "Technicians")]

        [HttpGet("{payoutId}")]

        [SwaggerOperation(Summary = "Gets a payout by ID", Description = "Retrieves a specific payout request by its unique identifier.")]
        [SwaggerResponse(200, "Payout retrieved successfully", type: typeof(GetPayoutByIdResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Payout not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetPayoutById(
            int payoutId)
        {
            var response = await Mediator.Send(
                new GetPayoutByIdQuery(payoutId));

            return NewResult(response);
        }


    }
}
