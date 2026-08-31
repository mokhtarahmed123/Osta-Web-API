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

public class ApproveAppointmentCommandHandlerTests
{

    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<ISendNotificationMessage> _sendNotificationMessageMock;

    private readonly ApproveAppointmentCommandHandler _handler;

    public ApproveAppointmentCommandHandlerTests()
    {

        _appointmentServiceMock = new Mock<IAppointmentService>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _bookingServiceMock = new Mock<IBookingService>();
        _sendNotificationMessageMock = new Mock<ISendNotificationMessage>();

        var store = new Mock<IUserStore<User>>();

        _userManagerMock = new Mock<UserManager<User>>(
            store.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        _handler = new ApproveAppointmentCommandHandler(

            _appointmentServiceMock.Object,
            _userManagerMock.Object,
            _currentUserServiceMock.Object,
            _bookingServiceMock.Object,
            _sendNotificationMessageMock.Object);
    }

    [Fact]
    public async Task Should_ThrowArgumentNullException_When_RequestIsNull()
    {

        ApproveAppointmentCommand request = null!;


        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _handler.Handle(request, CancellationToken.None));
    }
    [Fact]
    public async Task Should_ThrowUnauthorizedAccessException_When_CustomerIdIsEmpty()
    {

        var request = new ApproveAppointmentCommand("1")
        {
            AppointmentId = "1"
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns((string?)null);


        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(request, CancellationToken.None));

        Assert.Equal(
            "You are not authorized.",
            exception.Message);
    }
    [Fact]
    public async Task Should_ReturnNotFound_When_AppointmentDoesNotExist()
    {

        var request = new ApproveAppointmentCommand("1")
        {
            AppointmentId = "1"
        };

        var customerId = "customer-1";

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(customerId);

        _appointmentServiceMock
            .Setup(x => x.Get(
                request.AppointmentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (Osta.Domain.Entities.Appointment.Appointment?)null);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _bookingServiceMock.Verify(
            x => x.GetBookingById(It.IsAny<int>(), CancellationToken.None),
            Times.Never);

        _appointmentServiceMock.Verify(
            x => x.ApproveAppointmentAsync(It.IsAny<string>(), CancellationToken.None),
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

        var request = new ApproveAppointmentCommand("1")
        {
            AppointmentId = "1"
        };

        var customerId = "customer-1";

        var appointment =
            new Osta.Domain.Entities.Appointment.Appointment
            {
                Id = "1",
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
            .Setup(x => x.GetBookingById(appointment.BookingId, CancellationToken.None))
            .ReturnsAsync((Bookings?)null);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _appointmentServiceMock.Verify(
            x => x.ApproveAppointmentAsync(It.IsAny<string>(), CancellationToken.None),
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

        var request = new ApproveAppointmentCommand("1")
        {
            AppointmentId = "1"
        };

        var customerId = "customer-1";

        var appointment =
            new Osta.Domain.Entities.Appointment.Appointment
            {
                Id = "1",
                BookingId = 10,
                IsApproved = false
            };

        var booking = new Bookings
        {
            Id = 10,
            CustomerId = "another-customer",
            TechnicianId = "tech-1"
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
            .Setup(x => x.GetBookingById(appointment.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _appointmentServiceMock.Verify(
            x => x.ApproveAppointmentAsync(It.IsAny<string>(), CancellationToken.None),
            Times.Never);

        _sendNotificationMessageMock.Verify(
            x => x.SendNotification(
                It.IsAny<NotificationMessage>(),
                "Notification"),
            Times.Never);
    }
    [Fact]
    public async Task Should_ReturnBadRequest_When_AppointmentAlreadyApproved()
    {

        var request = new ApproveAppointmentCommand("1")
        {
            AppointmentId = "1"
        };

        var customerId = "customer-1";

        var appointment =
            new Osta.Domain.Entities.Appointment.Appointment
            {
                Id = "1",
                BookingId = 10,
                IsApproved = true
            };

        var booking = new Bookings
        {
            Id = 10,
            CustomerId = customerId,
            TechnicianId = "tech-1"
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
            .Setup(x => x.GetBookingById(appointment.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _appointmentServiceMock.Verify(
            x => x.ApproveAppointmentAsync(It.IsAny<string>(), CancellationToken.None),
            Times.Never);

        _sendNotificationMessageMock.Verify(
            x => x.SendNotification(
                It.IsAny<NotificationMessage>(),
                "Notification"),
            Times.Never);
    }

    [Fact]
    public async Task Should_ApproveAppointmentAndNotifyTechnician_When_RequestIsValid()
    {

        var request = new ApproveAppointmentCommand("1")
        {
            AppointmentId = "1"
        };

        var customerId = "customer-1";

        var appointment =
            new Osta.Domain.Entities.Appointment.Appointment
            {
                Id = "1",
                BookingId = 10,
                IsApproved = false,
                ScheduledStart = new DateTime(2026, 8, 30, 10, 0, 0),
                ScheduledEnd = new DateTime(2026, 8, 30, 11, 0, 0)
            };

        var booking = new Bookings
        {
            Id = 10,
            CustomerId = customerId,
            TechnicianId = "tech-1"
        };

        var technician = new User
        {
            Id = "tech-1",
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
            .Setup(x => x.GetBookingById(appointment.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);

        _appointmentServiceMock
            .Setup(x => x.ApproveAppointmentAsync(
                request.AppointmentId, CancellationToken.None))
            .Returns(Task.CompletedTask);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(booking.TechnicianId))
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
            "Appointment approved successfully.",
            result.Data);


        _appointmentServiceMock.Verify(
            x => x.ApproveAppointmentAsync(
                request.AppointmentId, CancellationToken.None),
            Times.Once);


        _sendNotificationMessageMock.Verify(
            x => x.SendNotification(
                It.Is<NotificationMessage>(n =>
                    n.RecipientId == technician.Id &&
                    n.RecipientEmail == technician.Email &&
                    n.BookingId == booking.Id &&
                    n.RecipientName == technician.FullName),
                "Notification"),
            Times.Once);
    }
    [Fact]
    public async Task Should_ReturnNotFound_When_TechnicianDoesNotExist()
    {

        var request = new ApproveAppointmentCommand("1")
        {
            AppointmentId = "1"
        };

        var customerId = "customer-1";

        var appointment =
            new Osta.Domain.Entities.Appointment.Appointment
            {
                Id = "1",
                BookingId = 10,
                IsApproved = false
            };

        var booking = new Bookings
        {
            Id = 10,
            CustomerId = customerId,
            TechnicianId = "tech-1"
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
            .Setup(x => x.GetBookingById(appointment.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);

        _appointmentServiceMock
            .Setup(x => x.ApproveAppointmentAsync(
                request.AppointmentId, CancellationToken.None))
            .Returns(Task.CompletedTask);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(booking.TechnicianId))
            .ReturnsAsync((User?)null);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _sendNotificationMessageMock.Verify(
            x => x.SendNotification(
                It.IsAny<NotificationMessage>(),
                "Notification"),
            Times.Never);
    }

}