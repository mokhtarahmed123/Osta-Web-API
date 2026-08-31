
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Authentication.Query.Handler;
using Osta.Core.Feature.Authentication.Query.Model;
using Osta.Core.Feature.Authentication.Query.Model.AuthModel;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authentication;
using Osta.Identity.DTOs;
using Osta.SharedKernel.Identity;

namespace Osta.Test.AuthenticationTesting.AuthenticationQueryTesting
{
    public class AuthQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<IAuthenticationService> _authenticationMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly AuthQueryHandler _handler;

        public AuthQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();

            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null!, null!, null!, null!, null!, null!, null!, null!);

            _roleManagerMock = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(),
                null!, null!, null!, null!);

            _authenticationMock = new Mock<IAuthenticationService>();

            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new AuthQueryHandler(
                _mapperMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _authenticationMock.Object,
                _currentUserServiceMock.Object);
        }



        [Fact]
        public async Task ConfirmResetPassword_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            var request = new ConfirmResetPasswordQuery(
                "123456",
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.ConfirmResetPassword(
                    request.Code,
                    request.Email))
                .ReturnsAsync(ConfirmResetPasswordResult.UserNotFound);

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
                x => x.ConfirmResetPassword(
                    request.Code,
                    request.Email),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmResetPassword_ShouldReturnBadRequest_WhenErrorInUpdating()
        {

            var request = new ConfirmResetPasswordQuery(
                "123456",
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.ConfirmResetPassword(
                    request.Code,
                    request.Email))
                .ReturnsAsync(ConfirmResetPasswordResult.ErrorInUpdating);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.Equal(
                "Failed to update password.",
                result.Message);

            Assert.Null(result.Data);

            _authenticationMock.Verify(
                x => x.ConfirmResetPassword(
                    request.Code,
                    request.Email),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmResetPassword_ShouldReturnBadRequest_WhenFailedToSendEmail()
        {

            var request = new ConfirmResetPasswordQuery(
                "123456",
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.ConfirmResetPassword(
                    request.Code,
                    request.Email))
                .ReturnsAsync(ConfirmResetPasswordResult.FailedToSendEmail);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.Equal(
                "Failed to send email.",
                result.Message);

            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ConfirmResetPassword_ShouldReturnSuccess_WhenCodeIsCorrect()
        {

            var request = new ConfirmResetPasswordQuery(
                "123456",
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.ConfirmResetPassword(
                    request.Code,
                    request.Email))
                .ReturnsAsync(ConfirmResetPasswordResult.Success);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Equal(
                "Correct code.",
                result.Data);

            _authenticationMock.Verify(
                x => x.ConfirmResetPassword(
                    request.Code,
                    request.Email),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmResetPassword_ShouldReturnBadRequest_WhenResultIsUnknown()
        {

            var request = new ConfirmResetPasswordQuery(
                "123456",
                "test@gmail.com");

            _authenticationMock
                .Setup(x => x.ConfirmResetPassword(
                    request.Code,
                    request.Email))
                .ReturnsAsync(ConfirmResetPasswordResult.FailedToSendEmail);


            var result = await _handler.Handle(
                request,
                CancellationToken.None);



            Assert.Null(result.Data);

            _authenticationMock.Verify(
                x => x.ConfirmResetPassword(
                    request.Code,
                    request.Email),
                Times.Once);
        }



        [Fact]
        public async Task MyProfile_ShouldReturnNotFound_WhenUserIdIsNull()
        {

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string?)null);

            var request = new MyProfileQuery();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Equal(
                "User not found.",
                result.Message);

            Assert.Null(result.Data);

            _userManagerMock.Verify(
                x => x.FindByIdAsync(
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task MyProfile_ShouldReturnNotFound_WhenUserDoesNotExist()
        {

            var userId = "user-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var request = new MyProfileQuery();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);

            Assert.Equal(
                "User not found.",
                result.Message);

            Assert.Null(result.Data);

            _userManagerMock.Verify(
                x => x.FindByIdAsync(userId),
                Times.Once);
        }

        [Fact]
        public async Task MyProfile_ShouldReturnSuccess_WhenUserExists()
        {

            var userId = "user-123";

            var user = new User
            {
                Id = userId,
                FullName = "Test User",
                Email = "test@gmail.com",
                PhoneNumber = "01012345678"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var request = new MyProfileQuery();


            var result = await _handler.Handle(
                request,
                CancellationToken.None);


            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(
                user.Id,
                result.Data.Id);

            Assert.Equal(
                user.FullName,
                result.Data.FullName);

            Assert.Equal(
                user.Email,
                result.Data.Email);

            Assert.Equal(
                user.PhoneNumber,
                result.Data.PhoneNumber);

            _userManagerMock.Verify(
                x => x.FindByIdAsync(userId),
                Times.Once);
        }

        [Fact]
        public async Task MyProfile_ShouldMapAllUserPropertiesCorrectly()
        {

            var user = new User
            {
                Id = "user-456",
                FullName = "Mohamed Ahmed",
                Email = "mohamed@gmail.com",
                PhoneNumber = "01111111111"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(user.Id);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);


            var result = await _handler.Handle(
                new MyProfileQuery(),
                CancellationToken.None);


            Assert.Equal("user-456", result.Data!.Id);
            Assert.Equal("Mohamed Ahmed", result.Data.FullName);
            Assert.Equal("mohamed@gmail.com", result.Data.Email);
            Assert.Equal("01111111111", result.Data.PhoneNumber);
        }
    }
}

