using Osta.Data.Entities.Technician;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Technician
{
    [Table("TechnicianImages", Schema = "Technician")]
=======

namespace Osta.Domain.Entities.Technician
{

>>>>>>> c73a0b6 (Initial commit)
    public class TechnicianImages
    {

        public string TechnicianId { get; set; } = string.Empty;
        public Technicians Technician { get; set; } = null!;

        public string? ProfilePicture { get; set; }
        public string FrontNationalIdImage { get; set; } = string.Empty;
        public string BackNationalIdImage { get; set; } = string.Empty;

    }
}
