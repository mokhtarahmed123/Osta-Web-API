using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Feature.Coupon.Command.Result;
using Osta.Core.HandlerMiddleware;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Domain.Enum;
using Osta.Payment.CouponService;
using Osta.Payment.Model;
using Osta.Payment.Services;
using Osta.SharedKernel.Exceptions;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Payment.Command
{
    public class CreatePaymentIntentCommandHandler
        : IRequestHandler<CreatePaymentIntentCommand, PaymentIntentResult>
    {
        private readonly IPaymentService paymentService;


        private readonly ICurrentUserService currentUserService;
        private readonly UserManager<User> userManager;
        private readonly ICouponService couponService;

        public CreatePaymentIntentCommandHandler(
            IPaymentService paymentService,
            ICurrentUserService currentUserService,
            UserManager<User> userManager,
            ICouponService couponService)
        {
            this.paymentService = paymentService;
            this.currentUserService = currentUserService;
            this.userManager = userManager;
            this.couponService = couponService;
        }

        public async Task<PaymentIntentResult> Handle(
            CreatePaymentIntentCommand request,
            CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            if (request.Amount <= 0)
                throw new BadRequestException(
                    "Amount must be greater than zero.");

            var finalAmount = request.Amount;

            // =========================
            // Validate & Apply Coupon
            // =========================
            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var couponResult = await ValidateAndCalculateCoupon(
                    request.CouponCode,
                    userId,
                    request.Amount,
                    cancellationToken);

                finalAmount = couponResult.FinalAmount;
            }

            // =========================
            // Check Existing Payment
            // =========================
            var existingPayment = await paymentService.GetByBookingIdAsync(request.BookingId);

            if (existingPayment is not null)
            {
                if (existingPayment.Status == PaymentStatus.Completed)
                    throw new BadRequestException("This booking has already been paid for.");

                if (existingPayment.Status == PaymentStatus.Pending)
                {
                    var result = await paymentService.UpdatePaymentIntentAmountAsync(
                        existingPayment.TransactionId, finalAmount, cancellationToken);

                    existingPayment.Amount = finalAmount;
                    await paymentService.UpdateAsync(existingPayment, cancellationToken);


                    return result;
                }

            }

            // =========================
            // Create Payment Intent
            // =========================
            var paymentIntent =
                await paymentService.CreatePaymentIntentAsync(
                    finalAmount,
                    "egp",
                    request.BookingId, request.CouponCode,
                    cancellationToken);

            // =========================
            // Create Payment
            // =========================
            var payment = new Osta.Data.Entities.Payment
            {
                BookingId = request.BookingId,
                Amount = finalAmount,
                Status = PaymentStatus.Pending,
                Method = PaymentMethod.Card,
                TransactionId = paymentIntent.PaymentIntentId,
                CreatedAt = DateTime.UtcNow
            };

            await paymentService.Add(payment, cancellationToken);

            return paymentIntent;
        }

        private async Task<ApplyCouponResult> ValidateAndCalculateCoupon(
            string couponCode,
            string userId,
            decimal amount,
            CancellationToken cancellationToken)
        {
            // =========================
            // Basic Validation
            // =========================
            if (string.IsNullOrWhiteSpace(couponCode))
                throw new BadRequestException(
                    "Coupon Code is required.");

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            if (amount <= 0)
                throw new BadRequestException(
                    "Amount must be greater than zero.");

            // =========================
            // Validate User
            // =========================
            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
                throw new NotFoundException(
                    "User Not Found.");

            // =========================
            // Get Coupon
            // =========================
            var coupon = await couponService.GetByCodeAsync(
                couponCode,
                cancellationToken);

            if (coupon is null)
                throw new NotFoundException(
                    "Coupon Not Found.");

            // =========================
            // Coupon Status
            // =========================
            if (!coupon.IsActive)
                throw new BadRequestException(
                    "Coupon is not active.");

            var today = DateOnly.FromDateTime(
                DateTime.UtcNow);

            if (today < coupon.StartDate)
                throw new BadRequestException(
                    "Coupon is not active yet.");

            if (today > coupon.EndDate)
                throw new BadRequestException(
                    "Coupon has expired.");

            // =========================
            // Global Usage Limit
            // =========================
            if (coupon.UsageLimit != 0 && coupon.UsedCount >= coupon.UsageLimit)
            {
                throw new BadRequestException(
                    "Coupon usage limit has been reached.");
            }

            // =========================
            // User Usage
            // =========================
            var userAlreadyUsed =
                await couponService.HasUserUsedCouponAsync(
                    coupon.Id,
                    userId,
                    cancellationToken);

            if (userAlreadyUsed)
            {
                throw new BadRequestException(
                    "You have already used this coupon.");
            }

            // =========================
            // Calculate Discount
            // =========================
            decimal discountApplied;

            switch (coupon.DiscountType)
            {
                case DiscountTypeEnum.Percentage:

                    if (coupon.DiscountValue <= 0 ||
                        coupon.DiscountValue > 100)
                    {
                        throw new BadRequestException(
                            "Invalid discount percentage.");
                    }

                    discountApplied =
                        amount *
                        (coupon.DiscountValue / 100m);

                    break;

                case DiscountTypeEnum.FixedAmount:

                    if (coupon.DiscountValue <= 0)
                    {
                        throw new BadRequestException(
                            "Invalid fixed discount amount.");
                    }

                    discountApplied =
                        coupon.DiscountValue;

                    break;

                default:

                    throw new BadRequestException(
                        "Invalid discount type.");
            }

            // Don't allow discount greater than amount
            discountApplied =
                Math.Min(discountApplied, amount);

            var finalAmount =
                amount - discountApplied;

            // =========================
            // Result
            // =========================
            return new ApplyCouponResult
            {
                CouponId = coupon.Id,
                OriginalAmount = amount,
                DiscountApplied = discountApplied,
                FinalAmount = finalAmount
            };
        }
    }
}