using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Query.Model;
using Osta.Core.Feature.Coupon.Query.Result;
using Osta.Payment.CouponService;

namespace Osta.Core.Feature.Coupon.Query.Handler
{
    public class GetCouponByIdQueryHandler : ResponseHandler, IRequestHandler<GetCouponByIdQuery, Response<GetCouponByIdResult>>
    {
        private readonly ICouponService _couponService;

        public GetCouponByIdQueryHandler(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<Response<GetCouponByIdResult>> Handle(GetCouponByIdQuery request, CancellationToken ct)
        {
            if (request.Id < 0) return BadRequest<GetCouponByIdResult>("Invalid coupon Id.");


            var coupon = await _couponService.GetByIdAsync(request.Id, ct);

            if (coupon is null)
                return NotFound<GetCouponByIdResult>("Coupon not found.");

            var result = new GetCouponByIdResult
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