using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Core.Feature.Complaint.Query.Model;
using Osta.Core.Feature.Complaint.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ComplaintController : AppBaseController
    {
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new complaint", Description = "Allows an authenticated user to submit a new complaint.")]
        [SwaggerResponse(201, "Complaint added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid complaint data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Add(
            [FromBody] AddComplaintCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Updates a complaint", Description = "Updates an existing complaint using its unique identifier.")]
        [SwaggerResponse(200, "Complaint updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid complaint data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Complaint not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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

        [SwaggerOperation(Summary = "Deletes a complaint", Description = "Deletes an existing complaint using its unique identifier.")]
        [SwaggerResponse(200, "Complaint deleted successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Complaint not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/status")]
        [SwaggerOperation(Summary = "Updates complaint status", Description = "Updates the status of an existing complaint.")]
        [SwaggerResponse(200, "Complaint status updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid complaint status")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Complaint not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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
        [SwaggerOperation(Summary = "Gets all complaints", Description = "Retrieves all complaints available in the system.")]
        [SwaggerResponse(200, "List of complaints returned successfully", type: typeof(List<GetAllComplaintResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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
        [Authorize(Roles = "User")]
        [HttpGet("my")]
        [SwaggerOperation(Summary = "Gets my complaints", Description = "Retrieves all complaints submitted by the authenticated user.")]
        [SwaggerResponse(200, "User complaints returned successfully", type: typeof(List<GetMyComplaintsAsUserResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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
        [SwaggerOperation(Summary = "Gets a complaint by ID", Description = "Retrieves a specific complaint using its unique identifier.")]
        [SwaggerResponse(200, "Complaint retrieved successfully", type: typeof(GetByIdResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Complaint not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]

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
        [SwaggerOperation(Summary = "Gets complaints by booking", Description = "Retrieves all complaints associated with a specific booking.")]
        [SwaggerResponse(200, "Booking complaints returned successfully", type: typeof(List<GetByBookingIdResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Booking not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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
