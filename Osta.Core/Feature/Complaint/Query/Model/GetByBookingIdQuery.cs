using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Complaint.Query.Result;

namespace Osta.Core.Feature.Complaint.Query.Model
{
    public record GetByBookingIdQuery(int BookingId) : IRequest<Response<List<GetByBookingIdResult>>>;


}
