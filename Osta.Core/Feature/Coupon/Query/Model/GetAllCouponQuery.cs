using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Query.Result;

namespace Osta.Core.Feature.Coupon.Query.Model
{
    public record GetAllCouponQuery(bool IsActive) : IRequest<Response<List<GetAllCouponResult>>>
  ;
}
