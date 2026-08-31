using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.MediaBooking.Command.Model;
using Osta.Core.Feature.MediaBooking.Query.Model;
using Osta.Core.Feature.MediaBooking.Query.Result;
using Osta.Domain.Enum;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class MediaBookingController : AppBaseController
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Adds media to a booking", Description = "Adds media files and related information to a booking.")]
        [SwaggerResponse(201, "Media booking added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid media booking data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AddMedia([FromForm] AddMediaBookingCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);

        }
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]

        [SwaggerOperation(Summary = "Updates booking media", Description = "Updates media files and related information for an existing booking media record.")]
        [SwaggerResponse(200, "Media booking updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid media booking data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Media booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UpdateMedia(int id, [FromForm] UpdateMediaBookingCommand command, CancellationToken cancellationToken)
        {

            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Deletes booking media", Description = "Deletes an existing media booking record using its unique identifier.")]
        [SwaggerResponse(200, "Media booking deleted successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Media booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> DeleteMedia(int id, CancellationToken cancellationToken)
        {
            var command =
                new DeleteMediaBookingCommand(id);

            var response =
                await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpGet("booking/{bookingId:int}")]
        [SwaggerOperation(Summary = "Gets media by booking ID", Description = "Retrieves all media associated with a specific booking.")]
        [SwaggerResponse(200, "Booking media returned successfully", type: typeof(List<GetMediaBookingByBookingIdResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetByBookingId(
           int bookingId)
        {
            var query =
                new GetMediaBookingByBookingIdQuery(bookingId);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }

        [HttpGet("booking/{bookingId:int}/type/{repairType}")]
        [SwaggerOperation(Summary = "Gets booking media by type", Description = "Retrieves booking media filtered by the specified repair media type.")]
        [SwaggerResponse(200, "Booking media returned successfully", type: typeof(List<GetMediaBookingByTypeResult>))]
        [SwaggerResponse(400, "Invalid repair media type")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Booking or media not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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
        [SwaggerOperation(Summary = "Gets booking media by ID", Description = "Retrieves a specific media booking record using its unique identifier.")]
        [SwaggerResponse(200, "Media booking retrieved successfully", type: typeof(GetMediaBookingByIdResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Media booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetById(int id)
        {
            var query =
                new GetMediaBookingByIdQuery(id);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }
    }
}
