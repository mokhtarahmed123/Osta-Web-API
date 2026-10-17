using AutoMapper;
using Moq;
using Osta.Core.Feature.Category.Query.Handler;
using Osta.Core.Feature.Category.Query.Model;
using Osta.Core.Feature.Category.Query.Result;
using Osta.Data.Entities.Services;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.CategoryTesting.Queries
{
    public class GetCategoryByIdQueryHandlerTest
    {


        [Fact]

        public async Task Handle_ShouldReturnCategory_WhenIdExists()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var category = new Category
            {
                Id = 1,
                Name = "Test Category"
            };

            var dto = new GetCategoryByIdResult
            {
                Id = 1,
                Name = "Test Category"
            };

            mockCategoryService
                .Setup(x => x.GetCategoryAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            mockMapper
                .Setup(x => x.Map<GetCategoryByIdResult>(category))
                .Returns(dto);

            var handler = new GetCategoryByIdQueryHandler(
                mockMapper.Object,
                mockCategoryService.Object,
                mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetCategoryByIdQuery(1), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("Test Category", result.Data.Name);

            mockCategoryService.Verify(
                x => x.GetCategoryAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(
                x => x.Map<GetCategoryByIdResult>(It.IsAny<Category>()),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var category = new Category
            {
                Id = 1,
                Name = "Test Category"
            };

            var dto = new GetCategoryByIdResult
            {
                Id = 1,
                Name = "Test Category"
            };

            mockCategoryService
           .Setup(x => x.GetCategoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

            mockMapper
                .Setup(x => x.Map<GetCategoryByIdResult>(It.IsAny<Category>()))
                .Returns(dto);
            var handler = new GetCategoryByIdQueryHandler(
           mockMapper.Object,
            mockCategoryService.Object,
            mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetCategoryByIdQuery(1), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Data);
            Assert.Equal("Category not found.", result.Message);

            mockCategoryService.Verify(
                x => x.GetCategoryAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(
                x => x.Map<GetCategoryByIdResult>(It.IsAny<Category>()),
                Times.Never);
        }
    }
}
