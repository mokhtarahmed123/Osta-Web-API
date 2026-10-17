using Osta.Data.Entities.Services;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Technician
{
    [Table("TechnicianServices", Schema = "Technician")]
    public class TechnicianService
    {

        public string TechnicianId { get; set; }
        public int ServiceId { get; set; }
        public Technicians Technician { get; set; } = null!;
        public Service Service { get; set; } = null!;


    }
}
