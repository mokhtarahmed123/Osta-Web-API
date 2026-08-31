using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.MediaBooking.Command.Model;
using Osta.Core.Feature.MediaBooking.Query.Model;
using Osta.Domain.Enum;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class MediaBookingController : AppBaseController
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddMedia([FromForm] AddMediaBookingCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);

        }
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMedia(int id, [FromForm] UpdateMediaBookingCommand command, CancellationToken cancellationToken)
        {

            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [HttpDelete("{id:int}")]

        public async Task<IActionResult> DeleteMedia(int id, CancellationToken cancellationToken)
        {
            var command =
                new DeleteMediaBookingCommand(id);

            var response =
                await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpGet("booking/{bookingId:int}")]
        public async Task<IActionResult> GetByBookingId(
           int bookingId)
        {
            var query =
                new GetMediaBookingByBookingIdQuery(bookingId);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }

        [HttpGet("booking/{bookingId:int}/type/{repairType}")]
        public async Task<IActionResult> GetByType(
            int bookingId,
            RepairMediaTypeEnum repairType)
        {
            var query =
                new GetMediaBookingByTypeQuery(
                    bookingId,
                    repairType);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query =
                new GetMediaBookingByIdQuery(id);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }
    }
}
