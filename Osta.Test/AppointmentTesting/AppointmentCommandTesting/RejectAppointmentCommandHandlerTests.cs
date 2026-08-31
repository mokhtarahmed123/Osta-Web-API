using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.Appointment.Command.Handler;
using Osta.Core.Feature.Appointment.Command.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.AppointmentTesting.AppointmentCommandTesting
{
    public class RejectAppointmentCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAppointmentService> _appointmentServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<ISendNotificationMessage> _sendNotificationMessageMock;

        private readonly RejectAppointmentCommandHandler _handler;

        public RejectAppointmentCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();

            _appointmentServiceMock =
                new Mock<IAppointmentService>();

            _currentUserServiceMock =
                new Mock<ICurrentUserService>();

            _bookingServiceMock =
                new Mock<IBookingService>();

            _sendNotificationMessageMock =
                new Mock<ISendNotificationMessage>();

            var store = new Mock<IUserStore<User>>();

            _userManagerMock =
                new Mock<UserManager<User>>(
                    store.Object,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);

            _handler = new RejectAppointmentCommandHandler(
                _mapperMock.Object,
                _appointmentServiceMock.Object,
                _userManagerMock.Object,
                _currentUserServiceMock.Object,
                _bookingServiceMock.Object,
                _sendNotificationMessageMock.Object);
        }


        [Fact]
        public async Task Should_ThrowArgumentNullException_When_RequestIsNull()
        {

            RejectAppointmentCommand request = null!;


            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _handler.Handle(
                    request,
                    CancellationToken.None));
        }


        [Fact]
        public async Task Should_ReturnBadRequest_When_AppointmentIdIsEmpty()
        {

            var request = new RejectAppointmentCommand(
                AppointmentId: "",
                Reason: "Customer is not available");


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            _appointmentServiceMock.Verify(
                x => x.Get(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }


        [Fact]
        public async Task Should_ReturnBadRequest_When_RejectionReasonIsEmpty()
        {

            var request = new RejectAppointmentCommand(
                AppointmentId: "appointment-1",
                Reason: "");


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            _appointmentServiceMock.Verify(
                x => x.Get(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }


        [Fact]
        public async Task Should_ThrowUnauthorizedAccessException_When_CustomerIdIsEmpty()
        {

            var request = new RejectAppointmentCommand(
                AppointmentId: "appointment-1",
                Reason: "Customer is not available");

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string?)null);


            var exception =
                await Assert.ThrowsAsync<UnauthorizedAccessException>(
                    () => _handler.Handle(
                        request,
                        CancellationToken.None));

            Assert.Equal(
                "You are not authorized.",
                exception.Message);
        }


        [Fact]
        public async Task Should_ReturnNotFound_When_AppointmentDoesNotExist()
        {

            var request = new RejectAppointmentCommand(
                AppointmentId: "appointment-1",
                Reason: "Customer is not available");

            var customerId = "customer-1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(customerId);

            _appointmentServiceMock
                .Setup(x => x.Get(
                    request.AppointmentId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    (Domain.Entities.Appointment.Appointment?)null);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(
                    It.IsAny<int>(), CancellationToken.None),
                Times.Never);

            _appointmentServiceMock.Verify(
                x => x.RejectAppointmentAsync(
                    It.IsAny<string>(), CancellationToken.None),
                Times.Never);

            _sendNotificationMessageMock.Verify(
                x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"),
                Times.Never);
        }


        [Fact]
        public async Task Should_ReturnNotFound_When_BookingDoesNotExist()
        {

            var request = new RejectAppointmentCommand(
                AppointmentId: "appointment-1",
                Reason: "Customer is not available");

            var customerId = "customer-1";

            var appointment =
                new Domain.Entities.Appointment.Appointment
                {
                    Id = "appointment-1",
                    BookingId = 10,
                    IsApproved = false
                };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(customerId);

            _appointmentServiceMock
                .Setup(x => x.Get(
                    request.AppointmentId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    appointment.BookingId, CancellationToken.None))
                .ReturnsAsync((Bookings?)null);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            _appointmentServiceMock.Verify(
                x => x.RejectAppointmentAsync(
                    It.IsAny<string>(), CancellationToken.None),
                Times.Never);

            _sendNotificationMessageMock.Verify(
                x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"),
                Times.Never);
        }


        [Fact]
        public async Task Should_ReturnUnauthorized_When_AppointmentDoesNotBelongToCustomer()
        {

            var request = new RejectAppointmentCommand(
                AppointmentId: "appointment-1",
                Reason: "Customer is not available");

            var customerId = "customer-1";

            var appointment =
                new Domain.Entities.Appointment.Appointment
                {
                    Id = "appointment-1",
                    BookingId = 10,
                    IsApproved = false
                };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = "another-customer",
                TechnicianId = "technician-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(customerId);

            _appointmentServiceMock
                .Setup(x => x.Get(
                    request.AppointmentId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    appointment.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            _appointmentServiceMock.Verify(
                x => x.RejectAppointmentAsync(
                    It.IsAny<string>(), CancellationToken.None),
                Times.Never);

            _sendNotificationMessageMock.Verify(
                x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"),
                Times.Never);
        }


        [Fact]
        public async Task Should_ReturnBadRequest_When_AppointmentIsAlreadyApproved()
        {

            var request = new RejectAppointmentCommand(
                AppointmentId: "appointment-1",
                Reason: "Customer is not available");

            var customerId = "customer-1";

            var appointment =
                new Domain.Entities.Appointment.Appointment
                {
                    Id = "appointment-1",
                    BookingId = 10,
                    IsApproved = true
                };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = customerId,
                TechnicianId = "technician-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(customerId);

            _appointmentServiceMock
                .Setup(x => x.Get(
                    request.AppointmentId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    appointment.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            _appointmentServiceMock.Verify(
                x => x.RejectAppointmentAsync(
                    It.IsAny<string>(), CancellationToken.None),
                Times.Never);

            _sendNotificationMessageMock.Verify(
                x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"),
                Times.Never);
        }


        [Fact]
        public async Task Should_ReturnNotFound_When_TechnicianDoesNotExist()
        {

            var request = new RejectAppointmentCommand(
                AppointmentId: "appointment-1",
                Reason: "Customer is not available");

            var customerId = "customer-1";

            var appointment =
                new Domain.Entities.Appointment.Appointment
                {
                    Id = "appointment-1",
                    BookingId = 10,
                    IsApproved = false
                };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = customerId,
                TechnicianId = "technician-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(customerId);

            _appointmentServiceMock
                .Setup(x => x.Get(
                    request.AppointmentId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    appointment.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _appointmentServiceMock
                .Setup(x => x.RejectAppointmentAsync(
                    appointment.Id, CancellationToken.None))
                .Returns(Task.CompletedTask);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(
                    booking.TechnicianId))
                .ReturnsAsync((User?)null);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            _appointmentServiceMock.Verify(
                x => x.RejectAppointmentAsync(
                    appointment.Id, CancellationToken.None),
                Times.Once);

            _sendNotificationMessageMock.Verify(
                x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"),
                Times.Never);
        }


        [Fact]
        public async Task Should_RejectAppointmentAndNotifyTechnician_When_RequestIsValid()
        {

            var request = new RejectAppointmentCommand(
                AppointmentId: "appointment-1",
                Reason: "Customer is not available");

            var customerId = "customer-1";

            var appointment =
                new Domain.Entities.Appointment.Appointment
                {
                    Id = "appointment-1",
                    BookingId = 10,
                    IsApproved = false
                };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = customerId,
                TechnicianId = "technician-1"
            };

            var technician = new User
            {
                Id = "technician-1",
                Email = "technician@test.com",
                FullName = "Test Technician"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(customerId);

            _appointmentServiceMock
                .Setup(x => x.Get(
                    request.AppointmentId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    appointment.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _appointmentServiceMock
                .Setup(x => x.RejectAppointmentAsync(
                    appointment.Id, CancellationToken.None))
                .Returns(Task.CompletedTask);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(
                    booking.TechnicianId))
                .ReturnsAsync(technician);

            _sendNotificationMessageMock
                .Setup(x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"))
                .Returns(Task.CompletedTask);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Equal(
                "Appointment rejected successfully.",
                result.Data);

            _appointmentServiceMock.Verify(
                x => x.RejectAppointmentAsync(
                    appointment.Id, CancellationToken.None),
                Times.Once);

            _sendNotificationMessageMock.Verify(
                x => x.SendNotification(
                    It.Is<NotificationMessage>(n =>
                        n.RecipientId == technician.Id &&
                        n.RecipientEmail == technician.Email &&
                        n.BookingId == booking.Id &&
                        n.RecipientName == technician.FullName &&
                        n.Message.Contains(request.Reason)),
                    "Notification"),
                Times.Once);
        }
    }
}