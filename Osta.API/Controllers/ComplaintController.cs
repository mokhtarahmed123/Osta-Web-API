using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Core.Feature.Complaint.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ComplaintController : AppBaseController
    {
        [HttpPost]
        public async Task<IActionResult> Add(
            [FromBody] AddComplaintCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateComplaintCommand command)
        {
            command = command with
            {
                Id = id
            };

            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command =
                new DeleteComplaintCommand(id);

            var response =
                await Mediator.Send(command);

            return NewResult(response);
        }

        // =========================
        // Admin
        // =========================

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromQuery] UpdateStatusOfComplaintCommand command)
        {
            command = command with
            {
                Id = id
            };

            var response =
                await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query =
                new GetAllComplaintQuery();

            var response =
                await Mediator.Send(query);

            return NewResult(response);
        }

        // =========================
        // Customer
        // =========================

        [HttpGet("my")]
        public async Task<IActionResult> GetMyComplaints()
        {
            var query =
                new GetMyComplaintsAsUserQuery();

            var response =
                await Mediator.Send(query);

            return NewResult(response);
        }

        // =========================
        // Get by ID
        // =========================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query =
                new GetByIdQuery(id);

            var response =
                await Mediator.Send(query);

            return NewResult(response);
        }

        // =========================
        // Get by Booking
        // =========================

        [HttpGet("booking/{bookingId:int}")]
        public async Task<IActionResult> GetByBookingId(
            int bookingId)
        {
            var query =
                new GetByBookingIdQuery(bookingId);

            var response =
                await Mediator.Send(query);

            return NewResult(response);
        }
    }
}
