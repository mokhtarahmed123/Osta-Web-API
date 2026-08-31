using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Core.Feature.Coupon.Command.Result;
using Osta.Domain.Enum;
using Osta.Payment.CouponService;

namespace Osta.Core.Feature.Coupon.Command.Handler
{
    public class ApplyCouponCommandHandler
        : ResponseHandler, IRequestHandler<ApplyCouponCommand, Response<ApplyCouponResult>>
    {
        private readonly ICouponService _couponService;

        public ApplyCouponCommandHandler(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<Response<ApplyCouponResult>> Handle(ApplyCouponCommand request, CancellationToken ct)
        {
            var coupon = await _couponService.GetByCodeAsync(request.Code, ct);

            if (coupon is null)
                return NotFound<ApplyCouponResult>("Coupon not found.");

            if (!coupon.IsActive)
                return BadRequest<ApplyCouponResult>("Coupon is inactive.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (today < coupon.StartDate || today > coupon.EndDate)
                return BadRequest<ApplyCouponResult>("Coupon is not valid at this time.");

            if (coupon.UsedCount >= coupon.UsageLimit)
                return BadRequest<ApplyCouponResult>("Coupon usage limit reached.");

            var alreadyUsed = await _couponService.HasUserUsedCouponAsync(coupon.Id, request.UserId, ct);
            if (alreadyUsed)
                return BadRequest<ApplyCouponResult>("You have already used this coupon.");

            decimal discount = coupon.DiscountType switch
            {
                DiscountTypeEnum.Percentage => request.OriginalAmount * (coupon.DiscountValue / 100m),
                DiscountTypeEnum.FixedAmount => coupon.DiscountValue,
                _ => 0
            };


            discount = Math.Min(discount, request.OriginalAmount);
            var finalAmount = request.OriginalAmount - discount;

            var result = new ApplyCouponResult
            {
                CouponId = coupon.Id,
                OriginalAmount = request.OriginalAmount,
                DiscountApplied = discount,
                FinalAmount = finalAmount
            };

            return Success(result);
        }
    }
}