using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Chat.Model;
using Osta.Core.Feature.Chat.Command.Model;
using Osta.Core.Feature.Chat.Query.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]

    public class ChatController : AppBaseController
    {

        [HttpPost("send")]
        [SwaggerOperation(Summary = "Sends a chat message", Description = "Sends a message to another user within a booking conversation.")]
        [SwaggerResponse(200, "Message sent successfully", type: typeof(MessageModel))]
        [SwaggerResponse(400, "Invalid message data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Booking or recipient not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Send(SendMessageCommand command)
            => NewResult(await Mediator.Send(command));


        [HttpGet("booking/{bookingId}")]
        [SwaggerOperation(Summary = "Gets booking messages", Description = "Retrieves all chat messages associated with a specific booking.")]
        [SwaggerResponse(200, "Booking messages returned successfully", type: typeof(List<MessageModel>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetMessages(int bookingId)
            => NewResult(await Mediator.Send(new GetBookingMessagesQuery(bookingId)));
    }
}
