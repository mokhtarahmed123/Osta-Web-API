namespace Osta.Data.Entities.Technician
{
    public class TechnicianServiceArea
    {
        public string TechnicianId { get; set; } = string.Empty;
        public int ServiceAreaId { get; set; }
        public Technicians Technician { get; set; } = null!;
        public ServiceArea ServiceArea { get; set; } = null!;
    }
}
