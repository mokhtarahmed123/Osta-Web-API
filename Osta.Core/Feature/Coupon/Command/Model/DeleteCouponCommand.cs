using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Coupon.Command.Model
{
    public record DeleteCouponCommand(int Id) : IRequest<Response<string>>;

}
