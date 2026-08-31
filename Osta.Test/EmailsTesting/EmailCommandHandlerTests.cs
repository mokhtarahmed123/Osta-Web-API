using Moq;
using Osta.Core.Feature.Emails.Command.Handler;
using Osta.Core.Feature.Emails.Command.Model;
using Osta.Notification.DTOs;
using Osta.Notification.Interfaces;

namespace Osta.Test.EmailsTesting
{
    public class EmailCommandHandlerTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly EmailCommandHandler _handler;

        public EmailCommandHandlerTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _handler = new EmailCommandHandler(_emailServiceMock.Object);
        }

        [Fact]
        public async Task Handle_EmailSentSuccessfully_ReturnsSuccess()
        {
            // Arrange
            var request = new SendEmailCommand("test@example.com", "Hello there")
            {
                Email = "test@example.com",
                Massege = "Hello there"
            };
            _emailServiceMock
                .Setup(s => s.SendEmailAsync(It.IsAny<Emaildto>()))
                .ReturnsAsync("Success");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            //Assert.Equal("Email sent successfully", result.Message);
        }

        [Fact]
        public async Task Handle_EmailSendingFails_ReturnsBadRequest()
        {
            // Arrange
            var request = new SendEmailCommand("test@example.com", "Hello there")
            {
                Email = "test@example.com",
                Massege = "Hello there"
            };
            _emailServiceMock
                .Setup(s => s.SendEmailAsync(It.IsAny<Emaildto>()))
                .ReturnsAsync("Failed");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Failed to send email", result.Message);
        }

        [Fact]
        public async Task Handle_BuildsEmailDtoWithCorrectValues()
        {
            // Arrange
            var request = new SendEmailCommand("test@example.com", "Hello there")
            {
                Email = "someone@example.com",
                Massege = "Test message body"
            };

            Emaildto capturedDto = null;
            _emailServiceMock
                .Setup(s => s.SendEmailAsync(It.IsAny<Emaildto>()))
                .Callback<Emaildto>(dto => capturedDto = dto)
                .ReturnsAsync("Success");

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedDto);
            Assert.Equal("someone@example.com", capturedDto.Email);

        }

        [Fact]
        public async Task Handle_EmailServiceCalledExactlyOnce()
        {
            // Arrange
            var request = new SendEmailCommand("test@example.com", "Hello there")
            {
                Email = "test@example.com",
                Massege = "Hello there"
            };
            _emailServiceMock
                .Setup(s => s.SendEmailAsync(It.IsAny<Emaildto>()))
                .ReturnsAsync("Success");

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _emailServiceMock.Verify(s => s.SendEmailAsync(It.IsAny<Emaildto>()), Times.Once);
        }
    }
}