
using Moq;
using Osta.Core.Feature.Authentication.Command.Handler;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Data.Helper;
using Osta.Identity.Authentication;

namespace Osta.Test.AuthenticationTesting.AuthenticationCommandTesting
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationMock;
        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _authenticationMock = new Mock<IAuthenticationService>();

            _handler = new RefreshTokenCommandHandler(
                _authenticationMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRefreshTokenIsValid()
        {
            // Arrange
            var refreshToken = "refresh-token-123";
            var token = "access-token-123";

            var request = new RefreshTokenCommand(
                refreshToken,
                token);

            var jwtResponse = new JWTAuthResponse();

            _authenticationMock
                .Setup(x => x.GetRefreshToken(
                    refreshToken,
                    token))
                .ReturnsAsync(jwtResponse);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Same(
                jwtResponse,
                result.Data);

            _authenticationMock.Verify(
                x => x.GetRefreshToken(
                    refreshToken,
                    token),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectTokensToAuthenticationService()
        {
            // Arrange
            var request = new RefreshTokenCommand(
                "my-refresh-token",
                "my-access-token");

            var jwtResponse = new JWTAuthResponse();

            _authenticationMock
                .Setup(x => x.GetRefreshToken(
                    "my-refresh-token",
                    "my-access-token"))
                .ReturnsAsync(jwtResponse);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authenticationMock.Verify(
                x => x.GetRefreshToken(
                    "my-refresh-token",
                    "my-access-token"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSameJwtResponse_WhenAuthenticationServiceReturnsResponse()
        {
            // Arrange
            var request = new RefreshTokenCommand(
                "refresh-123",
                "token-123");

            var expectedResponse = new JWTAuthResponse();

            _authenticationMock
                .Setup(x => x.GetRefreshToken(
                    request.RefreshToken,
                    request.Token))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Same(
                expectedResponse,
                result.Data);

            _authenticationMock.Verify(
                x => x.GetRefreshToken(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenGetRefreshTokenThrows()
        {
            // Arrange
            var request = new RefreshTokenCommand(
                "refresh-123",
                "token-123");

            _authenticationMock
                .Setup(x => x.GetRefreshToken(
                    request.RefreshToken,
                    request.Token))
                .ThrowsAsync(
                    new Exception("Invalid refresh token."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(
                    request,
                    CancellationToken.None));

            Assert.Equal(
                "Invalid refresh token.",
                exception.Message);

            _authenticationMock.Verify(
                x => x.GetRefreshToken(
                    request.RefreshToken,
                    request.Token),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallAuthenticationServiceOnlyOnce()
        {
            // Arrange
            var request = new RefreshTokenCommand(
                "refresh-token",
                "access-token");

            var jwtResponse = new JWTAuthResponse();

            _authenticationMock
                .Setup(x => x.GetRefreshToken(
                    request.RefreshToken,
                    request.Token))
                .ReturnsAsync(jwtResponse);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authenticationMock.Verify(
                x => x.GetRefreshToken(
                    request.RefreshToken,
                    request.Token),
                Times.Once);
        }
    }
}
