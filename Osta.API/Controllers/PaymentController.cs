using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Payment.Command;
using Osta.Core.Feature.Payment.Query;
using Stripe;

namespace Osta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController : AppBaseController
    {
        [Authorize]
        [HttpPost("create-intent")]
        public async Task<IActionResult> CreateIntent([FromBody] CreatePaymentIntentCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("webhook")]
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
        [HttpPost("refund")]
        public async Task<IActionResult> Refund([FromBody] RefundPaymentCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("my-payments")]
        public async Task<IActionResult> GetMyPayments()
        {
            var result = await Mediator.Send(new GetMyPaymentsQuery());
            return Ok(result);
        }

    }
}
