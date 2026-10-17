using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Entities.Customer;
using Osta.Payment.CouponService;

namespace Osta.Core.Feature.Coupon.Command.Handler
{
    public class AddCouponCommandHandler : ResponseHandler, IRequestHandler<AddCouponCommand, Response<string>>
    {
        private readonly ICouponService couponService;


        public AddCouponCommandHandler(ICouponService couponService)
        {
            this.couponService = couponService;

        }
        public async Task<Response<string>> Handle(AddCouponCommand request, CancellationToken cancellationToken)
        {
            var coupon = new Coupons
            {
                Code = string.IsNullOrWhiteSpace(request.Code)
               ? Guid.NewGuid().ToString("N")[..8].ToUpper()
                    : request.Code.ToUpper(),
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UsageLimit = request.UsageLimit,
                IsActive = true
            };

            await couponService.AddAsync(coupon, cancellationToken);
            return Success("Added Successfully");
        }
    }
}
