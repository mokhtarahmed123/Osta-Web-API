using Osta.Data.Entities;

namespace Osta.Service.Abstract.CustomerAbstract
{
    public interface IFavoriteTechnicianService
    {
        Task Add(FavoriteTechnician favoriteTechnician, CancellationToken ct = default);
        Task Delete(string CustomerId, string TechnicianId, CancellationToken ct = default);
        Task<List<FavoriteTechnician>> GetMyFavorites(
            string customerId, CancellationToken ct = default);
    }
}
