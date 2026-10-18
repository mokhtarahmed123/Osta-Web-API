using MediatR;
using Osta.Booking.Interface;
using Osta.Core.Abstract.TechnicianAbstract;
using Osta.Data.Enum;
using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Payment.CouponService;
using Osta.Payment.CouponUsageService;
using Osta.Payment.Services;

namespace Osta.Core.Feature.Payment.Command
{
    public class HandleStripeWebhookCommandHandler
        : IRequestHandler<HandleStripeWebhookCommand>
    {
        private readonly IPaymentService _paymentService;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ICouponService _couponService;
        private readonly ITechnicianEarningService technicianEarningService;
        private readonly IBookingService bookingService;
        private readonly ITechnicianWalletService technicianWalletService;
        private readonly ICouponUsageService couponUsageService;

        public HandleStripeWebhookCommandHandler(
            IPaymentService paymentService,

            IUnitOfWork unitOfWork,

            ICouponService couponService, ITechnicianEarningService technicianEarningService, IBookingService bookingService, ITechnicianWalletService technicianWalletService, ICouponUsageService couponUsageService)
        {
            _paymentService = paymentService;

            _unitOfWork = unitOfWork;
            _couponService = couponService;
            this.technicianEarningService = technicianEarningService;
            this.bookingService = bookingService;
            this.technicianWalletService = technicianWalletService;
            this.couponUsageService = couponUsageService;
        }

        public async Task Handle(
            HandleStripeWebhookCommand request,
            CancellationToken ct)
        {
            var stripeEvent =
                _paymentService.ConstructWebhookEvent(
                    request.Json,
                    request.Signature);

            Console.WriteLine(
                $"Stripe Event: {stripeEvent.Type}");

            string? transactionId = null;
            PaymentStatus? newStatus = null;

            Stripe.PaymentIntent? paymentIntent = null;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":

                    paymentIntent =
                        stripeEvent.Data.Object
                        as Stripe.PaymentIntent;

                    transactionId = paymentIntent?.Id;
                    newStatus = PaymentStatus.Completed;

                    break;

                case "payment_intent.payment_failed":

                    paymentIntent =
                        stripeEvent.Data.Object
                        as Stripe.PaymentIntent;

                    transactionId = paymentIntent?.Id;
                    newStatus = PaymentStatus.Failed;

                    break;
            }

            if (transactionId is null || newStatus is null)
            {
                Console.WriteLine(
                    $"Unhandled Stripe Event: {stripeEvent.Type}");

                return;
            }

            var payment =
                await _paymentService
                    .GetByTransactionIdAsync(transactionId, ct);

            var book = await bookingService.GetBookingById(payment.BookingId);
            if (book is null)
            {
                throw new Exception(
                    $"Booking with Id {payment.BookingId} was not found.");
            }
            Console.WriteLine(
                $"TransactionId: {transactionId}");

            Console.WriteLine(
                $"Payment Found: {payment != null}");

            if (payment is null)
                return;

            // =========================================
            // Update Payment Status
            // =========================================

            payment.Status = newStatus.Value;

            // =========================================
            // Payment Successfully Completed
            // =========================================

            if (newStatus == PaymentStatus.Completed &&
                paymentIntent is not null)
            {
                if (paymentIntent.Metadata.TryGetValue(
                        "CouponCode",
                        out var couponCode) &&
                    !string.IsNullOrWhiteSpace(couponCode))
                {


                    var coupon =
                        await _couponService.GetByCodeAsync(
                            couponCode,
                            ct);

                    if (coupon is not null)
                    {
                        // =====================================
                        // Prevent Duplicate Coupon Usage
                        // =====================================

                        var alreadyUsed =
                            await _couponService.HasUserUsedCouponAsync(
                                coupon.Id,
                                book.CustomerId,
                                ct);

                        if (!alreadyUsed)
                        {
                            // =================================
                            // Create Coupon Usage
                            // =================================

                            var usageCoupon =
                                new Osta.Domain.Entities
                                    .Payment___Reviews.CouponUsage
                                {
                                    CouponId = coupon.Id,
                                    BookingId = payment.BookingId,
                                    UsedAt = DateTime.UtcNow,
                                    UserId = book.CustomerId,
                                };

                            await couponUsageService
                                .AddAsync(usageCoupon, ct);

                            // =================================
                            // Increment Used Count
                            // =================================

                            coupon.UsedCount++;

                            // =================================
                            // Disable Coupon If Limit Reached
                            // =================================

                            if (coupon.UsageLimit != 0 &&
                                coupon.UsedCount >=
                                coupon.UsageLimit)
                            {
                                coupon.IsActive = false;
                            }

                            await _couponService.UpdateAsync(coupon.Id,
                                coupon,
                                ct);
                        }
                    }



                }
            }

            var netamount = payment.Amount - (payment.Amount * 15 / 100);
            var earning = new TechnicianEarning
            {
                BookingId = payment.BookingId,
                TechnicianId = book.TechnicianId,
                EarnedAt = DateTime.UtcNow,

                GrossAmount = payment.Amount,

                PlatformFee = payment.Amount * 15 / 100,

                NetAmount = netamount
            };

            await technicianEarningService.CreateEarningAsync(earning, ct);
            await technicianWalletService.AddAmountAsync(book.TechnicianId, netamount, ct);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}