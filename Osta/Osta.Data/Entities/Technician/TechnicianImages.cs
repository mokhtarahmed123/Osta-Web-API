using Osta.Data.Entities.Technician;

namespace Osta.Domain.Entities.Technician
{

    public class TechnicianImages
    {

        public string TechnicianId { get; set; } = string.Empty;
        public Technicians Technician { get; set; } = null!;

        public string? ProfilePicture { get; set; }
        public string FrontNationalIdImage { get; set; } = string.Empty;
        public string BackNationalIdImage { get; set; } = string.Empty;

    }
}
