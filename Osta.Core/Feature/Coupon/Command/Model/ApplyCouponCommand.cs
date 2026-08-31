using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Command.Result;

namespace Osta.Core.Feature.Coupon.Command.Model
{
    public record ApplyCouponCommand(
       string Code,
       string UserId,
       decimal OriginalAmount
   ) : IRequest<Response<ApplyCouponResult>>;
}
