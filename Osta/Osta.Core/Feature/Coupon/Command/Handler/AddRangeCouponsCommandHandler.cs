using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Entities.Customer;
using Osta.Payment.CouponService;

namespace Osta.Core.Feature.Coupon.Command.Handler
{
    public class AddRangeCouponsCommandHandler : ResponseHandler, IRequestHandler<AddRangeCouponsCommand, Response<List<string>>>
    {
        private readonly ICouponService couponService;

        public AddRangeCouponsCommandHandler(ICouponService couponService)
        {
            this.couponService = couponService;
        }
        public async Task<Response<List<string>>> Handle(AddRangeCouponsCommand request, CancellationToken cancellationToken)
        {
            var generatedCodes = new HashSet<string>();
            var coupons = new List<Coupons>();

            while (coupons.Count < request.Count)
            {
                var code = Guid.NewGuid().ToString("N")[..8].ToUpper();

                if (!generatedCodes.Add(code))
                    continue;

                coupons.Add(new Coupons
                {
                    Code = code,
                    DiscountType = request.DiscountType,
                    DiscountValue = request.DiscountValue,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    UsageLimit = request.UsageLimit,
                    IsActive = true
                });
            }

            await couponService.AddRangeAsync(coupons, cancellationToken);


            return Success(coupons.Select(c => c.Code).ToList());
        }
    }
}
