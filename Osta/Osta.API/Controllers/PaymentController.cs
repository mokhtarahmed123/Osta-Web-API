using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Payment.Command;
using Osta.Core.Feature.Payment.Query;
using Osta.Payment.Model;
using Stripe;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController : AppBaseController
    {
        [Authorize]
        [HttpPost("create-intent")]

        [SwaggerOperation(Summary = "Creates a payment intent", Description = "Creates a Stripe payment intent for an authenticated user.")]
        [SwaggerResponse(200, "Payment intent created successfully", type: typeof(PaymentIntentResult))]
        [SwaggerResponse(400, "Invalid payment data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> CreateIntent([FromBody] CreatePaymentIntentCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("webhook")]
        [SwaggerOperation(Summary = "Handles Stripe webhook", Description = "Receives and processes Stripe webhook events.")]
        [SwaggerResponse(200, "Stripe webhook processed successfully")]
        [SwaggerResponse(400, "Invalid Stripe webhook signature or event")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            try
            {
                await Mediator.Send(new HandleStripeWebhookCommand(json, signature!));
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("refund")]
        [SwaggerOperation(Summary = "Refunds a payment", Description = "Processes a refund for an existing payment.")]
        [SwaggerResponse(200, "Payment refunded successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid refund request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Payment not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Refund([FromBody] RefundPaymentCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("my-payments")]
        [SwaggerOperation(Summary = "Gets my payments", Description = "Retrieves all payments belonging to the authenticated user.")]
        [SwaggerResponse(200, "Payments returned successfully", type: typeof(List<GetMyPaymentsResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetMyPayments()
        {
            var result = await Mediator.Send(new GetMyPaymentsQuery());
            return Ok(result);
        }

    }
}
