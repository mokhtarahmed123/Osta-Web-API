using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Appointment.Query.Model;
using Osta.Core.Feature.Appointment.Query.Result;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Appointment.Query.Handler
{
    public class GetAllAppointmentsQueryHandler : ResponseHandler, IRequestHandler<GetAllAppointmentsQuery, Response<List<GetAllAppointmentsResult>>>
    {
        private readonly IMapper mapper;
        private readonly IAppointmentService appointmentService;

        private readonly ICurrentUserService currentUserService;


        public GetAllAppointmentsQueryHandler(IMapper mapper, IAppointmentService appointmentService, UserManager<User> userManager,
            ICurrentUserService currentUserService, IBookingService bookingService)
        {
            this.mapper = mapper;
            this.appointmentService = appointmentService;
            this.currentUserService = currentUserService;

        }

        public async Task<Response<List<GetAllAppointmentsResult>>> Handle(
      GetAllAppointmentsQuery request,
      CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var appointments =
                await appointmentService.GetAllAppointmentsByUserIdAsync(
                    userId);

            var result =
                mapper.Map<List<GetAllAppointmentsResult>>(appointments);

            return Success(result);
        }
    }
}
