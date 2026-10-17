using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Appointment.Command.Model;
using Osta.Core.Feature.Appointment.Query.Model;
using Osta.Core.Feature.Appointment.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AppointmentController : AppBaseController
    {
        [HttpPost]
        [Authorize(Roles = "Technicians")]
        [SwaggerOperation(Summary = "Creates a new Appointment", Description = "Allows a technician to create a new appointment.")]
        [SwaggerResponse(201, "Appointment created successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]


        public async Task<IActionResult> AddAppointment(
    [FromBody] AddAppointmentCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }



        [Authorize(Roles = "Technicians")]

        [HttpPut("{appointmentId}")]


        [SwaggerOperation(Summary = "Updates an Appointment", Description = "Allows a technician to update an existing appointment.")]
        [SwaggerResponse(200, "Appointment updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Appointment not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UpdateAppointment(
            [FromRoute] string appointmentId,
            [FromBody] UpdateAppointmentCommand command)
        {
            command = command with
            {
                AppointmentId = appointmentId
            };

            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [Authorize(Roles = "Technicians,User")]
        [HttpGet("{appointmentId}")]

        [SwaggerOperation(Summary = "Gets an Appointment by ID", Description = "Retrieves a specific appointment using its unique identifier.")]
        [SwaggerResponse(200, "Appointment retrieved successfully", type: typeof(GetAppointmentbyIdResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Appointment not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAppointmentById(
    [FromRoute] string appointmentId)
        {
            var response = await Mediator.Send(
                new GetAppointmentbyIdQuery(appointmentId));

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians,User")]
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all Appointments", Description = "Retrieves all appointments available for the authenticated user.")]
        [SwaggerResponse(200, "List of appointments returned successfully", type: typeof(List<GetAllAppointmentsResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var response =
                await Mediator.Send(
                    new GetAllAppointmentsQuery());

            return NewResult(response);
        }


        [Authorize(Roles = "User")]
        [HttpPatch("{appointmentId}/approve")]
        [SwaggerOperation(Summary = "Approves an Appointment", Description = "Allows a user to approve a requested appointment.")]
        [SwaggerResponse(200, "Appointment approved successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid appointment ID")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Appointment not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]

        public async Task<IActionResult> ApproveAppointment([FromRoute] string appointmentId)
        {
            var response = await Mediator.Send(
                new ApproveAppointmentCommand(appointmentId));

            return NewResult(response);
        }
        [Authorize(Roles = "User")]
        [HttpPatch("{appointmentId}/reject")]
        [SwaggerOperation(Summary = "Rejects an Appointment", Description = "Allows a user to reject a requested appointment.")]
        [SwaggerResponse(200, "Appointment rejected successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Appointment not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> RejectAppointment([FromRoute] string appointmentId, [FromBody] RejectAppointmentCommand command)
        {
            command = command with { AppointmentId = appointmentId };
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}
