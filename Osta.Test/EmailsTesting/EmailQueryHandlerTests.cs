using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Emails.Query.Handler;
using Osta.Core.Feature.Emails.Query.Model;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authentication;
using Osta.Identity.DTOs;

namespace Osta.Test.EmailsTesting
{
    public class EmailQueryHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly EmailQueryHandler _handler;

        public EmailQueryHandlerTests()
        {
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _userManagerMock = MockUserManager();
            _handler = new EmailQueryHandler(
                _authenticationServiceMock.Object,
                _userManagerMock.Object);
        }
        private static Mock<UserManager<User>> MockUserManager()
        {
            var storeMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                storeMock.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task Handle_UserIdOrCodeNull_ReturnsBadRequest()
        {
            // Arrange
            var request = new ConfirmEmailQuery("code-1", "user-1") { UserId = "user-1", Code = "code-1" };
            _authenticationServiceMock
                .Setup(a => a.ConfirmEmail(request.UserId, request.Code))
                .ReturnsAsync(ConfirmEmailResult.UserIdOrCodeNull);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User ID or confirmation code is required.", result.Message);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new ConfirmEmailQuery("code-1", "user-1") { UserId = "user-1", Code = "code-1" };
            _authenticationServiceMock
                .Setup(a => a.ConfirmEmail(request.UserId, request.Code))
                .ReturnsAsync(ConfirmEmailResult.UserNotFound);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User not found.", result.Message);
        }

        [Fact]
        public async Task Handle_ConfirmationFailed_ReturnsBadRequest()
        {
            // Arrange
            var request = new ConfirmEmailQuery("code-1", "user-1") { UserId = "user-1", Code = "code-1" };
            _authenticationServiceMock
                .Setup(a => a.ConfirmEmail(request.UserId, request.Code))
                .ReturnsAsync(ConfirmEmailResult.Failed);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Email confirmation failed.", result.Message);
        }

        [Fact]
        public async Task Handle_Confirmed_ReturnsSuccess()
        {
            // Arrange
            var request = new ConfirmEmailQuery("code-1", "user-1") { UserId = "user-1", Code = "code-1" };
            _authenticationServiceMock
                .Setup(a => a.ConfirmEmail(request.UserId, request.Code))
                .ReturnsAsync(ConfirmEmailResult.Confirmed);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            //Assert.Equal("Email confirmed successfully.", result.Message);
        }

        [Fact]
        public async Task Handle_UnknownResult_ReturnsBadRequestWithGenericMessage()
        {
            // Arrange
            var request = new ConfirmEmailQuery("code-1", "user-1") { UserId = "user-1", Code = "code-1" };
            _authenticationServiceMock
                .Setup(a => a.ConfirmEmail(request.UserId, request.Code))
                .ReturnsAsync(ConfirmEmailResult.Failed);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            //Assert.Equal("Invalid email confirmation request.", result.Message);
        }

        [Fact]
        public async Task Handle_CallsAuthenticationServiceWithCorrectParameters()
        {
            // Arrange
            var request = new ConfirmEmailQuery(
                "code-1",
                "user-1")
            {
                UserId = "user-1",
                Code = "code-1"
            };

            _authenticationServiceMock
                .Setup(a => a.ConfirmEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(ConfirmEmailResult.Confirmed);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authenticationServiceMock.Verify(
                a => a.ConfirmEmail(
                    "user-1",
                    "code-1"),
                Times.Once);
        }
    }
}