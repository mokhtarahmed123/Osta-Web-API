using MediatR;
using Osta.Core.Bases;
using Osta.Domain.Enum;

namespace Osta.Core.Feature.Coupon.Command.Model
{
    public record AddCouponCommand(string? Code,
        DiscountTypeEnum DiscountType,
        decimal DiscountValue,
        DateOnly StartDate,
        DateOnly EndDate,
        int UsageLimit) : IRequest<Response<string>>
    {
    }
}
