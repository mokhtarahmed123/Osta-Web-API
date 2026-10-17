<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Technician
{
    [Table("TechnicianServiceAreas", Schema = "Technician")]
=======
﻿namespace Osta.Data.Entities.Technician
{
>>>>>>> c73a0b6 (Initial commit)
    public class TechnicianServiceArea
    {
        public string TechnicianId { get; set; } = string.Empty;
        public int ServiceAreaId { get; set; }
        public Technicians Technician { get; set; } = null!;
        public ServiceArea ServiceArea { get; set; } = null!;
    }
}
