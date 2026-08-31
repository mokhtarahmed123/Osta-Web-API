using MediatR;
using Osta.Core.Bases;
using Osta.Domain.Enum;

namespace Osta.Core.Feature.Coupon.Command.Model
{
    public record AddRangeCouponsCommand(
        int Count,
        DiscountTypeEnum DiscountType,
        decimal DiscountValue,
        DateOnly StartDate,
        DateOnly EndDate,
        int UsageLimit
    ) : IRequest<Response<List<string>>>;
}