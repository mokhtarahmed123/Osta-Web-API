using MediatR;
using Osta.Core.Bases;
using Osta.Domain.Enum;

namespace Osta.Core.Feature.Coupon.Command.Model
{
    public record UpdateCouponCommand(
        int Id,
        DiscountTypeEnum DiscountType,
        decimal DiscountValue,
        DateOnly StartDate,
        DateOnly EndDate,
        int UsageLimit,
        bool IsActive
    ) : IRequest<Response<string>>;
}