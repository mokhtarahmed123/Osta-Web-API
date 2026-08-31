using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.MediaBooking.Query.Result;

namespace Osta.Core.Feature.MediaBooking.Query.Model
{
    public record GetMediaBookingByIdQuery(int Id) : IRequest<Response<GetMediaBookingByIdResult>>
;
}
