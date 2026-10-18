using Microsoft.AspNetCore.Http;

namespace Osta.Service.Model
{
    public class TechnicianImageModel
    {
        public required IFormFile ProfileImage { get; set; }
        public required IFormFile FrontNationalIdImage { get; set; }
        public required IFormFile BackNationalIdImage { get; set; }
    }
}
