
using Moq;
using Osta.Core.Feature.Authentication.Command.Handler;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Identity.Authentication;
using Osta.Identity.DTOs;

namespace Osta.Test.AuthenticationTesting.AuthenticationCommandTesting
{
    public class ResetPasswordCommandHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationMock;
        private readonly ResetPasswordCommandHandler _handler;

        public ResetPasswordCommandHandlerTests()
        {
            _authenticationMock = new Mock<IAuthenticationService>();

            _handler = new ResetPasswordCommandHandler(
                _authenticationMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenPasswordIsTooShort()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "Ab1!",
                "Ab1!");

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "PasswordTooWeak",
                result.Message);

            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenPasswordHasNoUppercase()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "password123!",
                "password123!");

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "PasswordTooWeak",
                result.Message);

            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenPasswordHasNoLowercase()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "PASSWORD123!",
                "PASSWORD123!");

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "PasswordTooWeak",
                result.Message);

            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenPasswordHasNoDigit()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "Password!",
                "Password!");

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "PasswordTooWeak",
                result.Message);

            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenPasswordHasNoSymbolOrPunctuation()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "Password123",
                "Password123");

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "PasswordTooWeak",
                result.Message);

            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "Password123!",
                "Password123!");

            _authenticationMock
                .Setup(x => x.ResetPasswordCode(
                    request.Email,
                    request.Password))
                .ReturnsAsync(ResetPasswordResult.UserNotFound);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "User not found.",
                result.Message);

            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    request.Email,
                    request.Password),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenResetPasswordFails()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "Password123!",
                "Password123!");

            _authenticationMock
                .Setup(x => x.ResetPasswordCode(
                    request.Email,
                    request.Password))
                .ReturnsAsync(ResetPasswordResult.Failed);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "Failed to reset password.",
                result.Message);

            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    request.Email,
                    request.Password),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPasswordResetSuccessfully()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "Password123!",
                "Password123!");

            _authenticationMock
                .Setup(x => x.ResetPasswordCode(
                    request.Email,
                    request.Password))
                .ReturnsAsync(ResetPasswordResult.Success);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Password reset successfully. You can login now.",
                result.Data);

            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    request.Email,
                    request.Password),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenAuthenticationReturnsUnknownResult()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "Password123!",
                "Password123!");

            _authenticationMock
                .Setup(x => x.ResetPasswordCode(
                    request.Email,
                    request.Password))
                .ReturnsAsync(ResetPasswordResult.Failed);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "Failed to reset password.",
                result.Message);

            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    request.Email,
                    request.Password),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectEmailAndPassword()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "user@example.com",
                "StrongPass123!",
                "StrongPass123!");

            _authenticationMock
                .Setup(x => x.ResetPasswordCode(
                    request.Email,
                    request.Password))
                .ReturnsAsync(ResetPasswordResult.Success);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    "user@example.com",
                    "StrongPass123!"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotCallAuthenticationService_WhenPasswordIsInvalid()
        {
            // Arrange
            var request = new ResetPasswordCommand(
                "test@gmail.com",
                "weak",
                "weak");

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authenticationMock.Verify(
                x => x.ResetPasswordCode(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }
    }
}
