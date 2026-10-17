using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Entities.Customer;
using Osta.Payment.CouponService;

namespace Osta.Core.Feature.Coupon.Command.Handler
{
    public class UpdateCouponCommandHandler : ResponseHandler, IRequestHandler<UpdateCouponCommand, Response<string>>
    {
        private readonly ICouponService _couponService;
        public UpdateCouponCommandHandler(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<Response<string>> Handle(UpdateCouponCommand request, CancellationToken ct)
        {
            var coupon = new Coupons
            {
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UsageLimit = request.UsageLimit,
                IsActive = request.IsActive
            };

            await _couponService.UpdateAsync(request.Id, coupon, ct);


            return Success("Coupon updated successfully");
        }
    }
}
