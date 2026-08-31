using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Services;
using Osta.Infrastructure.Abstract.ServicesAbstract;
using Osta.Infrastructure.Caching;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel;
using Osta.SharedKernel.Logging;
using System.Diagnostics;

namespace Osta.Service.Service.ServicesServiceFolder
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepo;
        private readonly IFileService imageUpload;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebHostEnvironment env;
        private readonly ILoggerService loggerService;
        private readonly ICacheService cacheService;
        private const string CategoriesCacheKey = "categories";


        public CategoryService(ICategoryRepository categoryRepo, IFileService imageUpload, IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, ILoggerService loggerService, ICacheService? cacheService
            )
        {
            this.categoryRepo = categoryRepo;
            this.imageUpload = imageUpload;
            this.unitOfWork = unitOfWork;
            this.httpContextAccessor = httpContextAccessor;
            this.env = env;
            this.loggerService = loggerService;
            this.cacheService = cacheService;

        }
        public async Task AddCategoryAsync(Category category, IFormFile? formFile, CancellationToken ct = default)
        {


            if (formFile != null)
            {
                var request = httpContextAccessor.HttpContext?.Request
                   ?? throw new InvalidOperationException("No HTTP context");
                var baseUrl = $"{request.Scheme}://{request.Host}";

                var location = $"Images/Category/{category.Id}";

                var imagePath = await imageUpload.UploadImageAsync(formFile, location, ct);
                category.ImageUrl = baseUrl + imagePath;
            }

            try
            {

                await categoryRepo.AddAsync(category, ct);
                await unitOfWork.SaveChangesAsync();
                await cacheService.RemoveAsync(CategoriesCacheKey);

            }
            catch (Exception ex)
            {
                if (category.ImageUrl != null)
                    await imageUpload.DeleteImage(category.ImageUrl, $"Images/Category/{category.Id}");
                loggerService.LogError(ex, $"Failed to add Category {category.Id}");

                throw;
            }
        }

        public async Task DeleteCategoryAsync(int id, CancellationToken ct = default)
        {
            var category = await categoryRepo.GetByIdAsync(id, ct);
            if (category == null) return;
            var imageUrl = category.ImageUrl;

            await using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                await categoryRepo.DeleteAsync(category, ct);
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                await cacheService.RemoveAsync(CategoriesCacheKey);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                loggerService.LogError(ex, $"Failed to Delete Category {category.Id}");
                throw;
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                await imageUpload.DeleteImage(imageUrl, $"Images/Category/{category.Id}");
            }


        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken ct = default)
        {
            var cachedCategories =
                await cacheService.GetDataAsync<List<Category>>(CategoriesCacheKey);

            if (cachedCategories is not null)
            {
                loggerService.LogInformation("Categories loaded from cache.");
                return cachedCategories;
            }

            var sw = Stopwatch.StartNew();

            var categories = (await categoryRepo.GetAllAsync(ct)).ToList();

            sw.Stop();

            loggerService.LogInformation(
                "Database took {Elapsed} ms",
                sw.ElapsedMilliseconds);

            await cacheService.SetDataAsync(
                CategoriesCacheKey,
                categories,
                TimeSpan.FromHours(1));

            loggerService.LogInformation("Categories cached successfully.");

            return categories;
        }
        public async Task<Category?> GetCategoryAsync(int id, CancellationToken ct = default)
        {
            return await categoryRepo.GetByIdAsync(id, ct);
        }

        public async Task UpdateCategoryAsync(int id, Category category, IFormFile? formFile, CancellationToken ct = default)
        {
            try
            {
                var existingCategory = await categoryRepo.GetByIdAsync(id, ct);

                if (existingCategory == null)
                    throw new Exception($"Category with ID {id} not found.");

                existingCategory.Name = category.Name;

                if (formFile != null)
                {
                    var request = httpContextAccessor.HttpContext?.Request
                        ?? throw new InvalidOperationException("No HTTP context");

                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    var location = $"Images/Category/{id}";

                    var imagePath = await imageUpload.UploadImageAsync(formFile, location, ct);

                    var oldImageUrl = existingCategory.ImageUrl;

                    existingCategory.ImageUrl = baseUrl + imagePath;

                    if (!string.IsNullOrEmpty(oldImageUrl))
                    {
                        await imageUpload.DeleteImage(oldImageUrl, $"Images/Category/{id}");
                    }
                }

                await categoryRepo.UpdateAsync(existingCategory, ct);
                await unitOfWork.SaveChangesAsync();
                await cacheService.RemoveAsync(CategoriesCacheKey);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Failed to add Category {category.Id}");

                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<bool> IsCategoryNameExistsAsync(string name, CancellationToken ct = default)
        {
            bool exists = await categoryRepo.ExistsAsync(c => c.Name.ToLower() == name.ToLower(), ct);
            return exists;
        }

        public async Task<bool> IsCategoryNameExistsForOtherCategoryAsync(string name, int id, CancellationToken ct = default)
        {

            return await categoryRepo.GetTableNoTracking(ct)
                .AnyAsync(x => x.Name == name && x.Id != id);
        }

        public IQueryable<Category> GetAllCategoriesQueryable(CancellationToken ct = default)
        {
            return categoryRepo.GetTableNoTracking(ct).AsQueryable();

        }
    }
}