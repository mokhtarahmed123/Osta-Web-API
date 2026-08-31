using Microsoft.AspNetCore.Http;
using Osta.Data.Entities.Services;

namespace Osta.Service.Abstract.ServicesAbstract
{
    public interface ICategoryService
    {
        Task AddCategoryAsync(Category category, IFormFile? formFile, CancellationToken ct = default);
        Task UpdateCategoryAsync(int id, Category category, IFormFile? formFile, CancellationToken ct = default);
        Task DeleteCategoryAsync(int id, CancellationToken ct = default);
        Task<Category?> GetCategoryAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken ct = default);
        IQueryable<Category> GetAllCategoriesQueryable(CancellationToken ct = default);

        Task<bool> IsCategoryNameExistsAsync(string name, CancellationToken ct = default);
        Task<bool> IsCategoryNameExistsForOtherCategoryAsync(string name, int id, CancellationToken ct = default);

    }
}
