using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Query.Model;
using Osta.Core.Feature.Coupon.Query.Result;
using Osta.Payment.CouponService;

namespace Osta.Core.Feature.Coupon.Query.Handler
{
    public class GetAllCouponQueryHandler
        : ResponseHandler, IRequestHandler<GetAllCouponQuery, Response<List<GetAllCouponResult>>>
    {
        private readonly ICouponService _couponService;

        public GetAllCouponQueryHandler(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<Response<List<GetAllCouponResult>>> Handle(GetAllCouponQuery request, CancellationToken ct)
        {
            var coupons = await _couponService.GetAllAsync(request.IsActive, ct);

            var result = coupons.Select(c => new GetAllCouponResult
            {
                Id = c.Id,
                Code = c.Code,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                UsageLimit = c.UsageLimit,
                UsedCount = c.UsedCount,
                IsActive = c.IsActive
            }).ToList();

            return Success(result);
        }
    }
}