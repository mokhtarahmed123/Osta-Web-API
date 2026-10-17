using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Booking.Query.Result;

namespace Osta.Core.Feature.Booking.Query.Model.TechnicianModel
{
    public record GetAllBookingsAsTechnicianQuery : IRequest<Response<List<GetAllBookingsAsTechnicianResult>>>
    {
    }
}
