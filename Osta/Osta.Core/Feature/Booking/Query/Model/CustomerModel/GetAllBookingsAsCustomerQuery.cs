using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Booking.Query.Result;

namespace Osta.Core.Feature.Booking.Query.Model.CustomerModel
{
    public record GetAllBookingsAsCustomerQuery : IRequest<Response<List<GetAllBookingsAsCustomerResult>>>
  ;
}
