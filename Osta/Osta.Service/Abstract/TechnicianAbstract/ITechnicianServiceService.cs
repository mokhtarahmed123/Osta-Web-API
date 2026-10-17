using Osta.Data.Entities.Technician;

namespace Osta.Service.Abstract.TechnicianAbstract
{
    public interface ITechnicianServiceService
    {
<<<<<<< HEAD
        // Basic CRUD
=======

>>>>>>> c73a0b6 (Initial commit)
        Task AddAsync(TechnicianService technicianService, CancellationToken cancellationToken);
        Task DeleteAsync(TechnicianService technicianService, CancellationToken cancellationToken);
        Task UpdateAsync(TechnicianService technicianService, CancellationToken cancellationToken);

<<<<<<< HEAD
        // Get all records
        Task<IEnumerable<TechnicianService>> GetAllAsync(CancellationToken cancellationToken);

        // Get by composite key (TechnicianId + ServiceId)
        Task<TechnicianService?> GetByIdAsync(string technicianId, int serviceId, CancellationToken cancellationToken);

        // Get all services for a specific technician
        Task<IEnumerable<TechnicianService>> GetByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken);

        // Get all technicians for a specific service
        Task<IEnumerable<TechnicianService>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken);

        // Check if a specific technician-service pair already exists
        Task<bool> ExistsAsync(string technicianId, int serviceId, CancellationToken cancellationToken);

        // Assign multiple services to a technician at once
=======

        Task<IEnumerable<TechnicianService>> GetAllAsync(CancellationToken cancellationToken);


        Task<TechnicianService?> GetByIdAsync(string technicianId, int serviceId, CancellationToken cancellationToken);


        Task<IEnumerable<TechnicianService>> GetByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken);

        Task<IEnumerable<TechnicianService>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken);


        Task<bool> ExistsAsync(string technicianId, int serviceId, CancellationToken cancellationToken);


>>>>>>> c73a0b6 (Initial commit)
        Task AddRangeAsync(IEnumerable<TechnicianService> technicianServices, CancellationToken cancellationToken);
        Task DeleteAllService_technicianBy_technicianIdAsync(string technicianId, CancellationToken cancellationToken);
        Task DeleteAllService_technicianBy_ServiceIdAsync(int ServiceId, CancellationToken cancellationToken);

<<<<<<< HEAD

        // Remove all services for a technician (e.g. when updating his services list)
        Task DeleteServiceByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken);

        // Remove all technicians for a service (e.g. when deleting a service)
=======
        Task DeleteServiceByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken);


>>>>>>> c73a0b6 (Initial commit)
        Task DeleteByServiceIdAsync(int serviceId, CancellationToken cancellationToken);



    }
}