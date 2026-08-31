
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Authentication.Command.Handler;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Data.Entities.Identity;
using Osta.Data.Helper;
using Osta.Identity.Authentication;

namespace Osta.Test.AuthenticationTesting.AuthenticationCommandTesting
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IAuthenticationService> _authenticationMock;

        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null!,
                null!,
                null!,
                null!);

            _authenticationMock =
                new Mock<IAuthenticationService>();

            _handler = new LoginCommandHandler(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _authenticationMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginCommand(
                "test@gmail.com",
                "Password123");

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                request.Email,
                result.Message);

            Assert.Null(result.Data);

            _userManagerMock.Verify(
                x => x.FindByEmailAsync(request.Email),
                Times.Once);

            _signInManagerMock.Verify(
                x => x.CheckPasswordSignInAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()),
                Times.Never);

            _authenticationMock.Verify(
                x => x.GenerateJWToken(
                    It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenEmailIsNotConfirmed()
        {
            // Arrange
            var request = new LoginCommand(
                "test@gmail.com",
                "Password123");

            var user = new User
            {
                Id = "user-1",
                Email = request.Email,
                EmailConfirmed = false
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Null(result.Data);

            _userManagerMock.Verify(
                x => x.FindByEmailAsync(request.Email),
                Times.Once);

            _signInManagerMock.Verify(
                x => x.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    false),
                Times.Once);

            _authenticationMock.Verify(
                x => x.GenerateJWToken(
                    It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new LoginCommand(
                "test@gmail.com",
                "WrongPassword");

            var user = new User
            {
                Id = "user-1",
                Email = request.Email,
                EmailConfirmed = true
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Password is incorrect.",
                result.Message);

            Assert.Null(result.Data);

            _authenticationMock.Verify(
                x => x.GenerateJWToken(
                    It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldGenerateToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var request = new LoginCommand(
                "test@gmail.com",
                "Password123");

            var user = new User
            {
                Id = "user-1",
                Email = request.Email,
                EmailConfirmed = true
            };

            var jwtResponse = new JWTAuthResponse();

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    false))
                .ReturnsAsync(SignInResult.Success);

            _authenticationMock
                .Setup(x => x.GenerateJWToken(user))
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

            _userManagerMock.Verify(
                x => x.FindByEmailAsync(request.Email),
                Times.Once);

            _signInManagerMock.Verify(
                x => x.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    false),
                Times.Once);

            _authenticationMock.Verify(
                x => x.GenerateJWToken(user),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotGenerateToken_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new LoginCommand(
                "test@gmail.com",
                "WrongPassword");

            var user = new User
            {
                Id = "user-1",
                Email = request.Email,
                EmailConfirmed = true
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authenticationMock.Verify(
                x => x.GenerateJWToken(
                    It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUseCorrectEmailAndPassword()
        {
            // Arrange
            var request = new LoginCommand(
                "mokhtar@gmail.com",
                "MyPassword123");

            var user = new User
            {
                Id = "user-10",
                Email = request.Email,
                EmailConfirmed = true
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(
                    request.Email))
                .ReturnsAsync(user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    false))
                .ReturnsAsync(SignInResult.Success);

            _authenticationMock
                .Setup(x => x.GenerateJWToken(user))
                .ReturnsAsync(new JWTAuthResponse());

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _userManagerMock.Verify(
                x => x.FindByEmailAsync(
                    "mokhtar@gmail.com"),
                Times.Once);

            _signInManagerMock.Verify(
                x => x.CheckPasswordSignInAsync(
                    user,
                    "MyPassword123",
                    false),
                Times.Once);
        }
    }
}

