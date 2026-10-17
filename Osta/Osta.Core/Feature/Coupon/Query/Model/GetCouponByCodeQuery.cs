using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Query.Result;

namespace Osta.Core.Feature.Coupon.Query.Model
{
    public record GetCouponByCodeQuery(string Code) : IRequest<Response<GetCouponByCodeResult>>;

}
