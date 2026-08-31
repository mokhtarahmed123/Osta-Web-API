using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Booking.Command.Model.CustomerModel;
using Osta.Core.Feature.Booking.Command.Model.TechnicianModel;
using Osta.Core.Feature.Booking.Query.Model.CustomerModel;
using Osta.Core.Feature.Booking.Query.Model.TechnicianModel;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : AppBaseController
    {

        [HttpPost]
        public async Task<IActionResult> SendBooking(AddBookingCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);

        }
        [Authorize(Roles = "Technicians")]

        [HttpGet("technician/bookings")]
        public async Task<IActionResult> GetBookingsAsTechnician()
        {
            var response = await Mediator.Send(
                new GetAllBookingsAsTechnicianQuery());

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]

        [HttpGet("technician/bookings/{id:int}")]
        public async Task<IActionResult> GetBookingAsTechnician(int id)
        {
            var response = await Mediator.Send(new GetBookingAsTechnicianQuery(id));

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]

        [HttpPatch("technician/confirm/{bookingId:int}")]
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            var response = await Mediator.Send(new ConfirmBookingCommand(bookingId));

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]

        [HttpPatch("technician/Refuse/{bookingId:int}")]
        public async Task<IActionResult> RefuseBooking(int bookingId)
        {
            var response = await Mediator.Send(new RejectBookingCommand(bookingId));

            return NewResult(response);
        }
        [Authorize(Roles = "User")]

        [HttpPatch("customer/Cancel/{bookingId:int}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var response = await Mediator.Send(new CancelBookingCommand(bookingId));

            return NewResult(response);
        }
        [Authorize(Roles = "User")]
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetAllBookingsAsCustomer()
        {
            var response = await Mediator.Send(
                new GetAllBookingsAsCustomerQuery());

            return Ok(response);
        }
        [Authorize(Roles = "Technicians")]
        [HttpPatch("{id:int}/complete")]
        public async Task<IActionResult> CompleteBooking(int id)
        {
            var command = new CompleteBookingCommand(id);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
    }
}
