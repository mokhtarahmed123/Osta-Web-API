using AutoMapper;
using Moq;
using Osta.Core.Feature.Category.Command.Handler;
using Osta.Core.Feature.Category.Command.Model;
using Osta.Data.Entities.Services;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;
using System.Net;

namespace Osta.Test.CategoryTesting.Commands
{
    public class DeleteCategoryCommandHandlerTest
    {
        [Fact]
        public async Task Handle_ShouldDeleteCategorySuccessfully()
        {

            var mockServiceService = new Mock<IServiceService>();

            var mockCategoryService = new Mock<ICategoryService>();

            var mockMapper = new Mock<IMapper>();

            var mockLogger = new Mock<ILoggerService>();


            mockCategoryService
    .Setup(x => x.GetCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new Category
        {
            Id = 6,
            ImageUrl = null,
            Name = "Home Cleaning",
            IsActive = true,

        });

            mockServiceService
                .Setup(x => x.DoesCategoryHaveServiceAsync(6, It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<bool>());

            mockCategoryService
      .Setup(x => x.DeleteCategoryAsync(6, It.IsAny<CancellationToken>())).
      Returns(Task.CompletedTask);

            var handler = new DeleteCategoryCommandHandler(mockMapper.Object, mockCategoryService.Object, mockLogger.Object, mockServiceService.Object);



            var result = await handler.Handle(new DeleteCategoryCommand(Id: 6), CancellationToken.None);



            Assert.True(result.Succeeded);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            mockCategoryService.Verify(
                x => x.DeleteCategoryAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenDeleteFails()
        {
            var mockCategoryService = new Mock<ICategoryService>(); var mockServiceService = new Mock<IServiceService>();

            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();
            mockCategoryService
.Setup(x => x.GetCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
.ReturnsAsync(new Category
{
    Id = 6,
    ImageUrl = null,
    Name = "Home Cleaning",
    IsActive = true,

});

            mockServiceService
                .Setup(x => x.DoesCategoryHaveServiceAsync(6, It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<bool>());

            mockCategoryService
      .Setup(x => x.DeleteCategoryAsync(6, It.IsAny<CancellationToken>())).
      Returns(Task.CompletedTask);




            mockCategoryService
                .Setup(x => x.DeleteCategoryAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Delete failed"));
            var handler = new DeleteCategoryCommandHandler(mockMapper.Object, mockCategoryService.Object, mockLogger.Object, mockServiceService.Object);



            var result = await handler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);


            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            mockCategoryService.Verify(
                x => x.DeleteCategoryAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        }
    }
}
