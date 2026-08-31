using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Booking.Command.Model.CustomerModel;
using Osta.Core.Feature.Booking.Command.Model.TechnicianModel;
using Osta.Core.Feature.Booking.Query.Model.CustomerModel;
using Osta.Core.Feature.Booking.Query.Model.TechnicianModel;
using Osta.Core.Feature.Booking.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : AppBaseController
    {

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new booking", Description = "Allows an authenticated customer to send a new booking request.")]
        [SwaggerResponse(201, "Booking created successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid booking data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Service or technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> SendBooking(AddBookingCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);

        }
        [Authorize(Roles = "Technicians")]
        [HttpGet("technician/bookings")]

        [SwaggerOperation(Summary = "Gets all technician bookings", Description = "Retrieves all bookings assigned to the authenticated technician.")]
        [SwaggerResponse(200, "List of technician bookings returned successfully", type: typeof(List<GetAllBookingsAsTechnicianResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]

        public async Task<IActionResult> GetBookingsAsTechnician()
        {
            var response = await Mediator.Send(
                new GetAllBookingsAsTechnicianQuery());

            return NewResult(response);
        }


        [Authorize(Roles = "Technicians")]

        [HttpGet("technician/bookings/{id:int}")]

        [SwaggerOperation(Summary = "Gets a technician booking by ID", Description = "Retrieves a specific booking assigned to the authenticated technician.")]
        [SwaggerResponse(200, "Booking retrieved successfully", type: typeof(GetBookingAsTechnicianResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetBookingAsTechnician(int id)
        {
            var response = await Mediator.Send(new GetBookingAsTechnicianQuery(id));

            return NewResult(response);
        }



        [Authorize(Roles = "Technicians")]

        [HttpPatch("technician/confirm/{bookingId:int}")]
        [SwaggerOperation(Summary = "Confirms a booking", Description = "Allows a technician to confirm a booking request.")]
        [SwaggerResponse(200, "Booking confirmed successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid booking request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            var response = await Mediator.Send(new ConfirmBookingCommand(bookingId));

            return NewResult(response);
        }


        [Authorize(Roles = "Technicians")]

        [HttpPatch("technician/Refuse/{bookingId:int}")]
        [SwaggerOperation(Summary = "Refuses a booking", Description = "Allows a technician to refuse a booking request.")]
        [SwaggerResponse(200, "Booking refused successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid booking request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> RefuseBooking(int bookingId)
        {
            var response = await Mediator.Send(new RejectBookingCommand(bookingId));

            return NewResult(response);
        }
        [Authorize(Roles = "User")]

        [HttpPatch("customer/Cancel/{bookingId:int}")]

        [SwaggerOperation(Summary = "Cancels a booking", Description = "Allows a customer to cancel their booking.")]
        [SwaggerResponse(200, "Booking cancelled successfully", type: typeof(string))]
        [SwaggerResponse(400, "Booking cannot be cancelled")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var response = await Mediator.Send(new CancelBookingCommand(bookingId));

            return NewResult(response);
        }

        [Authorize(Roles = "User")]
        [HttpGet("my-bookings")]
        [SwaggerOperation(Summary = "Gets customer bookings", Description = "Retrieves all bookings belonging to the authenticated customer.")]
        [SwaggerResponse(200, "List of customer bookings returned successfully", type: typeof(List<GetAllBookingsAsCustomerResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAllBookingsAsCustomer()
        {
            var response = await Mediator.Send(
                new GetAllBookingsAsCustomerQuery());

            return Ok(response);
        }
        [Authorize(Roles = "Technicians")]
        [HttpPatch("{id:int}/complete")]
        [SwaggerOperation(Summary = "Completes a booking", Description = "Allows a technician to mark a booking as completed.")]
        [SwaggerResponse(200, "Booking completed successfully", type: typeof(string))]
        [SwaggerResponse(400, "Booking cannot be completed")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]

        public async Task<IActionResult> CompleteBooking(int id)
        {
            var command = new CompleteBookingCommand(id);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
    }
}
