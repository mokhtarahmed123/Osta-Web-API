using Moq;
using Osta.Core.Feature.Appointment.Command.Handler;
using Osta.Core.Feature.Appointment.Command.Model.AppointmentJobMdoel;
using Osta.Data.Entities.Booking;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.AppointmentTesting.AppointmentCommandTesting
{
    public class SendAppointmentRemindersCommandHandlerTests
    {
        private readonly Mock<ISendNotificationMessage> _notificationServiceMock;
        private readonly Mock<ILoggerService> _loggerServiceMock;
        private readonly Mock<IAppointmentService> _appointmentServiceMock;

        private readonly SendAppointmentRemindersCommandHandler _handler;

        public SendAppointmentRemindersCommandHandlerTests()
        {
            _notificationServiceMock = new Mock<ISendNotificationMessage>();
            _loggerServiceMock = new Mock<ILoggerService>();
            _appointmentServiceMock = new Mock<IAppointmentService>();

            _handler = new SendAppointmentRemindersCommandHandler(
                _notificationServiceMock.Object,
                _loggerServiceMock.Object,
                _appointmentServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSentCount_WhenRemindersSentSuccessfully()
        {

            var appointments = new List<Domain.Entities.Appointment.Appointment>
            {
                CreateAppointment("1", "customer1@test.com", "Ahmed"),
                CreateAppointment("2", "customer2@test.com", "Mohamed")
            };

            _appointmentServiceMock
                .Setup(x => x.CheckUpcomingAppointmentsAsync(CancellationToken.None))
                .ReturnsAsync(appointments);

            _notificationServiceMock
                .Setup(x => x.SendNotification(
                    It.IsAny<AppointmentReminderNotification>(),
                    "appointment-email-notifications"))
                .Returns(Task.CompletedTask);

            _appointmentServiceMock
                .Setup(x => x.MakeReminderSentTrue(It.IsAny<string>(), CancellationToken.None))
                .Returns(Task.CompletedTask);


            var result = await _handler.Handle(
                new SendAppointmentRemindersCommand(),
                CancellationToken.None);


            Assert.NotNull(result);
            Assert.Equal(2, result.Data);

            _notificationServiceMock.Verify(
                x => x.SendNotification(
                    It.IsAny<AppointmentReminderNotification>(),
                    "appointment-email-notifications"),
                Times.Exactly(2));

            _appointmentServiceMock.Verify(
                x => x.MakeReminderSentTrue(It.IsAny<string>(), CancellationToken.None),
                Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenThereAreNoUpcomingAppointments()
        {

            _appointmentServiceMock
                .Setup(x => x.CheckUpcomingAppointmentsAsync(CancellationToken.None))
                .ReturnsAsync(new List<Domain.Entities.Appointment.Appointment>());


            var result = await _handler.Handle(
                new SendAppointmentRemindersCommand(),
                CancellationToken.None);


            Assert.NotNull(result);
            Assert.Equal(0, result.Data);

            _notificationServiceMock.Verify(
                x => x.SendNotification(
                    It.IsAny<AppointmentReminderNotification>(),
                    It.IsAny<string>()),
                Times.Never);

            _appointmentServiceMock.Verify(
                x => x.MakeReminderSentTrue(It.IsAny<string>(), CancellationToken.None),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldContinue_WhenNotificationFails()
        {

            var appointment1 = CreateAppointment(
                "1",
                "customer1@test.com",
                "Ahmed");

            var appointment2 = CreateAppointment(
                "2",
                "customer2@test.com",
                "Mohamed");

            var appointments = new List<Domain.Entities.Appointment.Appointment>
            {
                appointment1,
                appointment2
            };

            _appointmentServiceMock
                .Setup(x => x.CheckUpcomingAppointmentsAsync(CancellationToken.None))
                .ReturnsAsync(appointments);

            _notificationServiceMock
                .SetupSequence(x => x.SendNotification(
                    It.IsAny<AppointmentReminderNotification>(),
                    "appointment-email-notifications"))
                .ThrowsAsync(new Exception("Notification failed"))
                .Returns(Task.CompletedTask);

            _appointmentServiceMock
                .Setup(x => x.MakeReminderSentTrue(It.IsAny<string>(), CancellationToken.None))
                .Returns(Task.CompletedTask);


            var result = await _handler.Handle(
                new SendAppointmentRemindersCommand(),
                CancellationToken.None);


            Assert.NotNull(result);


            Assert.Equal(1, result.Data);

            _notificationServiceMock.Verify(
                x => x.SendNotification(
                    It.IsAny<AppointmentReminderNotification>(),
                    "appointment-email-notifications"),
                Times.Exactly(2));

            _appointmentServiceMock.Verify(
                x => x.MakeReminderSentTrue("1", CancellationToken.None),
                Times.Never);

            _appointmentServiceMock.Verify(
                x => x.MakeReminderSentTrue("2", CancellationToken.None),
                Times.Once);

            _loggerServiceMock.Verify(
                x => x.LogError(
                    It.Is<string>(message =>
                        message.Contains("Failed to send reminder for appointment 1"))),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSendCorrectNotificationData()
        {

            var appointment = CreateAppointment(
                "1",
                "customer@test.com",
                "Ahmed");

            _appointmentServiceMock
                .Setup(x => x.CheckUpcomingAppointmentsAsync(CancellationToken.None))
                .ReturnsAsync(new List<Domain.Entities.Appointment.Appointment>
                {
                    appointment
                });

            _notificationServiceMock
                .Setup(x => x.SendNotification(
                    It.IsAny<AppointmentReminderNotification>(),
                    "appointment-email-notifications"))
                .Returns(Task.CompletedTask);

            _appointmentServiceMock
                .Setup(x => x.MakeReminderSentTrue(It.IsAny<string>(), CancellationToken.None))
                .Returns(Task.CompletedTask);


            await _handler.Handle(
                new SendAppointmentRemindersCommand(),
                CancellationToken.None);


            _notificationServiceMock.Verify(
                x => x.SendNotification(
                    It.Is<AppointmentReminderNotification>(n =>
                        n.ToEmail == "customer@test.com" &&
                        n.Subject == "Appointment Reminder" &&
                        n.Body.Contains("Ahmed") &&
                        n.Body.Contains(appointment.ScheduledStart.ToString("t"))),
                    "appointment-email-notifications"),
                Times.Once);
        }

        private static Domain.Entities.Appointment.Appointment CreateAppointment(
            string id,
            string customerEmail,
            string technicianName)
        {
            return new Domain.Entities.Appointment.Appointment
            {
                Id = id,
                ScheduledStart = new DateTime(2026, 8, 30, 10, 30, 0),

                Booking = new Bookings
                {
                    Customer = new()
                    {
                        Email = customerEmail
                    },

                    Technician = new()
                    {
                        User = new()
                        {
                            FullName = technicianName
                        }
                    }
                }
            };
        }
    }
}

