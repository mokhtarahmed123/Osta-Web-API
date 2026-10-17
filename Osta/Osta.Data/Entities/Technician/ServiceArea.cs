using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Technician
{
    [Table("ServiceAreas", Schema = "Technician")]

    public class ServiceArea
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;


        public ICollection<TechnicianServiceArea> TechnicianServiceAreas { get; set; } = new List<TechnicianServiceArea>();

    }
}
