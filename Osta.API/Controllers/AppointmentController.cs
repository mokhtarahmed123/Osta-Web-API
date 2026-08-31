using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Appointment.Command.Model;
using Osta.Core.Feature.Appointment.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AppointmentController : AppBaseController
    {
        [HttpPost]
        [Authorize(Roles = "Technicians")]
        public async Task<IActionResult> AddAppointment(
    [FromBody] AddAppointmentCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]

        [HttpPut("{appointmentId}")]
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
        public async Task<IActionResult> GetAppointmentById(
    [FromRoute] string appointmentId)
        {
            var response = await Mediator.Send(
                new GetAppointmentbyIdQuery(appointmentId));

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians,User")]
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var response =
                await Mediator.Send(
                    new GetAllAppointmentsQuery());

            return NewResult(response);
        }
        [Authorize(Roles = "User")]
        [HttpPatch("{appointmentId}/approve")]

        public async Task<IActionResult> ApproveAppointment([FromRoute] string appointmentId)
        {
            var response = await Mediator.Send(
                new ApproveAppointmentCommand(appointmentId));

            return NewResult(response);
        }
        [Authorize(Roles = "User")]
        [HttpPatch("{appointmentId}/reject")]
        public async Task<IActionResult> RejectAppointment([FromRoute] string appointmentId, [FromBody] RejectAppointmentCommand command)
        {
            command = command with { AppointmentId = appointmentId };
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}
