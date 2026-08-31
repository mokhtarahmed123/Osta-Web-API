using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Query.Model;
using Osta.Core.Feature.Coupon.Query.Result;
using Osta.Payment.CouponService;

namespace Osta.Core.Feature.Coupon.Query.Handler
{
    public class GetCouponByCodeQueryHandler : ResponseHandler, IRequestHandler<GetCouponByCodeQuery, Response<GetCouponByCodeResult>>
    {
        private readonly ICouponService _couponService;
        public GetCouponByCodeQueryHandler(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<Response<GetCouponByCodeResult>> Handle(GetCouponByCodeQuery request, CancellationToken ct)
        {
            var coupon = await _couponService.GetByCodeAsync(request.Code, ct);

            if (coupon is null)
                return NotFound<GetCouponByCodeResult>("Coupon not found.");

            var result = new GetCouponByCodeResult
            {
                Id = coupon.Id,
                Code = coupon.Code,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount,
                IsActive = coupon.IsActive
            };

            return Success(result);
        }
    }
}
