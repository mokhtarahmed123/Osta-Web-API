
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.Appointment.Query.Handler;
using Osta.Core.Feature.Appointment.Query.Model;
using Osta.Core.Feature.Appointment.Query.Result;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.AppointmentTesting.AppointmentQueryTesting
{
    public class GetAllAppointmentsQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAppointmentService> _appointmentServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;

        private readonly GetAllAppointmentsQueryHandler _handler;

        public GetAllAppointmentsQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();

            _appointmentServiceMock = new Mock<IAppointmentService>();

            var userStore = new Mock<IUserStore<User>>();

            _userManagerMock = new Mock<UserManager<User>>(
                userStore.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _bookingServiceMock = new Mock<IBookingService>();

            _handler = new GetAllAppointmentsQueryHandler(
                _mapperMock.Object,
                _appointmentServiceMock.Object,
                _userManagerMock.Object,
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
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsEmpty()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new GetAllAppointmentsQuery();

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Equal(
                "You are not authorized.",
                exception.Message);

            _appointmentServiceMock.Verify(
                x => x.GetAllAppointmentsByUserIdAsync(
                    It.IsAny<string>(), CancellationToken.None),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoAppointments()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-1");

            var appointments = new List<Domain.Entities.Appointment.Appointment>();

            var mappedResult = new List<GetAllAppointmentsResult>();

            _appointmentServiceMock
                .Setup(x => x.GetAllAppointmentsByUserIdAsync("user-1", CancellationToken.None))
                .ReturnsAsync(appointments);

            _mapperMock
                .Setup(x => x.Map<List<GetAllAppointmentsResult>>(appointments))
                .Returns(mappedResult);

            var request = new GetAllAppointmentsQuery();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);

            _appointmentServiceMock.Verify(
                x => x.GetAllAppointmentsByUserIdAsync("user-1", CancellationToken.None),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllAppointmentsResult>>(appointments),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnAppointments_WhenUserHasAppointments()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-1");

            var appointments = new List<Domain.Entities.Appointment.Appointment>
            {
                new Domain.Entities.Appointment.Appointment
                {
                    Id = "appointment-1",
                    BookingId = 1,
                    ScheduledStart = new DateTime(2026, 8, 30, 10, 0, 0),
                    ScheduledEnd = new DateTime(2026, 8, 30, 11, 0, 0),
                    IsApproved = true
                },
                new Domain.Entities.Appointment.Appointment
                {
                    Id = "appointment-2",
                    BookingId = 2,
                    ScheduledStart = new DateTime(2026, 8, 31, 12, 0, 0),
                    ScheduledEnd = new DateTime(2026, 8, 31, 13, 0, 0),
                    IsApproved = false
                }
            };

            var mappedResult = new List<GetAllAppointmentsResult>
            {
                new GetAllAppointmentsResult(),
                new GetAllAppointmentsResult()
            };

            _appointmentServiceMock
                .Setup(x => x.GetAllAppointmentsByUserIdAsync("user-1", CancellationToken.None))
                .ReturnsAsync(appointments);

            _mapperMock
                .Setup(x => x.Map<List<GetAllAppointmentsResult>>(appointments))
                .Returns(mappedResult);

            var request = new GetAllAppointmentsQuery();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(2, result.Data.Count);
            Assert.Same(mappedResult, result.Data);

            _appointmentServiceMock.Verify(
                x => x.GetAllAppointmentsByUserIdAsync("user-1", CancellationToken.None),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllAppointmentsResult>>(appointments),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectUserIdToService()
        {

            const string userId = "technician-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var appointments = new List<Domain.Entities.Appointment.Appointment>();

            _appointmentServiceMock
                .Setup(x => x.GetAllAppointmentsByUserIdAsync(userId, CancellationToken.None))
                .ReturnsAsync(appointments);

            _mapperMock
                .Setup(x => x.Map<List<GetAllAppointmentsResult>>(appointments))
                .Returns(new List<GetAllAppointmentsResult>());


            await _handler.Handle(
                new GetAllAppointmentsQuery(),
                CancellationToken.None);


            _appointmentServiceMock.Verify(
                x => x.GetAllAppointmentsByUserIdAsync(userId, CancellationToken.None),
                Times.Once);
        }
    }
}

