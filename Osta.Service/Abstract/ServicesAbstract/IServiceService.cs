using Microsoft.AspNetCore.Http;

namespace Osta.Service.Abstract.ServicesAbstract
{
    using Service = Osta.Data.Entities.Services.Service;
    public interface IServiceService
    {
        Task AddServiceAsync(Service service, IFormFile? formFile, CancellationToken ct = default);
        Task UpdateServiceAsync(int id, Service service, IFormFile? formFile, CancellationToken ct = default);
        Task DeleteServiceAsync(int id, CancellationToken ct = default);
        Task<Service?> GetServiceAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Service>> GetAllServicesAsync(CancellationToken ct = default);
        public IQueryable<Service> GetAllServicesQueryable();
        Task<bool> DoesCategoryHaveServiceAsync(int CategoryId, CancellationToken ct = default);
        Task<IEnumerable<Service>> GetServicesByTechnicianIdAsync(string TechId, CancellationToken ct = default);



    }
}
