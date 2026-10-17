using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Payment.CouponService;

namespace Osta.Core.Feature.Coupon.Command.Handler
{
    public class DeleteCouponCommandHandler : ResponseHandler, IRequestHandler<DeleteCouponCommand, Response<string>>
    {
        private readonly ICouponService _couponService;
        public DeleteCouponCommandHandler(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<Response<string>> Handle(DeleteCouponCommand request, CancellationToken ct)
        {
            if (request.Id < 0)
                return BadRequest<string>("\"Invalid coupon Id.\"");

            var coupon = await _couponService.GetByIdAsync(request.Id, ct);

            if (coupon is null)
                return NotFound<string>("Coupon not found.");

            if (!coupon.IsActive)
                return BadRequest<string>("Coupon is already deactivated.");

            coupon.IsActive = false;

            await _couponService.UpdateAsync(request.Id, coupon, ct);
            return Success("Coupon deactivated successfully");
        }
    }
}