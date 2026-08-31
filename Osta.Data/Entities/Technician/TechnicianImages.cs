using Osta.Data.Entities.Technician;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Technician
{
    [Table("TechnicianImages", Schema = "Technician")]
    public class TechnicianImages
    {
        [Key, ForeignKey(nameof(Technician))]
        public string TechnicianId { get; set; } = string.Empty;
        public Technicians Technician { get; set; } = null!;

        public string? ProfilePicture { get; set; }
        public string FrontNationalIdImage { get; set; } = string.Empty;
        public string BackNationalIdImage { get; set; } = string.Empty;

    }
}
