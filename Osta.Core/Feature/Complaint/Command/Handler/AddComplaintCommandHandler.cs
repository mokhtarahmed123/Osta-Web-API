using AutoMapper;
using MediatR;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Data.Enum;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Complaint.Command.Handler
{
    public class AddComplaintCommandHandler : ResponseHandler, IRequestHandler<AddComplaintCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUserService;
        private readonly IComplaintService complaintService;
        private readonly IBookingService bookingService;


        public AddComplaintCommandHandler(IMapper mapper, ICurrentUserService currentUserService, IComplaintService complaintService,
                IBookingService bookingService)
        {
            this.mapper = mapper;
            this.currentUserService = currentUserService;
            this.complaintService = complaintService;
            this.bookingService = bookingService;

        }
        public async Task<Response<string>> Handle(
         AddComplaintCommand request,
    CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var customerId = currentUserService.UserId;

            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            // Get Booking
            var booking =
                await bookingService.GetBookingById(
                    request.BookingId, cancellationToken);

            if (booking is null)
                return NotFound<string>(
                    "Booking not found.");

            // Check ownership
            if (booking.CustomerId != customerId)
                return Unauthorized<string>(
                    "This booking does not belong to you.");

            // Complaint only for completed booking
            if (booking.Status != BookingStatus.Completed)
                return BadRequest<string>(
                    "You can create a complaint only for completed bookings.");

            // Check existing complaint
            var existingComplaints =
                await complaintService.GetByBookingId(
                    request.BookingId,
                    cancellationToken);

            if (existingComplaints.Any())
                return BadRequest<string>(
                    "You have already submitted a complaint for this booking.");


            var complaint =
                mapper.Map<Osta.Data.Entities.Administration.Complaint>(request);
            complaint.Status = ComplaintStatus.Open;

            await complaintService.Add(
                complaint,
                cancellationToken);

            return Success<string>(
                "Complaint submitted successfully.");
        }
    }
}
