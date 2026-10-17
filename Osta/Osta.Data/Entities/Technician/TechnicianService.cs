using Osta.Data.Entities.Services;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Technician
{
    [Table("TechnicianServices", Schema = "Technician")]
=======

namespace Osta.Data.Entities.Technician
{

>>>>>>> c73a0b6 (Initial commit)
    public class TechnicianService
    {

        public string TechnicianId { get; set; }
        public int ServiceId { get; set; }
        public Technicians Technician { get; set; } = null!;
        public Service Service { get; set; } = null!;


    }
}
