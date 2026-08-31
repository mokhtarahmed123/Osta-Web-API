using MediatR;
using Osta.Booking.Interface;
using Osta.Data.Enum;
using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Payment.CouponService;
using Osta.Payment.Services;
using Osta.Service.Abstract.TechnicianAbstract;

namespace Osta.Core.Feature.Payment.Command
{
    public class HandleStripeWebhookCommandHandler
        : IRequestHandler<HandleStripeWebhookCommand>
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsageCouponsRepository _usageCouponsRepository;
        private readonly ICouponService _couponService;
        private readonly ITechnicianEarningService technicianEarningService;
        private readonly IBookingService bookingService;
        private readonly ITechnicianWalletService technicianWalletService;

        public HandleStripeWebhookCommandHandler(
            IPaymentService paymentService,
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork,
            IUsageCouponsRepository usageCouponsRepository,
            ICouponService couponService, ITechnicianEarningService technicianEarningService, IBookingService bookingService, ITechnicianWalletService technicianWalletService)
        {
            _paymentService = paymentService;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _usageCouponsRepository = usageCouponsRepository;
            _couponService = couponService;
            this.technicianEarningService = technicianEarningService;
            this.bookingService = bookingService;
            this.technicianWalletService = technicianWalletService;
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
                await _paymentRepository
                    .GetByTransactionIdAsync(transactionId);

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

                            await _usageCouponsRepository
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