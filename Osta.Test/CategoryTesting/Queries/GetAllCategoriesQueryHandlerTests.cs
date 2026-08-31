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
    public class GetAllCategoriesQueryHandlerTests
    {

        [Fact]
        public async Task Handle_ShouldReturnAllCategories()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var categories = new List<Category>
    {
        new Category
        {
            Id = 1,
            Name = "Category 1",
            ImageUrl = "image1.jpg",
            IsActive = true
        },
        new Category
        {
            Id = 2,
            Name = "Category 2",
            ImageUrl = "image2.jpg",
            IsActive = true
        }
    };

            var response = new List<GetAllCategoryResult>
    {
        new()
        {
            Id = 1,
            Name = "Category 1",
            ImageUrl = "image1.jpg",
            IsActive = true
        },
        new()
        {
            Id = 2,
            Name = "Category 2",
            ImageUrl = "image2.jpg",
            IsActive = true
        }
    };

            mockCategoryService
                .Setup(x => x.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            mockMapper
                .Setup(x => x.Map<List<GetAllCategoryResult>>(categories))
                .Returns(response);

            var handler = new GetAllCategoryQueryHandler(
                mockMapper.Object,
                mockCategoryService.Object,
                mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllCategoryQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());

            Assert.Contains(result.Data, x => x.Id == 1 && x.Name == "Category 1");
            Assert.Contains(result.Data, x => x.Id == 2 && x.Name == "Category 2");

            mockCategoryService.Verify(
                x => x.GetAllCategoriesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(
                x => x.Map<List<GetAllCategoryResult>>(It.IsAny<List<Category>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();
            var categories = new List<Category>();
            var response = new List<GetAllCategoryResult>();
            mockCategoryService
                .Setup(x => x.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);
            mockMapper
                .Setup(x => x.Map<List<GetAllCategoryResult>>(categories))
                .Returns(response);
            var handler = new GetAllCategoryQueryHandler(
                mockMapper.Object,
                mockCategoryService.Object,
                mockLogger.Object);
            // Act
            var result = await handler.Handle(new GetAllCategoryQuery(), CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
            mockCategoryService.Verify(
                x => x.GetAllCategoriesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
            mockMapper.Verify(
                x => x.Map<List<GetAllCategoryResult>>(It.IsAny<IEnumerable<Category>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallCategoryServiceOnce()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategoryService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();
            mockCategoryService
                .Setup(x => x.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category>());



            mockMapper
                .Setup(x => x.Map<List<GetAllCategoryResult>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(new List<GetAllCategoryResult>());


            var handler = new GetAllCategoryQueryHandler(
                mockMapper.Object,
                mockCategoryService.Object,
                mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllCategoryQuery(), CancellationToken.None);

            // Assert
            mockCategoryService.Verify(
             x => x.GetAllCategoriesAsync(It.IsAny<CancellationToken>()),
             Times.Once);
        }




    }
}
