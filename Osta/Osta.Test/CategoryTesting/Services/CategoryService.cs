using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Osta.Data.Entities.Services;
using Osta.Infrastructure.Abstract.ServicesAbstract;
using Osta.Infrastructure.Caching;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Service.ServicesServiceFolder;
using Osta.SharedKernel;
using Osta.SharedKernel.Logging;

namespace Osta.Test.CategoryTesting.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> categoryRepoMock;
        private readonly Mock<IFileService> imageUploadMock;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
        private readonly Mock<IWebHostEnvironment> envMock;
        private readonly Mock<ILoggerService> loggerServiceMock;
        private readonly Mock<ICacheService> cacheservicemock;
        private readonly CategoryService sut;

        private const string BaseUrl = "https://localhost";

        public CategoryServiceTests()
        {
            categoryRepoMock = new Mock<ICategoryRepository>();
            imageUploadMock = new Mock<IFileService>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            envMock = new Mock<IWebHostEnvironment>();
            loggerServiceMock = new Mock<ILoggerService>();
            cacheservicemock = new Mock<ICacheService>();


            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost");
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            sut = new CategoryService(
                categoryRepoMock.Object,
                imageUploadMock.Object,
                unitOfWorkMock.Object,
                httpContextAccessorMock.Object,
                envMock.Object,
                loggerServiceMock.Object, cacheservicemock.Object);
        }

        private static Mock<IFormFile> CreateFormFileMock(string fileName = "image.png")
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns(fileName);
            formFileMock.Setup(f => f.Length).Returns(1024);
            return formFileMock;
        }

        // ---------------------------------------------------------------
        // AddCategoryAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task ShouldUploadImage_WhenImageExists()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Plumbing" };
            var formFile = CreateFormFileMock().Object;
            var expectedImagePath = "/Images/Category/1/image.png";

            imageUploadMock
                .Setup(x => x.UploadImageAsync(formFile, $"Images/Category/{category.Id}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedImagePath);

            // Act
            await sut.AddCategoryAsync(category, formFile);

            // Assert
            imageUploadMock.Verify(
                x => x.UploadImageAsync(formFile, $"Images/Category/{category.Id}", It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.Equal(BaseUrl + expectedImagePath, category.ImageUrl);
        }

        [Fact]
        public async Task ShouldNotUploadImage_WhenImageIsNull()
        {
            // Arrange
            var category = new Category { Id = 2, Name = "Electrical" };

            // Act
            await sut.AddCategoryAsync(category, null);

            // Assert
            imageUploadMock.Verify(
                x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            Assert.Null(category.ImageUrl);
        }

        //[Fact]
        //public async Task ShouldDeleteUploadedImage_WhenDatabaseFails()
        //{
        //    // Arrange
        //    var category = new Category { Id = 3, Name = "Carpentry" };
        //    var formFile = CreateFormFileMock().Object;
        //    var expectedImagePath = "/Images/Category/3/image.png";

        //    imageUploadMock
        //        .Setup(x => x.UploadImageAsync(formFile, $"Images/Category/{category.Id}", It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(expectedImagePath);

        //    categoryRepoMock
        //        .Setup(x => x.AddAsync(category))
        //        .ThrowsAsync(new InvalidOperationException("DB failure"));

        //    // Act & Assert
        //    await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AddCategoryAsync(category, formFile));

        //    imageUploadMock.Verify(
        //        x => x.DeleteImage(BaseUrl + expectedImagePath, "CategoryImages"),
        //        Times.Once);
        //    unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        //}

        [Fact]
        public async Task ShouldSaveCategorySuccessfully()
        {
            // Arrange
            var category = new Category { Id = 4, Name = "Painting" };

            // Act
            await sut.AddCategoryAsync(category, null);

            // Assert
            categoryRepoMock.Verify(x => x.AddAsync(category, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            imageUploadMock.Verify(
                x => x.DeleteImage(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        // ---------------------------------------------------------------
        // DeleteCategoryAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task ShouldDeleteCategory()
        {
            // Arrange
            var category = new Category { Id = 5, Name = "Cleaning", ImageUrl = BaseUrl + "/Images/Category/5/old.png" };
            categoryRepoMock.Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);

            var transactionMock = new Mock<IDbContextTransaction>();
            unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);

            // Act
            await sut.DeleteCategoryAsync(category.Id);

            // Assert
            categoryRepoMock.Verify(x => x.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        //[Fact]
        //public async Task ShouldDeleteImage()
        //{
        //    // Arrange
        //    var imageUrl = BaseUrl + "/Images/Category/1/6c3c8a31280847bfb14013680387152f";
        //    var category = new Category { Id = 1, Name = "Plumbing", ImageUrl = imageUrl };
        //    categoryRepoMock.Setup(x => x.GetByIdAsync(category.Id)).ReturnsAsync(category);

        //    var transactionMock = new Mock<IDbContextTransaction>();
        //    unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);

        //    // Act
        //    await sut.DeleteCategoryAsync(category.Id);

        //    // Assert
        //    imageUploadMock.Verify(x => x.DeleteImage(imageUrl, "CategoryImages"), Times.Once);
        //}



        [Fact]
        public async Task ShouldRollbackTransaction_WhenExceptionOccurs()
        {
            // Arrange
            var category = new Category { Id = 7, Name = "Moving", ImageUrl = BaseUrl + "/Images/Category/7/old.png" };
            categoryRepoMock.Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);

            var transactionMock = new Mock<IDbContextTransaction>();
            unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);

            categoryRepoMock
                .Setup(x => x.DeleteAsync(category, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Delete failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteCategoryAsync(category.Id));

            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            imageUploadMock.Verify(
                x => x.DeleteImage(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        // ---------------------------------------------------------------
        // UpdateCategoryAsync
        // ---------------------------------------------------------------

        [Fact]
        public async Task ShouldUpdateCategory()
        {
            // Arrange
            var existingCategory = new Category { Id = 8, Name = "Old Name" };
            var updatedCategory = new Category { Id = 8, Name = "New Name" };
            categoryRepoMock.Setup(x => x.GetByIdAsync(8, It.IsAny<CancellationToken>())).ReturnsAsync(existingCategory);

            // Act
            await sut.UpdateCategoryAsync(8, updatedCategory, null);

            // Assert
            Assert.Equal("New Name", existingCategory.Name);
            categoryRepoMock.Verify(x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ShouldReplaceOldImage()
        {
            // Arrange
            var oldImageUrl = BaseUrl + "/Images/Category/9/old.png";
            var existingCategory = new Category { Id = 9, Name = "Roofing", ImageUrl = oldImageUrl };
            var updatedCategory = new Category { Id = 9, Name = "Roofing" };
            var formFile = CreateFormFileMock().Object;
            var newImagePath = "/Images/Category/9/new.png";

            categoryRepoMock.Setup(x => x.GetByIdAsync(9, It.IsAny<CancellationToken>())).ReturnsAsync(existingCategory);
            imageUploadMock
                .Setup(x => x.UploadImageAsync(formFile, "Images/Category/9", It.IsAny<CancellationToken>()))
                .ReturnsAsync(newImagePath);

            // Act
            await sut.UpdateCategoryAsync(9, updatedCategory, formFile);

            // Assert
            Assert.Equal(BaseUrl + newImagePath, existingCategory.ImageUrl);
            imageUploadMock.Verify(x => x.DeleteImage(oldImageUrl, "Images/Category/9"), Times.Once);
        }

        [Fact]
        public async Task ShouldKeepOldImage_WhenNoNewImage()
        {
            // Arrange
            var oldImageUrl = BaseUrl + "/Images/Category/10/old.png";
            var existingCategory = new Category { Id = 10, Name = "Flooring", ImageUrl = oldImageUrl };
            var updatedCategory = new Category { Id = 10, Name = "Flooring" };

            categoryRepoMock.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(existingCategory);

            // Act
            await sut.UpdateCategoryAsync(10, updatedCategory, null);

            // Assert
            Assert.Equal(oldImageUrl, existingCategory.ImageUrl);
            imageUploadMock.Verify(
                x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
            imageUploadMock.Verify(
                x => x.DeleteImage(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}