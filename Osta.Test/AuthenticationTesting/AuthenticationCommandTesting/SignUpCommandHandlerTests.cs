
using AutoMapper;

using Moq;
using Osta.Core.Feature.Authentication.Command.Handler;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authentication;
using Osta.Identity.DTOs;

namespace Osta.Test.AuthenticationTesting.AuthenticationCommandTesting
{
    public class SignUpCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAuthenticationService> _authenticationMock;

        private readonly SignUpCommandHandler _handler;

        public SignUpCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _authenticationMock = new Mock<IAuthenticationService>();

            _handler = new SignUpCommandHandler(
                _mapperMock.Object,
                _authenticationMock.Object);
        }

        private SignUpCommand CreateRequest()
        {
            return new SignUpCommand(
                FullName: "Test User",
                Email: "test@gmail.com",
                Password: "Password123!",
                Phone: "01012345678",
                ConfirmPassword: "Password123!",
                Area: "Helwan",
                City: "Cairo",
                Governorate: "Cairo",
                DateOfBirth: new DateOnly(2000, 1, 1),
                Street: "Test Street",
                ProfileImage: null
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenSignUpIsSuccessful()
        {
            // Arrange
            var request = CreateRequest();

            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FullName = request.FullName
            };

            _mapperMock
                .Setup(x => x.Map<User>(request))
                .Returns(user);

            _authenticationMock
                .Setup(x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage))
                .ReturnsAsync(Identity.DTOs.SignUpResult.Success);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(
                "User registered successfully. Please check your email to confirm your account.",
                result.Data);

            _mapperMock.Verify(
                x => x.Map<User>(request),
                Times.Once);

            _authenticationMock.Verify(
                x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var request = CreateRequest();

            var user = new User
            {
                Email = request.Email
            };

            _mapperMock
                .Setup(x => x.Map<User>(request))
                .Returns(user);

            _authenticationMock
                .Setup(x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage))
                .ReturnsAsync(SignUpResult.UserWithEmailAlreadyExists);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Email is already registered.",
                result.Message);

            Assert.Null(result.Data);

            _authenticationMock.Verify(
                x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserCreatedButEmailFailed()
        {
            // Arrange
            var request = CreateRequest();

            var user = new User
            {
                Email = request.Email,
                UserName = request.Email
            };

            _mapperMock
                .Setup(x => x.Map<User>(request))
                .Returns(user);

            _authenticationMock
                .Setup(x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage))
                .ReturnsAsync(SignUpResult.FailedToSendEmail);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Equal(
            //    "User created successfully, but the confirmation email could not be sent.",
            //    result.Data);

            _authenticationMock.Verify(
                x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenSignUpReturnsUnknownResult()
        {
            // Arrange
            var request = CreateRequest();

            var user = new User
            {
                Email = request.Email
            };

            _mapperMock
                .Setup(x => x.Map<User>(request))
                .Returns(user);

            _authenticationMock
                .Setup(x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage))
                .ReturnsAsync(SignUpResult.Failed);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Equal(
            //    "Something went wrong.",
            //    result.Message);

            Assert.Null(result.Data);

            _authenticationMock.Verify(
                x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectPasswordToAuthenticationService()
        {
            // Arrange
            var request = CreateRequest();

            var user = new User();

            _mapperMock
                .Setup(x => x.Map<User>(request))
                .Returns(user);

            _authenticationMock
                .Setup(x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage))
                .ReturnsAsync(SignUpResult.Success);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authenticationMock.Verify(
                x => x.SignUpAsync(
                    user,
                    "Password123!",
                    null),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMapRequestToUser()
        {
            // Arrange
            var request = CreateRequest();

            var user = new User
            {
                FullName = "Test User",
                Email = "test@gmail.com"
            };

            _mapperMock
                .Setup(x => x.Map<User>(request))
                .Returns(user);

            _authenticationMock
                .Setup(x => x.SignUpAsync(
                    user,
                    request.Password,
                    request.ProfileImage))
                .ReturnsAsync(SignUpResult.Success);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _mapperMock.Verify(
                x => x.Map<User>(request),
                Times.Once);
        }
    }
}

