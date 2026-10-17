using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Osta.Core.Feature.Category.Command.Handler;
using Osta.Core.Feature.Category.Command.Model;
using Osta.Data.Entities.Services;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;
using System.Net;

namespace Osta.Test.CategoryTesting.Commands
{
    public class UpdateCategoryCommandHandlerTest
    {
        [Fact]
        public async Task Handle_ShouldUpdateCategorySuccessfully()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();
            var mockServiceService = new Mock<IServiceService>();
            var command = new UpdateCategoryCommand(1)
            {
                Id = 1,
                Name = "Updated Category",
                Image = null,
                IsActive = true
            };

            var category = new Category
            {
                Id = 1,
                Name = "Updated Category",
                ImageUrl = null,
                IsActive = true
            };

            mockMapper
                .Setup(x => x.Map<Category>(command))
                .Returns(category);




            mockCategoryService
        .Setup(x => x.GetCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new Category
        {
            Id = 1,
            ImageUrl = "http://localhost:5083/Images/Category/0/d0f084d649e14432add56326bf0ff1e4.webp",
            Name = "Plumbing",
            IsActive = true,

        });




            mockCategoryService
                .Setup(x => x.UpdateCategoryAsync(
                    command.Id,
                    It.IsAny<Category>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdateCategoryCommandHandler(
                mockMapper.Object,
                mockCategoryService.Object,
                mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            mockMapper.Verify(
                x => x.Map<Category>(command),
                Times.Once);

            mockCategoryService.Verify(
                x => x.UpdateCategoryAsync(
                    command.Id,
                    It.IsAny<Category>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenUpdateFails()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new UpdateCategoryCommand(1)
            {
                Name = "Updated Category"
            };

            mockMapper
                .Setup(x => x.Map<Category>(It.IsAny<UpdateCategoryCommand>()))
                .Returns(new Category());

            mockCategoryService
                .Setup(x => x.UpdateCategoryAsync(
                    It.IsAny<int>(),
                    It.IsAny<Category>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            var handler = new UpdateCategoryCommandHandler(
                mockMapper.Object,
                mockCategoryService.Object,
                mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

    }
}
