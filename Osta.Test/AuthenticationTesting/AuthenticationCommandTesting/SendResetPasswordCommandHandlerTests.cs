
using Moq;
using Osta.Core.Feature.Authentication.Command.Handler;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Identity.Authentication;
using Osta.Identity.DTOs;

namespace Osta.Test.AuthenticationTesting.AuthenticationCommandTesting
{
    public class SendResetPasswordCommandHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationMock;
        private readonly SendResetPasswordCommandHandler _handler;

        public SendResetPasswordCommandHandlerTests()
        {
            _authenticationMock = new Mock<IAuthenticationService>();

            _handler = new SendResetPasswordCommandHandler(
                _authenticationMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            var request = new SendResetPasswordCommand(
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.SendResetPasswordCode(request.Email))
                .ReturnsAsync(SendResetPasswordCodeResult.UserNotFound);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "User not found.",
                result.Message);

            Assert.Null(result.Data);

            _authenticationMock.Verify(
                x => x.SendResetPasswordCode(request.Email),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenCodeIsWrong()
        {
            // Arrange
            var request = new SendResetPasswordCommand(
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.SendResetPasswordCode(request.Email))
                .ReturnsAsync(SendResetPasswordCodeResult.InvalidInput);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Equal(
            //    "Invalid code.",
            //    result.Message);

            Assert.Null(result.Data);

            _authenticationMock.Verify(
                x => x.SendResetPasswordCode(request.Email),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenResetPasswordCodeSentSuccessfully()
        {
            // Arrange
            var request = new SendResetPasswordCommand(
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.SendResetPasswordCode(request.Email))
                .ReturnsAsync(SendResetPasswordCodeResult.Success);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Reset password code sent successfully.",
                result.Data);

            _authenticationMock.Verify(
                x => x.SendResetPasswordCode(request.Email),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenAuthenticationServiceReturnsUnknownResult()
        {
            // Arrange
            var request = new SendResetPasswordCommand(
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.SendResetPasswordCode(request.Email))
                .ReturnsAsync(SendResetPasswordCodeResult.Failed);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Failed to send reset password code.",
                result.Message);

            Assert.Null(result.Data);

            _authenticationMock.Verify(
                x => x.SendResetPasswordCode(request.Email),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectEmailToAuthenticationService()
        {
            // Arrange
            var email = "user@example.com";

            var request = new SendResetPasswordCommand(email);

            _authenticationMock
                .Setup(x => x.SendResetPasswordCode(email))
                .ReturnsAsync(SendResetPasswordCodeResult.Success);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authenticationMock.Verify(
                x => x.SendResetPasswordCode(
                    "user@example.com"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenAuthenticationServiceThrows()
        {
            // Arrange
            var request = new SendResetPasswordCommand(
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.SendResetPasswordCode(request.Email))
                .ThrowsAsync(
                    new Exception("Authentication service error."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(
                    request,
                    CancellationToken.None));

            Assert.Equal(
                "Authentication service error.",
                exception.Message);

            _authenticationMock.Verify(
                x => x.SendResetPasswordCode(request.Email),
                Times.Once);
        }
    }
}

