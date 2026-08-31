using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.Appointment.Command.Handler;
using Osta.Core.Feature.Appointment.Command.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Identity;

public class AddAppointmentCommandHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAppointmentService> _appointmentServiceMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<ISendNotificationMessage> _sendNotificationMessageMock;

    private readonly AddAppointmentCommandHandler _handler;

    public AddAppointmentCommandHandlerTests()
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

        _handler = new AddAppointmentCommandHandler(
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

        AddAppointmentCommand request = null!;


        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _handler.Handle(request, CancellationToken.None));
    }
    [Fact]
    public async Task Should_ThrowUnauthorizedAccessException_When_UserIdIsEmpty()
    {
        var request = new AddAppointmentCommand(1)
        {
            BookingId = 1
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
    public async Task Should_ReturnNotFound_When_BookingDoesNotExist()
    {

        var request = new AddAppointmentCommand(1)
        {
            BookingId = 1
        };

        var technicianId = "tech-1";

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(technicianId);

        _bookingServiceMock
            .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
            .ReturnsAsync((Bookings?)null);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _bookingServiceMock.Verify(
            x => x.GetBookingById(request.BookingId, CancellationToken.None),
            Times.Once);

        _appointmentServiceMock.Verify(
            x => x.HasConflictAsync(
                It.IsAny<string?>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_AppointmentHasConflict()
    {

        var request = new AddAppointmentCommand(1)
        {
            BookingId = 1,
            ScheduledStart = DateTime.Now.AddHours(1),
            ScheduledEnd = DateTime.Now.AddHours(2)
        };

        var technicianId = "tech-1";

        var booking = new Bookings
        {
            Id = 1,
            TechnicianId = technicianId,
            CustomerId = "customer-1",
            Status = BookingStatus.Confirmed
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(technicianId);

        _bookingServiceMock
            .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);

        _appointmentServiceMock
            .Setup(x => x.HasConflictAsync(
                null,
                technicianId,
                request.ScheduledStart,
                request.ScheduledEnd,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _appointmentServiceMock.Verify(
            x => x.GetByBookingIdAsync(It.IsAny<int>(), CancellationToken.None),
            Times.Never);

        _appointmentServiceMock.Verify(
            x => x.AddAppointmentAsync(
                It.IsAny<Osta.Domain.Entities.Appointment.Appointment>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _sendNotificationMessageMock.Verify(
            x => x.SendNotification(
                It.IsAny<NotificationMessage>(),
                "Notification"),
            Times.Never);
    }
    [Fact]
    public async Task Should_ReturnUnauthorized_When_BookingDoesNotBelongToTechnician()
    {

        var request = new AddAppointmentCommand(1)
        {
            BookingId = 1,
            ScheduledStart = DateTime.Now.AddHours(1),
            ScheduledEnd = DateTime.Now.AddHours(2)
        };

        var technicianId = "tech-1";

        var booking = new Bookings
        {
            Id = 1,
            TechnicianId = "another-tech",
            CustomerId = "customer-1",
            Status = BookingStatus.Confirmed
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(technicianId);

        _bookingServiceMock
            .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);

        _appointmentServiceMock
            .Setup(x => x.HasConflictAsync(
                null,
                technicianId,
                request.ScheduledStart,
                request.ScheduledEnd,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _appointmentServiceMock.Verify(
            x => x.GetByBookingIdAsync(It.IsAny<int>(), CancellationToken.None),
            Times.Never);

        _appointmentServiceMock.Verify(
            x => x.AddAppointmentAsync(
                It.IsAny<Osta.Domain.Entities.Appointment.Appointment>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    [Fact]
    public async Task Should_ReturnBadRequest_When_BookingIsNotConfirmed()
    {

        var request = new AddAppointmentCommand(1)
        {
            BookingId = 1,
            ScheduledStart = DateTime.Now.AddHours(1),
            ScheduledEnd = DateTime.Now.AddHours(2)
        };

        var technicianId = "tech-1";

        var booking = new Bookings
        {
            Id = 1,
            TechnicianId = technicianId,
            CustomerId = "customer-1",
            Status = BookingStatus.Pending
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(technicianId);

        _bookingServiceMock
            .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);

        _appointmentServiceMock
            .Setup(x => x.HasConflictAsync(
                null,
                technicianId,
                request.ScheduledStart,
                request.ScheduledEnd,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _appointmentServiceMock.Verify(
            x => x.GetByBookingIdAsync(It.IsAny<int>(), CancellationToken.None),
            Times.Never);

        _appointmentServiceMock.Verify(
            x => x.AddAppointmentAsync(
                It.IsAny<Osta.Domain.Entities.Appointment.Appointment>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    [Fact]
    public async Task Should_ReturnBadRequest_When_AppointmentAlreadyExists()
    {

        var request = new AddAppointmentCommand(1)
        {
            BookingId = 1,
            ScheduledStart = DateTime.Now.AddHours(1),
            ScheduledEnd = DateTime.Now.AddHours(2)
        };

        var technicianId = "tech-1";

        var booking = new Bookings
        {
            Id = 1,
            TechnicianId = technicianId,
            CustomerId = "customer-1",
            Status = BookingStatus.Confirmed
        };

        var existingAppointment =
            new Osta.Domain.Entities.Appointment.Appointment();

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(technicianId);

        _bookingServiceMock
            .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);

        _appointmentServiceMock
            .Setup(x => x.HasConflictAsync(
                null,
                technicianId,
                request.ScheduledStart,
                request.ScheduledEnd,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _appointmentServiceMock
            .Setup(x => x.GetByBookingIdAsync(request.BookingId, CancellationToken.None))
            .ReturnsAsync(existingAppointment);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _appointmentServiceMock.Verify(
            x => x.AddAppointmentAsync(
                It.IsAny<Osta.Domain.Entities.Appointment.Appointment>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _sendNotificationMessageMock.Verify(
            x => x.SendNotification(
                It.IsAny<NotificationMessage>(),
                "Notification"),
            Times.Never);
    }
    [Fact]
    public async Task Should_ReturnNotFound_When_CustomerDoesNotExist()
    {

        var request = new AddAppointmentCommand(1)
        {
            BookingId = 1,
            ScheduledStart = DateTime.Now.AddHours(1),
            ScheduledEnd = DateTime.Now.AddHours(2)
        };

        var technicianId = "tech-1";

        var booking = new Bookings
        {
            Id = 1,
            TechnicianId = technicianId,
            CustomerId = "customer-1",
            Status = BookingStatus.Confirmed
        };

        var appointment =
            new Osta.Domain.Entities.Appointment.Appointment();

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(technicianId);

        _bookingServiceMock
            .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);

        _appointmentServiceMock
            .Setup(x => x.HasConflictAsync(
                null,
                technicianId,
                request.ScheduledStart,
                request.ScheduledEnd,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _appointmentServiceMock
            .Setup(x => x.GetByBookingIdAsync(request.BookingId, CancellationToken.None))
            .ReturnsAsync((Osta.Domain.Entities.Appointment.Appointment?)null);

        _mapperMock
            .Setup(x => x.Map<Osta.Domain.Entities.Appointment.Appointment>(request))
            .Returns(appointment);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(booking.CustomerId))
            .ReturnsAsync((User?)null);


        var result = await _handler.Handle(
            request,
            CancellationToken.None);


        Assert.NotNull(result);

        _appointmentServiceMock.Verify(
            x => x.AddAppointmentAsync(
                appointment,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _sendNotificationMessageMock.Verify(
            x => x.SendNotification(
                It.IsAny<NotificationMessage>(),
                "Notification"),
            Times.Never);
    }
    [Fact]
    public async Task Should_CreateAppointmentAndSendNotification_When_RequestIsValid()
    {

        var request = new AddAppointmentCommand(1)
        {
            BookingId = 1,
            ScheduledStart = new DateTime(2026, 8, 30, 10, 0, 0),
            ScheduledEnd = new DateTime(2026, 8, 30, 11, 0, 0)
        };

        var technicianId = "tech-1";

        var booking = new Bookings
        {
            Id = 1,
            TechnicianId = technicianId,
            CustomerId = "customer-1",
            Status = BookingStatus.Confirmed
        };

        var appointment =
            new Osta.Domain.Entities.Appointment.Appointment();

        var customer = new User
        {
            Id = "customer-1",
            Email = "customer@test.com",
            FullName = "Test Customer"
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(technicianId);

        _bookingServiceMock
            .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
            .ReturnsAsync(booking);

        _appointmentServiceMock
            .Setup(x => x.HasConflictAsync(
                null,
                technicianId,
                request.ScheduledStart,
                request.ScheduledEnd,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _appointmentServiceMock
            .Setup(x => x.GetByBookingIdAsync(request.BookingId, CancellationToken.None))
            .ReturnsAsync(
                (Osta.Domain.Entities.Appointment.Appointment?)null);

        _mapperMock
            .Setup(x => x.Map<Osta.Domain.Entities.Appointment.Appointment>(request))
            .Returns(appointment);

        _appointmentServiceMock
            .Setup(x => x.AddAppointmentAsync(
                appointment,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(booking.CustomerId))
            .ReturnsAsync(customer);

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
            "Appointment created successfully. Customer has been notified.",
            result.Data);


        Assert.Equal(request.BookingId, appointment.BookingId);
        Assert.False(appointment.IsApproved);


        _appointmentServiceMock.Verify(
            x => x.AddAppointmentAsync(
                appointment,
                It.IsAny<CancellationToken>()),
            Times.Once);


        _sendNotificationMessageMock.Verify(
            x => x.SendNotification(
                It.Is<NotificationMessage>(n =>
                    n.RecipientId == booking.CustomerId &&
                    n.RecipientEmail == customer.Email &&
                    n.BookingId == booking.Id &&
                    n.RecipientName == customer.FullName),
                "Notification"),
            Times.Once);
    }
}