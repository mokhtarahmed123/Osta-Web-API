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
    public class AddCategoryCommandHandlerTest
    {
        [Fact]
        public async Task Handle_ShouldCreateCategorySuccessfully()
        {

            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new AddCategoryCommand
            {
                Name = "Electrical",
                Image = null,
                IsActive = true

            };

            var category = new Category
            {
                Id = 1,
                Name = "Electrical",
                ImageUrl = null,
                IsActive = true
            };

            mockMapper
                .Setup(x => x.Map<Category>(command))
                .Returns(category);

            mockCategoryService
                .Setup(x => x.AddCategoryAsync(
                    It.IsAny<Category>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new AddCategoryCommandHandler(
                mockMapper.Object,
                mockCategoryService.Object,
                mockLogger.Object);


            var result = await handler.Handle(command, CancellationToken.None);


            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);


            mockMapper.Verify(
                x => x.Map<Category>(command),
                Times.Once);

            mockCategoryService.Verify(
                x => x.AddCategoryAsync(
                    It.IsAny<Category>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallServiceOnce()
        {

            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();
            var command = new AddCategoryCommand
            {
                Name = "Electrical",
                Image = null,
                IsActive = true
            };
            var category = new Category
            {
                Id = 1,
                Name = "Electrical",
                ImageUrl = null,
                IsActive = true
            };
            mockMapper
                .Setup(x => x.Map<Category>(command))
                .Returns(category);
            mockCategoryService
                .Setup(x => x.AddCategoryAsync(
                    It.IsAny<Category>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var handler = new AddCategoryCommandHandler(
                mockMapper.Object,
                mockCategoryService.Object,
                mockLogger.Object);

            await handler.Handle(command, CancellationToken.None);

            mockCategoryService.Verify(
                x => x.AddCategoryAsync(
                    It.IsAny<Category>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {

            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();
            var command = new AddCategoryCommand
            {
                Name = "Electrical",
                Image = null,
                IsActive = true
            };
            var category = new Category
            {
                Id = 1,
                Name = "Electrical",
                ImageUrl = null,
                IsActive = true
            };
            mockMapper
                .Setup(x => x.Map<Category>(command))
                .Returns(category);
            mockCategoryService
                .Setup(x => x.AddCategoryAsync(
                    It.IsAny<Category>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));
            var handler = new AddCategoryCommandHandler(
                mockMapper.Object,
                mockCategoryService.Object,
                mockLogger.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);


        }


    }
}