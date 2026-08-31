using MediatR;
using Osta.Core.Feature.Appointment.Command.Model.AppointmentJobMdoel;

namespace Osta.API.Jobs
{
    public class AppointmentReminderJob
    {
        private readonly IMediator _mediator;

        public AppointmentReminderJob(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task RunAsync()
        {
            await _mediator.Send(new SendAppointmentRemindersCommand());
        }
    }
}
