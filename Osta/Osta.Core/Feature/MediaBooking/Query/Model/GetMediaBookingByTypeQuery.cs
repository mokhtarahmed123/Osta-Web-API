using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.MediaBooking.Query.Result;
using Osta.Domain.Enum;

namespace Osta.Core.Feature.MediaBooking.Query.Model
{
    public record GetMediaBookingByTypeQuery(int BookingId, RepairMediaTypeEnum repairType) : IRequest<Response<List<GetMediaBookingByTypeResult>>>;



}
