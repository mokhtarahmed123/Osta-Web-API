using AutoMapper;
using MediatR;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Appointment.Query.Model;
using Osta.Core.Feature.Appointment.Query.Result;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Appointment.Query.Handler
{
    public class GetAppointmentbyIdQueryHandler : ResponseHandler, IRequestHandler<GetAppointmentbyIdQuery, Response<GetAppointmentbyIdResult>>
    {
        private readonly IMapper mapper;
        private readonly IAppointmentService appointmentService;
        private readonly ICurrentUserService currentUserService;
        private readonly IBookingService bookingService;

        public GetAppointmentbyIdQueryHandler(IMapper mapper, IAppointmentService appointmentService,
            ICurrentUserService currentUserService, IBookingService bookingService)
        {
            this.mapper = mapper;
            this.appointmentService = appointmentService;

            this.currentUserService = currentUserService;
            this.bookingService = bookingService;
        }
        public async Task<Response<GetAppointmentbyIdResult>> Handle(
        GetAppointmentbyIdQuery request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Id))
                return BadRequest<GetAppointmentbyIdResult>(
                    "Appointment Id is required.");

            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var appointment =
                await appointmentService.Get(
                    request.Id,
                    cancellationToken);

            if (appointment is null)
                return NotFound<GetAppointmentbyIdResult>(
                    "Appointment not found.");

            var booking =
                await bookingService.GetBookingById(
                    appointment.BookingId);

            if (booking is null)
                return NotFound<GetAppointmentbyIdResult>(
                    "Booking not found.");


            if (booking.TechnicianId != userId &&
                booking.CustomerId != userId)
            {
                return Unauthorized<GetAppointmentbyIdResult>(
                    "You are not authorized to view this appointment.");
            }

            var result =
                mapper.Map<GetAppointmentbyIdResult>(appointment);

            return Success(result);
        }
    }
}
