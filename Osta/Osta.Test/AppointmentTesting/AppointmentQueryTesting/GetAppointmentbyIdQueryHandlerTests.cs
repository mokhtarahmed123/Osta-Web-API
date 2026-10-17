
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.Appointment.Query.Handler;
using Osta.Core.Feature.Appointment.Query.Model;
using Osta.Core.Feature.Appointment.Query.Result;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.AppointmentTesting.AppointmentQueryTesting
{
    public class GetAppointmentbyIdQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAppointmentService> _appointmentServiceMock;

        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;

        private readonly GetAppointmentbyIdQueryHandler _handler;

        public GetAppointmentbyIdQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();

            _appointmentServiceMock = new Mock<IAppointmentService>();

            var userStore = new Mock<IUserStore<User>>();

            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _bookingServiceMock = new Mock<IBookingService>();

            _handler = new GetAppointmentbyIdQueryHandler(
                _mapperMock.Object,
                _appointmentServiceMock.Object,

                _currentUserServiceMock.Object,
                _bookingServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null!, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenAppointmentIdIsEmpty()
        {

            var request = new GetAppointmentbyIdQuery("")
            {
                Id = ""
            };


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Contains(
                "Appointment Id is required.",
                result.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new GetAppointmentbyIdQuery("appointment-1")
            {
                Id = "appointment-1"
            };


            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-1");

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Appointment.Appointment?)null);

            var request = new GetAppointmentbyIdQuery("appointment-1")
            {
                Id = "appointment-1"
            };


            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            Assert.NotNull(result);

            Assert.Contains(
                "Appointment not found.",
                result.Message);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(It.IsAny<int>(), CancellationToken.None),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<GetAppointmentbyIdResult>(
                    It.IsAny<Domain.Entities.Appointment.Appointment>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-1");

            var appointment = CreateAppointment();

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(appointment.BookingId, CancellationToken.None))
                .ReturnsAsync((Bookings?)null);

            var request = new GetAppointmentbyIdQuery("appointment-1")
            {
                Id = "appointment-1"
            };


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Contains(
                "Booking not found.",
                result.Message);

            _mapperMock.Verify(
                x => x.Map<GetAppointmentbyIdResult>(
                    It.IsAny<Domain.Entities.Appointment.Appointment>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotTechnicianOrCustomer()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-3");

            var appointment = CreateAppointment();

            var booking = CreateBooking(
                technicianId: "technician-1",
                customerId: "customer-1");

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(appointment.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            var request = new GetAppointmentbyIdQuery("appointment-1")
            {
                Id = "appointment-1"
            };


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Contains(
                "You are not authorized to view this appointment.",
                result.Message);

            _mapperMock.Verify(
                x => x.Map<GetAppointmentbyIdResult>(
                    It.IsAny<Domain.Entities.Appointment.Appointment>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsTechnician()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            var appointment = CreateAppointment();

            var booking = CreateBooking(
                technicianId: "technician-1",
                customerId: "customer-1");

            var expectedResult = new GetAppointmentbyIdResult();

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(appointment.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(x => x.Map<GetAppointmentbyIdResult>(appointment))
                .Returns(expectedResult);

            var request = new GetAppointmentbyIdQuery("appointment-1")
            {
                Id = "appointment-1"
            };


            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.Same(expectedResult, result.Data);

            _mapperMock.Verify(
                x => x.Map<GetAppointmentbyIdResult>(appointment),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsCustomer()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var appointment = CreateAppointment();

            var booking = CreateBooking(
                technicianId: "technician-1",
                customerId: "customer-1");

            var expectedResult = new GetAppointmentbyIdResult();

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(appointment.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(x => x.Map<GetAppointmentbyIdResult>(appointment))
                .Returns(expectedResult);

            var request = new GetAppointmentbyIdQuery("appointment-1")
            {
                Id = "appointment-1"
            };


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);
            Assert.Same(expectedResult, result.Data);

            _mapperMock.Verify(
                x => x.Map<GetAppointmentbyIdResult>(appointment),
                Times.Once);
        }

        private static Domain.Entities.Appointment.Appointment CreateAppointment()
        {
            return new Domain.Entities.Appointment.Appointment
            {
                Id = "appointment-1",
                BookingId = 1,
                ScheduledStart = new DateTime(2026, 8, 30, 10, 0, 0),
                ScheduledEnd = new DateTime(2026, 8, 30, 11, 0, 0),
                IsApproved = true,
                Notes = "Test appointment"
            };
        }

        private static Bookings CreateBooking(
            string technicianId,
            string customerId)
        {
            return new Bookings
            {
                Id = 1,
                TechnicianId = technicianId,
                CustomerId = customerId
            };
        }
    }
}

