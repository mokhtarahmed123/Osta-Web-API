using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Appointment.Query.Result;

namespace Osta.Core.Feature.Appointment.Query.Model
{
    public record GetAppointmentbyIdQuery(string Id) : IRequest<Response<GetAppointmentbyIdResult>>;
}
