using Microsoft.AspNetCore.Http;

namespace Osta.Core.Feature.Technician.Command.Model.ModelTechnicianImage
{
    public class UpdateModelTechnicianImage
    {
        public IFormFile? ProfileImage { get; set; }
        public IFormFile? FrontNationalIdImage { get; set; }
        public IFormFile? BackNationalIdImage { get; set; }
    }
}
