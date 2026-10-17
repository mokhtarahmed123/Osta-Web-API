using Osta.Data.Entities.Services;

namespace Osta.Data.Entities.Technician
{

    public class TechnicianService
    {

        public string TechnicianId { get; set; }
        public int ServiceId { get; set; }
        public Technicians Technician { get; set; } = null!;
        public Service Service { get; set; } = null!;


    }
}
