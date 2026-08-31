using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Chat.Command.Model;
using Osta.Core.Feature.Chat.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]

    public class ChatController : AppBaseController
    {

        [HttpPost("send")]
        public async Task<IActionResult> Send(SendMessageCommand command)
            => NewResult(await Mediator.Send(command));


        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetMessages(int bookingId)
            => NewResult(await Mediator.Send(new GetBookingMessagesQuery(bookingId)));
    }
}
