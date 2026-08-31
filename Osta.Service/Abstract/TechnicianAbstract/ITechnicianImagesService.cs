using Osta.Domain.Entities.Technician;
using Osta.Service.Model;

namespace Osta.Service.Abstract.TechnicianAbstract
{
    public interface ITechnicianImagesService
    {
        Task<TechnicianImages> Add(string TechnicianId, TechnicianImageModel technicianImages, CancellationToken ct = default);
        Task<TechnicianImages> Update(string TechnicianId, TechnicianImageModel technicianImages, CancellationToken ct = default);
        Task Delete(string TechnicianId, CancellationToken ct = default);
        Task<TechnicianImages> Get(string TechnicianId, CancellationToken ct = default);
    }
}
