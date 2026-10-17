using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Query.Result;

namespace Osta.Core.Feature.Coupon.Query.Model
{
    public record GetCouponByIdQuery(int Id) : IRequest<Response<GetCouponByIdResult>>
 ;
}
