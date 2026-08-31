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
    public class UpdateAppointmentCommandHandlerTests
    {

        private readonly Mock<IAppointmentService> _appointmentServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<ISendNotificationMessage> _notificationMock;

        private readonly UpdateAppointmentCommandHandler _handler;

        public UpdateAppointmentCommandHandlerTests()
        {


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

            _notificationMock = new Mock<ISendNotificationMessage>();

            _handler = new UpdateAppointmentCommandHandler(

                _appointmentServiceMock.Object,
                _userManagerMock.Object,
                _currentUserServiceMock.Object,
                _bookingServiceMock.Object,
                _notificationMock.Object);
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

            var request = new UpdateAppointmentCommand("", 1)
            {
                AppointmentId = "",
                BookingId = 1,
                ScheduledStart = DateTime.Now,
                ScheduledEnd = DateTime.Now.AddHours(1)
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
        public async Task Handle_ShouldReturnBadRequest_WhenBookingIdIsInvalid()
        {

            var request = new UpdateAppointmentCommand("appointment-1", 0)
            {
                AppointmentId = "appointment-1",
                BookingId = 0,
                ScheduledStart = DateTime.Now,
                ScheduledEnd = DateTime.Now.AddHours(1)
            };


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);
            Assert.Contains(
                "Booking Id must be greater than 0.",
                result.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenTechnicianIdIsEmpty()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = CreateRequest();


            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Appointment.Appointment?)null);

            var request = CreateRequest();


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
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            var appointment = CreateAppointment();

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync((Bookings?)null);

            var request = CreateRequest();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Contains(
                "Booking not found.",
                result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenBookingDoesNotBelongToTechnician()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            var appointment = CreateAppointment();

            var booking = CreateBooking(
                technicianId: "technician-2");

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            var request = CreateRequest();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Contains(
                "This booking does not belong to you.",
                result.Message);

            _appointmentServiceMock.Verify(
                x => x.HasConflictAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(), CancellationToken.None),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenAppointmentDoesNotBelongToBooking()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            var appointment = CreateAppointment();
            appointment.BookingId = 2;

            var booking = CreateBooking(
                technicianId: "technician-1");

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            var request = CreateRequest();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Contains(
                "This appointment does not belong to this booking.",
                result.Message);

            _appointmentServiceMock.Verify(
                x => x.HasConflictAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(), CancellationToken.None),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenAppointmentHasConflict()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            var appointment = CreateAppointment();

            var booking = CreateBooking(
                technicianId: "technician-1");

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            _appointmentServiceMock
                .Setup(x => x.HasConflictAsync(
                    "appointment-1",
                    "technician-1",
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(true);

            var request = CreateRequest();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Contains(
                "You already have another appointment during this time.",
                result.Message);

            _appointmentServiceMock.Verify(
                x => x.UpdateAppointmentAsync(
                    It.IsAny<string>(),
                    It.IsAny<Domain.Entities.Appointment.Appointment>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _notificationMock.Verify(
                x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            var appointment = CreateAppointment();

            var booking = CreateBooking(
                technicianId: "technician-1");

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            _appointmentServiceMock
                .Setup(x => x.HasConflictAsync(
                    "appointment-1",
                    "technician-1",
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(false);

            _appointmentServiceMock
                .Setup(x => x.UpdateAppointmentAsync(
                    "appointment-1",
                    It.IsAny<Domain.Entities.Appointment.Appointment>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(booking.CustomerId))
                .ReturnsAsync((User?)null);

            var request = CreateRequest();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Contains(
                "Customer not found.",
                result.Message);

            _notificationMock.Verify(
                x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUpdateAppointmentAndSendNotification_WhenRequestIsValid()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            var appointment = CreateAppointment();

            var booking = CreateBooking(
                technicianId: "technician-1");

            var customer = new User
            {
                Id = "customer-1",
                Email = "customer@test.com",
                FullName = "Ahmed Customer"
            };

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            _appointmentServiceMock
                .Setup(x => x.HasConflictAsync(
                    "appointment-1",
                    "technician-1",
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(false);

            _appointmentServiceMock
                .Setup(x => x.UpdateAppointmentAsync(
                    "appointment-1",
                    It.IsAny<Domain.Entities.Appointment.Appointment>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(booking.CustomerId))
                .ReturnsAsync(customer);

            _notificationMock
                .Setup(x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"))
                .Returns(Task.CompletedTask);

            var request = CreateRequest();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            Assert.NotNull(result);

            Assert.Equal(
                request.ScheduledStart,
                appointment.ScheduledStart);

            Assert.Equal(
                request.ScheduledEnd,
                appointment.ScheduledEnd);

            Assert.Equal(
                request.Notes,
                appointment.Notes);

            Assert.False(appointment.IsApproved);

            _appointmentServiceMock.Verify(
                x => x.UpdateAppointmentAsync(
                    "appointment-1",
                    appointment,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _notificationMock.Verify(
                x => x.SendNotification(
                    It.Is<NotificationMessage>(n =>
                        n.RecipientId == customer.Id &&
                        n.RecipientEmail == customer.Email &&
                        n.RecipientName == customer.FullName &&
                        n.BookingId == booking.Id &&
                        n.Message.Contains("Your appointment has been updated")),
                    "Notification"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSendCorrectNotificationMessage_WhenRequestIsValid()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            var appointment = CreateAppointment();

            var booking = CreateBooking(
                technicianId: "technician-1");

            var customer = new User
            {
                Id = "customer-1",
                Email = "customer@test.com",
                FullName = "Ahmed Customer"
            };

            _appointmentServiceMock
                .Setup(x => x.Get(
                    "appointment-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            _appointmentServiceMock
                .Setup(x => x.HasConflictAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(false);

            _appointmentServiceMock
                .Setup(x => x.UpdateAppointmentAsync(
                    It.IsAny<string>(),
                    It.IsAny<Domain.Entities.Appointment.Appointment>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(booking.CustomerId))
                .ReturnsAsync(customer);

            _notificationMock
                .Setup(x => x.SendNotification(
                    It.IsAny<NotificationMessage>(),
                    "Notification"))
                .Returns(Task.CompletedTask);

            var request = CreateRequest();


            await _handler.Handle(
                request,
                CancellationToken.None);


            _notificationMock.Verify(
                x => x.SendNotification(
                    It.Is<NotificationMessage>(n =>
                        n.RecipientId == "customer-1" &&
                        n.RecipientEmail == "customer@test.com" &&
                        n.RecipientName == "Ahmed Customer" &&
                        n.BookingId == 1 &&
                        n.Message ==
                            $"Your appointment has been updated. " +
                            $"New time: {request.ScheduledStart:dd/MM/yyyy hh:mm tt} " +
                            $"to {request.ScheduledEnd:hh:mm tt}."),
                    "Notification"),
                Times.Once);
        }

        private static UpdateAppointmentCommand CreateRequest()
        {
            return new UpdateAppointmentCommand("appointment-1", 1)
            {
                AppointmentId = "appointment-1",
                BookingId = 1,
                ScheduledStart = new DateTime(2026, 9, 1, 10, 0, 0),
                ScheduledEnd = new DateTime(2026, 9, 1, 11, 0, 0),
                Notes = "Updated appointment notes"
            };
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
                Notes = "Old notes"
            };
        }

        private static Bookings CreateBooking(string technicianId)
        {
            return new Bookings
            {
                Id = 1,
                TechnicianId = technicianId,
                CustomerId = "customer-1"
            };
        }
    }
}

